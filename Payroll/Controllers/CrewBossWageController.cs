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
	public class CrewBossWageController : ControllerBase
	{
		private readonly ILogger<CrewBossWageController> _logger;
		private readonly ICrewBossWageService _crewBossWageService;
		private readonly IUserActionService _userActionService;

		public CrewBossWageController(
			ILogger<CrewBossWageController> logger,
			ICrewBossWageService crewBossWageService,
			IUserActionService userActionService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_crewBossWageService = crewBossWageService ?? throw new ArgumentNullException(nameof(crewBossWageService));
			_userActionService = userActionService ?? throw new ArgumentNullException(nameof(userActionService));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<IEnumerable<Domain.CrewBossWage>>> Get(
			int offset = 0, 
			int limit = 50, 
			bool orderByDescending = true)
		{
			var wages = _crewBossWageService.GetWages(offset, limit, orderByDescending);
			int wageCount = _crewBossWageService.GetTotalCrewBossWageCount();

			return Ok(
				new ApiResponse<IEnumerable<Domain.CrewBossWage>>
				{
					Pagination = new Pagination
					{
						Offset = offset,
						Limit = limit,
						Total = wageCount,
						OrderByDescending = orderByDescending
					},
					Data = wages
				});
		}

		[HttpGet("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<Domain.CrewBossWage>> Get(
			int id)
		{
			var wage = _crewBossWageService.GetWage(id);
			return Ok(
				new ApiResponse<Domain.CrewBossWage>
				{
					Data = wage
				});
		}

		[HttpPost]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public ActionResult<ApiResponse<Domain.CrewBossWage>> Post(
			[FromBody]Models.CrewBossWageViewModel viewModel)
		{
			var crewBossWage = new Domain.CrewBossWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage,
				WorkerCountThreshold = viewModel.WorkerCountThreshold
			};
			_crewBossWageService.AddWage(crewBossWage);
			LogUserAction("POST", crewBossWage);

			return Created(
				$"api/crewbosswage/{crewBossWage.Id}",
				new ApiResponse<Domain.CrewBossWage>
				{
					Data = crewBossWage
				}
			);
		}

		[HttpPut("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<ApiResponse<Domain.CrewBossWage>> Put(
			int id, 
			[FromBody]Models.CrewBossWageViewModel viewModel)
		{
			var crewBossWage = new Domain.CrewBossWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage,
				WorkerCountThreshold = viewModel.WorkerCountThreshold
			};
			try
			{
				var updatedWage = _crewBossWageService.UpdateWage(id, crewBossWage);
				LogUserAction("PUT", updatedWage);
				return Ok(
					new ApiResponse<Domain.CrewBossWage>
					{
						Data = updatedWage
					});
			}
			catch(Exception ex)
			{
				_logger.Log(LogLevel.Error, "Unable to update {CrewBossWage} because an exception occured. {Exception}", viewModel, ex);
				return BadRequest(
					new ApiResponse<Domain.CrewBossWage>
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
			_crewBossWageService.DeleteWage(id);
			LogUserAction("DELETE", id);
			return NoContent();
		}

		[NonAction]
		private void LogUserAction(string action, Domain.CrewBossWage crewBossWage)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			var message = JsonSerializer.Serialize(crewBossWage);
			_userActionService.AddActionForSubject(subjectFromToken, $"Crew Boss Wage ({action}) '{message}'");
		}

		[NonAction]
		private void LogUserAction(string action, int id)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			_userActionService.AddActionForSubject(subjectFromToken, $"Crew Boss Wage ({action}) ID:'{id}'");
		}
	}
}
