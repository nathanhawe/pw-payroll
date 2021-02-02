using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class SouthCrewBossWageServiceTests
	{
		private class WorkerCountTest
		{
			public int WorkerCountThreshold { get; set; }
			public decimal ExpectedWage { get; set; }
		}

		[TestMethod]
		public void SelectsRateByWorkerCount()
		{
			var dbName = "SelectsRateByWorkerCount";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			var southCrewBossWageService = new SouthCrewBossWageService(context);

			var tests = new List<WorkerCountTest>() {
				new WorkerCountTest { WorkerCountThreshold = 0, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 1, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 2, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 3, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 4, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 5, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 6, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 7, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 8, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 9, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 10, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 11, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 12, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 13, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 14, ExpectedWage = 17.9M },
				new WorkerCountTest { WorkerCountThreshold = 15, ExpectedWage = 22.75M },
				new WorkerCountTest { WorkerCountThreshold = 16, ExpectedWage = 23.25M },
				new WorkerCountTest { WorkerCountThreshold = 17, ExpectedWage = 23.75M },
				new WorkerCountTest { WorkerCountThreshold = 18, ExpectedWage = 24.25M },
				new WorkerCountTest { WorkerCountThreshold = 19, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 20, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 21, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 22, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 23, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 24, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 25, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 26, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 27, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 28, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 29, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 30, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 31, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 32, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 33, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 34, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 35, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 36, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 37, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 38, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 39, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 40, ExpectedWage = 24.75M },
				new WorkerCountTest { WorkerCountThreshold = 100, ExpectedWage = 24.75M }

			};

			foreach (var test in tests)
			{
				Assert.AreEqual(0, southCrewBossWageService.GetWage(new DateTime(2021, 2, 7), test.WorkerCountThreshold));
				Assert.AreEqual(test.ExpectedWage, southCrewBossWageService.GetWage(new DateTime(2021, 2, 8), test.WorkerCountThreshold));
			}
		}

		[TestMethod]
		public void SelectsCurrentEffectiveRate()
		{
			var dbName = "SelectsCurrentEffectiveRate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);

			// Mock Wage records
			context.Add(new SouthCrewBossWage { Wage = 30, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2022, 2, 3) });
			context.Add(new SouthCrewBossWage { Wage = 30.25M, WorkerCountThreshold = 12, EffectiveDate = new DateTime(2022, 2, 3) });
			context.Add(new SouthCrewBossWage { Wage = 30.50M, WorkerCountThreshold = 20, EffectiveDate = new DateTime(2022, 2, 3) });
			context.Add(new SouthCrewBossWage { Wage = 31M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2022, 2, 10) });
			context.Add(new SouthCrewBossWage { Wage = 31.25M, WorkerCountThreshold = 12, EffectiveDate = new DateTime(2022, 2, 10) });
			context.Add(new SouthCrewBossWage { Wage = 31.5M, WorkerCountThreshold = 20, EffectiveDate = new DateTime(2022, 2, 10) });
			context.Add(new SouthCrewBossWage { Wage = 32M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2022, 2, 17) });
			context.Add(new SouthCrewBossWage { Wage = 32.25M, WorkerCountThreshold = 12, EffectiveDate = new DateTime(2022, 2, 17) });
			context.Add(new SouthCrewBossWage { Wage = 32.5M, WorkerCountThreshold = 20, EffectiveDate = new DateTime(2022, 2, 17) });
			context.SaveChanges();

			if (context.SouthCrewBossWages.Count() != 9) Assert.Inconclusive("Count of SouthCrewBossWages is unexpected.");

			var southCrewBossWageService = new SouthCrewBossWageService(context);

			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 3), 0));
			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 4), 1));
			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 5), 2));
			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 6), 3));
			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 7), 4));
			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 8), 5));
			Assert.AreEqual(30M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 9), 11));

			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 3), 12));
			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 4), 13));
			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 5), 14));
			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 6), 15));
			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 7), 16));
			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 8), 17));
			Assert.AreEqual(30.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 9), 19));

			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 3), 20));
			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 4), 21));
			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 5), 22));
			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 6), 23));
			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 7), 24));
			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 8), 25));
			Assert.AreEqual(30.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 9), 26));



			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 10), 0));
			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 11), 1));
			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 12), 2));
			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 13), 3));
			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 14), 4));
			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 15), 5));
			Assert.AreEqual(31M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 16), 11));

			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 10), 12));
			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 11), 13));
			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 12), 14));
			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 13), 15));
			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 14), 16));
			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 15), 17));
			Assert.AreEqual(31.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 16), 19));

			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 10), 20));
			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 11), 21));
			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 12), 22));
			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 13), 23));
			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 14), 24));
			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 15), 25));
			Assert.AreEqual(31.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 16), 26));


			Assert.AreEqual(32M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 17), 0));
			Assert.AreEqual(32M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 18), 1));
			Assert.AreEqual(32M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 19), 5));
			Assert.AreEqual(32M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 20), 11));

			Assert.AreEqual(32.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 17), 12));
			Assert.AreEqual(32.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 18), 15));
			Assert.AreEqual(32.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 19), 16));
			Assert.AreEqual(32.25M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 20), 19));

			Assert.AreEqual(32.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 17), 20));
			Assert.AreEqual(32.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 18), 21));
			Assert.AreEqual(32.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 19), 22));
			Assert.AreEqual(32.5M, southCrewBossWageService.GetWage(new DateTime(2022, 2, 20), 23));
		}

		[TestMethod]
		public void IgnoresDeletedRecords()
		{
			var dbName = "IgnoresDeletedRecords";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);

			// Mock Wage records
			context.Add(new SouthCrewBossWage { Wage = 10.10M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 3) });
			context.Add(new SouthCrewBossWage { Wage = 20.20M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 10), IsDeleted = true });
			context.Add(new SouthCrewBossWage { Wage = 30.30M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 17), IsDeleted = true });
			context.SaveChanges();

			if (context.SouthCrewBossWages.Count() != 3) Assert.Inconclusive("Count of SouthCrewBossWages is unexpected.");

			var southCrewBossWageService = new SouthCrewBossWageService(context);

			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 3), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 4), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 5), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 6), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 7), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 8), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 9), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 10), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 11), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 12), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 13), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 14), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 15), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 16), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 17), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 18), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 19), 0));
			Assert.AreEqual(10.10M, southCrewBossWageService.GetWage(new DateTime(2020, 2, 20), 0));
		}
	}
}
