using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Infrastructure.HostedServices;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	//[Authorize]
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
		public IEnumerable<Payroll.Domain.Batch> Get(int pageNumber = 1, int itemsPerPage = 20, bool orderByDescending = true)
		{
			return _batchService.GetBatches(pageNumber, itemsPerPage, orderByDescending);
		}

		[HttpGet("{id:int}")]
		public Payroll.Domain.Batch Get(
			int id)
		{
			return _batchService.GetBatch(id);
		}

		[HttpPost]
		public Payroll.Domain.Batch Post(
			[FromBody]Models.BatchViewModel viewModel)
		{
			var batch = new Payroll.Domain.Batch
			{
				WeekEndDate = viewModel.WeekEndDate.Value,
				LayoffId = viewModel.LayoffId,
				Company = viewModel.Company
			};

			_batchService.AddBatch(batch, "ToDo");

			// Trigger calculations now that a batch has been added
			EnqueueTimeAndAttendanceCalculations(batch);

			return batch;
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
