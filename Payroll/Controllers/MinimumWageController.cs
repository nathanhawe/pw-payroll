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
	public class MinimumWageController : ControllerBase
	{
		private readonly ILogger<MinimumWageController> _logger;
		private readonly IMinimumWageService _minimumWageService;

		public MinimumWageController(
			ILogger<MinimumWageController> logger,
			IMinimumWageService minimumWageService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(minimumWageService));
		}

		[HttpGet]
		public IEnumerable<Payroll.Domain.MinimumWage> Get()
		{
			return _minimumWageService.GetWages();
		}

		[HttpGet("{id:int}")]
		public Payroll.Domain.MinimumWage Get(
			int id)
		{
			return _minimumWageService.GetWage(id);
		}

		[HttpPost]
		public Payroll.Domain.MinimumWage Post(
			[FromBody]Models.MinimumWageViewModel viewModel)
		{
			var minimumWage = new Payroll.Domain.MinimumWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};
			_minimumWageService.AddWage(minimumWage);
			return minimumWage;
		}

		[HttpPut("{id:int}")]
		public Payroll.Domain.MinimumWage Put(
			int id, 
			[FromBody]Models.MinimumWageViewModel viewModel)
		{
			var minimumWage = new Payroll.Domain.MinimumWage
			{
				EffectiveDate = viewModel.EffectiveDate,
				Wage = viewModel.Wage
			};

			return _minimumWageService.UpdateWage(id, minimumWage);
		}

		[HttpDelete("{id:int}")]
		public Payroll.Domain.MinimumWage Delete(
			int id)
		{
			return _minimumWageService.DeleteWage(id);
		}
	}
}
