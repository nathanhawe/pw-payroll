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
	public class AuditLockBatchController : ControllerBase
	{
		private readonly ILogger<AuditLockBatchController> _logger;
		private readonly IAuditLockBatchService _auditLockBatchService;
		private readonly IBackgroundTaskQueue _queue;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public AuditLockBatchController(
			ILogger<AuditLockBatchController> logger,
			IAuditLockBatchService auditLockBatchService,
			Infrastructure.HostedServices.IBackgroundTaskQueue queue,
			IServiceScopeFactory serviceScopeFactory)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_auditLockBatchService = auditLockBatchService ?? throw new ArgumentNullException(nameof(auditLockBatchService));
			_queue = queue ?? throw new ArgumentNullException(nameof(queue));
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<Models.ApiResponse<IEnumerable<AuditLockBatch>>> Get(
			int offset = 0, 
			int limit = 20, 
			bool orderByDescending = true)
		{
			var batches = _auditLockBatchService.GetAuditLockBatches(offset, limit, orderByDescending);
			int batchCount = _auditLockBatchService.GetTotalAuditLockBatchCount();
			return Ok(new Models.ApiResponse<IEnumerable<AuditLockBatch>>
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
		public ActionResult<Models.ApiResponse<AuditLockBatch>> Get(
			int id)
		{
			var batch = _auditLockBatchService.GetAuditLockBatch(id);
			return Ok(new Models.ApiResponse<AuditLockBatch>
			{
				Data = batch
			});
		}

		[HttpPost]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeBatchCreatingUser)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Models.ApiResponse<AuditLockBatch>> Post(
			[FromBody]Models.AuditLockBatchViewModel viewModel)
		{
			var errors = new Dictionary<string, List<string>>();
			var batch = new AuditLockBatch
			{
				WeekEndDate = viewModel.WeekEndDate.Value,
				LayoffId = viewModel.LayoffId,
				Company = viewModel.Company,
				Lock = viewModel.Lock
			};

			if (_auditLockBatchService.CanAddAuditLockBatch())
			{
				var sub = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "Unknown";
				_auditLockBatchService.AddAuditLockBatch(batch, sub);

				// Trigger calculations now that a batch has been added
				EnqueueAuditLock(batch);
			}
			else
			{
				errors.Add("AuditLock Batch Processing", new List<string> { "Unable to create a new auditLock batch while an existing auditLock batch is processing." });
			}
			
			if(errors.Count == 0)
			{
				return Created($"api/auditLockbatch/{batch.Id}",new Models.ApiResponse<AuditLockBatch>
				{
					Data = batch
				});
			}
			else
			{
				return BadRequest(new Models.ApiResponse<AuditLockBatch>
				{
					Errors = errors
				});
			}
		}

		[NonAction]
		private void EnqueueAuditLock(AuditLockBatch batch)
		{
			_logger.Log(LogLevel.Information, "{UserName} - ({userId}) enqueuing audit lock batch for audit locking. {Batch}", User.Identity.Name, User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value, batch);
			_queue.QueueBackgroundWorkItem(async token =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var auditLockService = scope.ServiceProvider.GetRequiredService<IAuditLockService>();
				auditLockService.ProcessAuditLockBatch(batch.Id);
			});
		}
	}
}
