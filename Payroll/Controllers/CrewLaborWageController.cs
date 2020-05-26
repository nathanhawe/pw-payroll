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
	public class CrewLaborWageController : ControllerBase
	{
		private readonly ILogger<CrewLaborWageController> _logger;
		private readonly ICrewLaborWageService _crewLaborWageService;

		public CrewLaborWageController(
			ILogger<CrewLaborWageController> logger,
			ICrewLaborWageService crewLaborWageService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_crewLaborWageService = crewLaborWageService ?? throw new ArgumentNullException(nameof(crewLaborWageService));
		}

		[HttpGet]
		public IEnumerable<Payroll.Domain.CrewLaborWage> Get()
		{
			return _crewLaborWageService.GetWages();
		}

		[HttpGet("{id:int}")]
		public Payroll.Domain.CrewLaborWage Get(
			int id)
		{
			return _crewLaborWageService.GetWage(id);
		}

		[HttpPost]
		public Payroll.Domain.CrewLaborWage Post(
			[FromBody]Models.CrewLaborWageViewModel viewModel)
		{
			var crewLaborWage = new Payroll.Domain.CrewLaborWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};
			_crewLaborWageService.AddWage(crewLaborWage);
			return crewLaborWage;
		}

		[HttpPut("{id:int}")]
		public Payroll.Domain.CrewLaborWage Put(
			int id, 
			[FromBody]Models.CrewLaborWageViewModel viewModel)
		{
			var crewLaborWage = new Payroll.Domain.CrewLaborWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};

			return _crewLaborWageService.UpdateWage(id, crewLaborWage);
		}

		[HttpDelete("{id:int}")]
		public Payroll.Domain.CrewLaborWage Delete(
			int id)
		{
			return _crewLaborWageService.DeleteWage(id);
		}

	}
}
