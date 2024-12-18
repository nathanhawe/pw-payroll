﻿using Microsoft.AspNetCore.Authorization;
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
	public class CrewLaborWageController : ControllerBase
	{
		private readonly ILogger<CrewLaborWageController> _logger;
		private readonly ICrewLaborWageService _crewLaborWageService;
		private readonly IUserActionService _userActionService;

		public CrewLaborWageController(
			ILogger<CrewLaborWageController> logger,
			ICrewLaborWageService crewLaborWageService,
			IUserActionService userActionService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_crewLaborWageService = crewLaborWageService ?? throw new ArgumentNullException(nameof(crewLaborWageService));
			_userActionService = userActionService ?? throw new ArgumentNullException(nameof(userActionService));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeViewingUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<IEnumerable<Domain.CrewLaborWage>>> Get(
			int offset = 0,
			int limit = 50,
			bool orderByDescending = true)
		{
			var wages = _crewLaborWageService.GetWages(offset, limit, orderByDescending);
			int wageCount = _crewLaborWageService.GetTotalCrewLaborWageCount();

			return Ok(
				new ApiResponse<IEnumerable<Domain.CrewLaborWage>>
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
		public ActionResult<ApiResponse<Domain.CrewLaborWage>> Get(
			int id)
		{
			var wage = _crewLaborWageService.GetWage(id);
			return Ok(
				new ApiResponse<Domain.CrewLaborWage>
				{
					Data = wage
				});
		}

		[HttpPost]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public ActionResult<ApiResponse<Domain.CrewLaborWage>> Post(
			[FromBody]CrewLaborWageViewModel viewModel)
		{
			var crewLaborWage = new Domain.CrewLaborWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};
			_crewLaborWageService.AddWage(crewLaborWage);
			LogUserAction("POST", crewLaborWage);

			return Created(
				$"api/crewlaborwage/{crewLaborWage.Id}",
				new ApiResponse<Domain.CrewLaborWage>
				{
					Data = crewLaborWage
				}
			);
		}

		[HttpPut("{id:int}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<ApiResponse<Domain.CrewLaborWage>> Put(
			int id, 
			[FromBody]CrewLaborWageViewModel viewModel)
		{
			var crewLaborWage = new Domain.CrewLaborWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};

			try
			{
				var updatedWage = _crewLaborWageService.UpdateWage(id, crewLaborWage);
				LogUserAction("PUT", updatedWage);
				return Ok(
					new ApiResponse<Domain.CrewLaborWage>
					{
						Data = updatedWage
					});
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, "Unable to update {CrewLaborWage} because an exception occured. {Exception}", viewModel, ex);
				return BadRequest(
					new ApiResponse<Domain.CrewLaborWage>
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
			_crewLaborWageService.DeleteWage(id);
			LogUserAction("DELETE", id);
			return NoContent();
		}

		[NonAction]
		private void LogUserAction(string action, Domain.CrewLaborWage crewLaborWage)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			var message = JsonSerializer.Serialize(crewLaborWage);
			_userActionService.AddActionForSubject(subjectFromToken, $"Crew Labor Wage ({action}) '{message}'");
		}

		[NonAction]
		private void LogUserAction(string action, int id)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			_userActionService.AddActionForSubject(subjectFromToken, $"Crew Labor Wage ({action}) ID:'{id}'");
		}
	}
}
