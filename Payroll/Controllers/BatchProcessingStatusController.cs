using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Infrastructure.HostedServices;
using Payroll.Models;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class BatchProcessingStatusController : ControllerBase
	{
		private readonly ILogger<BatchProcessingStatusController> _logger;
		private readonly IBatchService _batchService;

		public BatchProcessingStatusController(
			ILogger<BatchProcessingStatusController> logger,
			IBatchService batchService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<Batch>> Get()
		{
			var batch = _batchService.GetCurrentlyProcessingBatch();
			return Ok(new ApiResponse<Batch>
			{
				Data = batch				
			});
		}
	}
}
