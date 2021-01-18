using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using Payroll.Domain.Constants;
using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickBase.IntegrationTests
{
	[TestClass]
	public class RanchSummariesRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private RanchSummariesRepo _repo;

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
			_repo = new RanchSummariesRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery()
		{
			var temp = _repo.Get(new DateTime(2020, 3, 29));
			Print(temp);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV()
		{
			var ranchSummaries = new List<RanchSummary>
			{
				new RanchSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), LastCrew = 1002, EmployeeId = "25,18C", TotalHours = 100M, TotalGross = 1000.99M, CulturalHours = 9.99M },
				new RanchSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), LastCrew = 1002, EmployeeId = "29,37D", TotalHours = 110M, TotalGross = 1001M, CulturalHours = 9.98M },
				new RanchSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), LastCrew = 1002, EmployeeId = "48,70C", TotalHours = 120M, TotalGross = 1001.01M, CulturalHours = 9.97M },
				new RanchSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), LastCrew = 1002, EmployeeId = "48,67C", TotalHours = 130M, TotalGross = 1001.02M, CulturalHours = 9.96M },
				new RanchSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), LastCrew = 1002, EmployeeId = "25,18C", TotalHours = 140M, TotalGross = 1001.03M, CulturalHours = 9.95M },
				new RanchSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), LastCrew = 1002, EmployeeId = "29,37D", TotalHours = 150M, TotalGross = 1001.04M, CulturalHours = 9.94M },
				new RanchSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), LastCrew = 1002, EmployeeId = "48,70C", TotalHours = 160M, TotalGross = 1001.05M, CulturalHours = 9.93M },
				new RanchSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), LastCrew = 1002, EmployeeId = "48,67C", TotalHours = 170M, TotalGross = 1001.06M, CulturalHours = 9.92M }
			};

			_repo.PostBatchSize = 3;
			_repo.Save(ranchSummaries);
		}

		private void Print(IEnumerable<RanchSummary> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Ranch Summaries:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(RanchSummary line)
		{
			Console.WriteLine($"---");
			var properties = typeof(RanchSummary).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
