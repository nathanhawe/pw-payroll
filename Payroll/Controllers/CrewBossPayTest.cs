using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payroll.Data.QuickBase;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CrewBossPayTest : ControllerBase
	{
		private readonly ILogger<CrewBossPayTest> _logger;
		private readonly ICrewBossPayRepo _crewBossPayRepo;

		public CrewBossPayTest(ILogger<CrewBossPayTest> logger, ICrewBossPayRepo crewBossPayRepo)
		{
			_logger = logger;
			_crewBossPayRepo = crewBossPayRepo;
		}

		[HttpGet]
		public IEnumerable<Payroll.Domain.CrewBossPayLine> Get()
		{
			_logger.Log(LogLevel.Information, "{UserName} - ({userId}) called Get(). {Claims}", User.Identity.Name, User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value, User.Claims);

			return _crewBossPayRepo.Get(new DateTime(2020, 5, 10), 0);
		}
	}
}
