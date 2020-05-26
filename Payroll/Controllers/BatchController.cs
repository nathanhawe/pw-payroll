using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payroll.Data;
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

		public BatchController(
			ILogger<BatchController> logger,
			IBatchService batchService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
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
			return batch;
		}
	}
}
