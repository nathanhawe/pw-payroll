using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payroll.Domain;
using Payroll.Models;
using Payroll.Service.Interface;
using System;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class SummaryBatchProcessingStatusController : ControllerBase
	{
		private readonly ILogger<SummaryBatchProcessingStatusController> _logger;
		private readonly ISummaryBatchService _summaryBatchService;

		public SummaryBatchProcessingStatusController(
			ILogger<SummaryBatchProcessingStatusController> logger,
			ISummaryBatchService summaryBatchService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_summaryBatchService = summaryBatchService ?? throw new ArgumentNullException(nameof(summaryBatchService));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<SummaryBatch>> Get()
		{
			var batch = _summaryBatchService.GetCurrentlyProcessingSummaryBatch();
			return Ok(new ApiResponse<SummaryBatch>
			{
				Data = batch				
			});
		}
	}
}
