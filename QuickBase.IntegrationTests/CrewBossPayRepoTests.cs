using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickBase.IntegrationTests
{
	[TestClass]
	public class CrewBossPayRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private CrewBossPayRepo _repo;

		[TestInitialize]
		public void Setup()
		{
			if (_configuration == null)
			{
				_configuration = ConfigurationHelper.GetIConfigurationRoot();
			}

			var realm = _configuration["QuickBase:Realm"];
			var userToken = _configuration["QuickBase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();
			_quickBaseConn = new QuickBaseConnection(realm, userToken, logger);
			_repo = new CrewBossPayRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_NoLayoff()
		{
			var temp = _repo.Get(new DateTime(2021, 6, 20), 0);
			Print(temp);
		}

		[TestMethod]
		public void DoQuery_WithLayoff()
		{
			var temp = _repo.Get(new DateTime(2020, 3, 1), 920);
			Print(temp);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV()
		{
			var crewBossPayLines = new List<CrewBossPayLine>
			{
				new CrewBossPayLine{ QuickBaseRecordId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, HoursWorked = 9M, WorkerCount = 20, EmployeeId = "1006"},
				new CrewBossPayLine{ QuickBaseRecordId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, HoursWorked = 9M, WorkerCount = 21, EmployeeId = "1006"},
				new CrewBossPayLine{ QuickBaseRecordId = 0, ShiftDate = new DateTime(2019, 1, 3), Crew = 1002, HoursWorked = 9M, WorkerCount = 22, EmployeeId = "1006"},
				new CrewBossPayLine{ QuickBaseRecordId = 0, ShiftDate = new DateTime(2019, 1, 4), Crew = 1002, HoursWorked = 9M, WorkerCount = 23, EmployeeId = "1006"}
			};

			_repo.PostBatchSize = 3;
			_repo.Save(crewBossPayLines);
		}

		private void Print(IEnumerable<CrewBossPayLine> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' CrewBossPayLines:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(CrewBossPayLine line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(CrewBossPayLine).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
