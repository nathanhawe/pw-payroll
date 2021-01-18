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
	public class PlantSummariesRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private PlantSummariesRepo _repo;

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
			_repo = new PlantSummariesRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery()
		{
			var temp = _repo.Get(new DateTime(2020, 4, 12));
			Print(temp);
		}

		[TestMethod]
		[Ignore("This test can create records in Quick Base.")]
		public void ImportFromCSV()
		{
			var PlantSummaries = new List<PlantSummary>
			{
				new PlantSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), EmployeeId = "25,18C", TotalHours = 100M, TotalGross = 1000.99M },
				new PlantSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), EmployeeId = "29,37D", TotalHours = 110M, TotalGross = 1001M },
				new PlantSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), EmployeeId = "48,70C", TotalHours = 120M, TotalGross = 1001.01M },
				new PlantSummary{ LayoffId = 0, WeekEndDate = new DateTime(2019, 1, 6), EmployeeId = "48,67C", TotalHours = 130M, TotalGross = 1001.02M },
				new PlantSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), EmployeeId = "25,18C", TotalHours = 140M, TotalGross = 1001.03M },
				new PlantSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), EmployeeId = "29,37D", TotalHours = 150M, TotalGross = 1001.04M },
				new PlantSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), EmployeeId = "48,70C", TotalHours = 160M, TotalGross = 1001.05M },
				new PlantSummary{ LayoffId = 920, WeekEndDate = new DateTime(2019, 1, 13), EmployeeId = "48,67C", TotalHours = 170M, TotalGross = 1001.06M }
			};

			_repo.PostBatchSize = 3;
			_repo.Save(PlantSummaries);
		}

		private void Print(IEnumerable<PlantSummary> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Plant Summaries:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PlantSummary line)
		{
			Console.WriteLine($"---");
			var properties = typeof(PlantSummary).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
