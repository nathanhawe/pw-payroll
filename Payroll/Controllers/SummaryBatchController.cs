using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payroll.Domain;
using Payroll.Infrastructure.HostedServices;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class SummaryBatchController : ControllerBase
	{
		private readonly ILogger<SummaryBatchController> _logger;
		private readonly ISummaryBatchService _summaryBatchService;
		private readonly IBackgroundTaskQueue _queue;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public SummaryBatchController(
			ILogger<SummaryBatchController> logger,
			ISummaryBatchService summaryBatchService,
			Infrastructure.HostedServices.IBackgroundTaskQueue queue,
			IServiceScopeFactory serviceScopeFactory)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_summaryBatchService = summaryBatchService ?? throw new ArgumentNullException(nameof(summaryBatchService));
			_queue = queue ?? throw new ArgumentNullException(nameof(queue));
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<Models.ApiResponse<IEnumerable<SummaryBatch>>> Get(
			int offset = 0, 
			int limit = 20, 
			bool orderByDescending = true)
		{
			var batches = _summaryBatchService.GetSummaryBatches(offset, limit, orderByDescending);
			int batchCount = _summaryBatchService.GetTotalSummaryBatchCount();
			return Ok(new Models.ApiResponse<IEnumerable<SummaryBatch>>
			{
				Data = batches,
				Pagination = new Models.Pagination
				{
					Offset = offset,
					Limit = limit,
					Total = batchCount,
					OrderByDescending = orderByDescending
				}
			});
		}

		[HttpGet("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<Models.ApiResponse<SummaryBatch>> Get(
			int id)
		{
			var batch = _summaryBatchService.GetSummaryBatch(id);
			return Ok(new Models.ApiResponse<SummaryBatch>
			{
				Data = batch
			});
		}

		[HttpPost]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeBatchCreatingUser)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Models.ApiResponse<SummaryBatch>> Post(
			[FromBody]Models.BatchViewModel viewModel)
		{
			var errors = new Dictionary<string, List<string>>();
			var batch = new SummaryBatch
			{
				WeekEndDate = viewModel.WeekEndDate.Value,
				LayoffId = viewModel.LayoffId,
				Company = viewModel.Company
			};

			if (_summaryBatchService.CanAddSummaryBatch())
			{
				var sub = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "Unknown";
				_summaryBatchService.AddSummaryBatch(batch, sub);

				// Trigger calculations now that a batch has been added
				EnqueueSummaryCreation(batch);
			}
			else
			{
				errors.Add("Summary Batch Processing", new List<string> { "Unable to create a new summary batch while an existing summary batch is processing." });
			}
			
			if(errors.Count == 0)
			{
				return Created($"api/summarybatch/{batch.Id}",new Models.ApiResponse<SummaryBatch>
				{
					Data = batch
				});
			}
			else
			{
				return BadRequest(new Models.ApiResponse<SummaryBatch>
				{
					Errors = errors
				});
			}
		}

		[NonAction]
		private void EnqueueSummaryCreation(SummaryBatch batch)
		{
			_logger.Log(LogLevel.Information, "{UserName} - ({userId}) enqueuing summary batch for summary creation. {Batch}", User.Identity.Name, User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value, batch);
			_queue.QueueBackgroundWorkItem(async token =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var summaryCreationService = scope.ServiceProvider.GetRequiredService<ISummaryCreationService>();
				summaryCreationService.CreateSummaries(batch.Id);
			});
		}
	}
}
