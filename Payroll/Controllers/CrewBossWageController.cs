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
	public class CrewBossWageController : ControllerBase
	{
		private readonly ILogger<CrewBossWageController> _logger;
		private readonly ICrewBossWageService _crewBossWageService;

		public CrewBossWageController(
			ILogger<CrewBossWageController> logger,
			ICrewBossWageService crewBossWageService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_crewBossWageService = crewBossWageService ?? throw new ArgumentNullException(nameof(crewBossWageService));
		}

		[HttpGet]
		public IEnumerable<Payroll.Domain.CrewBossWage> Get()
		{
			return _crewBossWageService.GetWages();
		}

		[HttpGet("{id:int}")]
		public Payroll.Domain.CrewBossWage Get(
			int id)
		{
			return _crewBossWageService.GetWage(id);
		}

		[HttpPost]
		public Payroll.Domain.CrewBossWage Post(
			[FromBody]Models.CrewBossWageViewModel viewModel)
		{
			var crewBossWage = new Payroll.Domain.CrewBossWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage,
				WorkerCountThreshold = viewModel.WorkerCountThreshold
			};
			_crewBossWageService.AddWage(crewBossWage);
			return crewBossWage;
		}

		[HttpPut("{id:int}")]
		public Payroll.Domain.CrewBossWage Put(
			int id, 
			[FromBody]Models.CrewBossWageViewModel viewModel)
		{
			var crewBossWage = new Payroll.Domain.CrewBossWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage,
				WorkerCountThreshold = viewModel.WorkerCountThreshold
			};

			return _crewBossWageService.UpdateWage(id, crewBossWage);
		}

		[HttpDelete("{id:int}")]
		public Payroll.Domain.CrewBossWage Delete(
			int id)
		{
			return _crewBossWageService.DeleteWage(id);
		}

	}
}
