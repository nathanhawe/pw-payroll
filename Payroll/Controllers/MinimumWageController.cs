using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payroll.Models;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class MinimumWageController : ControllerBase
	{
		private readonly ILogger<MinimumWageController> _logger;
		private readonly IMinimumWageService _minimumWageService;
		private readonly IUserActionService _userActionService;

		public MinimumWageController(
			ILogger<MinimumWageController> logger,
			IMinimumWageService minimumWageService,
			IUserActionService userActionService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(minimumWageService));
			_userActionService = userActionService ?? throw new ArgumentNullException(nameof(userActionService));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<IEnumerable<Domain.MinimumWage>>> Get(
			int offset=0, 
			int limit=50, 
			bool orderByDescending=true)
		{

			var wages = _minimumWageService.GetWages(offset, limit, orderByDescending);
			int wageCount = _minimumWageService.GetTotalMininumWageCount();

			return Ok(
				new ApiResponse<IEnumerable<Domain.MinimumWage>>
				{
					Data = wages,
					Pagination = new Pagination
					{
						Offset = offset,
						Limit = limit,
						Total = wageCount,
						OrderByDescending = orderByDescending
					}
				});
		}

		[HttpGet("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<Domain.MinimumWage>> Get(
			int id)
		{
			var wage = _minimumWageService.GetWage(id);

			return Ok(
				new ApiResponse<Domain.MinimumWage>
				{
					Data = wage
				});
		}

		[HttpPost]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public ActionResult<ApiResponse<Domain.MinimumWage>> Post(
			[FromBody]MinimumWageViewModel viewModel)
		{
			var minimumWage = new Domain.MinimumWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};
			_minimumWageService.AddWage(minimumWage);
			LogUserAction("POST", minimumWage);

			return Created(
				$"api/minimumwage/{minimumWage.Id}",
				new ApiResponse<Domain.MinimumWage>
				{
					Data = minimumWage
				});
		}

		[HttpPut("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<ApiResponse<Domain.MinimumWage>> Put(
			int id, 
			[FromBody]MinimumWageViewModel viewModel)
		{
			var minimumWage = new Domain.MinimumWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};

			try
			{
				var updatedWage = _minimumWageService.UpdateWage(id, minimumWage);
				LogUserAction("PUT", updatedWage);
				return Ok(
					new ApiResponse<Domain.MinimumWage>
					{
						Data = updatedWage
					});
			}
			catch(Exception ex)
			{
				_logger.LogError("Unable to update MinimumWage with {ViewMode} because an exception was thrown. {Exception}", viewModel, ex);
				return BadRequest(
					new ApiResponse<Domain.MinimumWage>
					{
						Errors = new Dictionary<string, List<string>>
						{
							{"", new List<string>{ "Unable to complete update because an error occured."} }
						}
					});
			}
		}

		[HttpDelete("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public ActionResult Delete(
			int id)
		{
			_minimumWageService.DeleteWage(id);
			LogUserAction("DELETE", id);
			return NoContent();
		}

		[NonAction]
		private void LogUserAction(string action, Domain.MinimumWage minimumWage)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			var message = JsonSerializer.Serialize(minimumWage);
			_userActionService.AddActionForSubject(subjectFromToken, $"Minimum Wage ({action}) '{message}'");
		}

		[NonAction]
		private void LogUserAction(string action, int id)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			_userActionService.AddActionForSubject(subjectFromToken, $"Minimum Wage ({action}) ID:'{id}'");
		}
	}
}
