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
	public class BatchController : ControllerBase
	{
		private readonly ILogger<BatchController> _logger;
		private readonly IBatchService _batchService;
		private readonly IBackgroundTaskQueue _queue;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public BatchController(
			ILogger<BatchController> logger,
			IBatchService batchService,
			Infrastructure.HostedServices.IBackgroundTaskQueue queue,
			IServiceScopeFactory serviceScopeFactory)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
			_queue = queue ?? throw new ArgumentNullException(nameof(queue));
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<Models.ApiResponse<IEnumerable<Batch>>> Get(
			int offset = 0, 
			int limit = 20, 
			bool orderByDescending = true)
		{
			var batches = _batchService.GetBatches(offset, limit, orderByDescending);
			int batchCount = _batchService.GetTotalBatchCount();
			return Ok(new Models.ApiResponse<IEnumerable<Batch>>
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
		public ActionResult<Models.ApiResponse<Batch>> Get(
			int id)
		{
			var batch = _batchService.GetBatch(id);
			return Ok(new Models.ApiResponse<Batch>
			{
				Data = batch
			});
		}

		[HttpPost]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeBatchCreatingUser)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Models.ApiResponse<Batch>> Post(
			[FromBody]Models.BatchViewModel viewModel)
		{
			var errors = new Dictionary<string, List<string>>();
			var batch = new Batch
			{
				WeekEndDate = viewModel.WeekEndDate.Value,
				LayoffId = viewModel.LayoffId,
				Company = viewModel.Company
			};

			if (_batchService.CanAddBatch())
			{
				var sub = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "Unknown";
				_batchService.AddBatch(batch, sub);

				// Trigger calculations now that a batch has been added
				EnqueueTimeAndAttendanceCalculations(batch);
			}
			else
			{
				errors.Add("Batch Processing", new List<string> { "Unable to create a new batch while processing an existing batch." });
			}
			
			if(errors.Count == 0)
			{
				return Created($"api/batch/{batch.Id}",new Models.ApiResponse<Batch>
				{
					Data = batch
				});
			}
			else
			{
				return BadRequest(new Models.ApiResponse<Batch>
				{
					Errors = errors
				});
			}
		}

		[NonAction]
		private void EnqueueTimeAndAttendanceCalculations(Batch batch)
		{
			_logger.Log(LogLevel.Information, "{UserName} - ({userId}) enqueuing batch for time and attendance calculations. {Batch}", User.Identity.Name, User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value, batch);
			_queue.QueueBackgroundWorkItem(async token =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var timeAndAttendanceService = scope.ServiceProvider.GetRequiredService<ITimeAndAttendanceService>();
				timeAndAttendanceService.PerformCalculations(batch.Id);
			});
		}
	}
}
