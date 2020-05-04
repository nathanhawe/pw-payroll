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
	public class CrewBossWageSelectorTests
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

			var cbWageSelector = new CrewBossWageSelector(context);

			var tests = new List<WorkerCountTest>() {
				new WorkerCountTest { WorkerCountThreshold = 0, ExpectedWage = 16M },
				new WorkerCountTest { WorkerCountThreshold = 15, ExpectedWage = 16.25M },
				new WorkerCountTest { WorkerCountThreshold = 16, ExpectedWage = 16.5M },
				new WorkerCountTest { WorkerCountThreshold = 17, ExpectedWage = 16.75M },
				new WorkerCountTest { WorkerCountThreshold = 18, ExpectedWage = 17M },
				new WorkerCountTest { WorkerCountThreshold = 19, ExpectedWage = 17.25M },
				new WorkerCountTest { WorkerCountThreshold = 20, ExpectedWage = 17.5M },
				new WorkerCountTest { WorkerCountThreshold = 21, ExpectedWage = 17.75M },
				new WorkerCountTest { WorkerCountThreshold = 22, ExpectedWage = 18M },
				new WorkerCountTest { WorkerCountThreshold = 23, ExpectedWage = 18.25M },
				new WorkerCountTest { WorkerCountThreshold = 24, ExpectedWage = 18.5M },
				new WorkerCountTest { WorkerCountThreshold = 25, ExpectedWage = 19M },
				new WorkerCountTest { WorkerCountThreshold = 26, ExpectedWage = 19.5M },
				new WorkerCountTest { WorkerCountThreshold = 27, ExpectedWage = 20M },
				new WorkerCountTest { WorkerCountThreshold = 28, ExpectedWage = 20.5M },
				new WorkerCountTest { WorkerCountThreshold = 29, ExpectedWage = 21M },
				new WorkerCountTest { WorkerCountThreshold = 30, ExpectedWage = 21.5M },
				new WorkerCountTest { WorkerCountThreshold = 31, ExpectedWage = 22M },
				new WorkerCountTest { WorkerCountThreshold = 32, ExpectedWage = 22.5M },
				new WorkerCountTest { WorkerCountThreshold = 33, ExpectedWage = 23M },
				new WorkerCountTest { WorkerCountThreshold = 34, ExpectedWage = 23.5M },
				new WorkerCountTest { WorkerCountThreshold = 35, ExpectedWage = 24M },
				new WorkerCountTest { WorkerCountThreshold = 36, ExpectedWage = 24.5M },
				new WorkerCountTest { WorkerCountThreshold = 37, ExpectedWage = 24.5M },
				new WorkerCountTest { WorkerCountThreshold = 38, ExpectedWage = 24.5M },
				new WorkerCountTest { WorkerCountThreshold = 39, ExpectedWage = 24.5M },
				new WorkerCountTest { WorkerCountThreshold = 40, ExpectedWage = 24.5M },
				new WorkerCountTest { WorkerCountThreshold = 100, ExpectedWage = 24.5M }

			};

			foreach (var test in tests)
			{
				Assert.AreEqual(test.ExpectedWage, cbWageSelector.GetWage(DateTime.Now, test.WorkerCountThreshold));
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
			context.Add(new CrewBossWage { Wage = 10.10M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 3) });
			context.Add(new CrewBossWage { Wage = 20.20M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 10) });
			context.Add(new CrewBossWage { Wage = 30.30M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 17) });
			context.SaveChanges();

			if (context.CrewBossWages.Count() != 3) Assert.Inconclusive("Count of CrewBossWages is unexpected.");

			var cbWageSelector = new CrewBossWageSelector(context);

			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 3), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 4), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 5), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 6), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 7), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 8), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 9), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 10), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 11), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 12), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 13), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 14), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 15), 0));
			Assert.AreEqual(20.20M, cbWageSelector.GetWage(new DateTime(2020, 2, 16), 0));
			Assert.AreEqual(30.30M, cbWageSelector.GetWage(new DateTime(2020, 2, 17), 0));
			Assert.AreEqual(30.30M, cbWageSelector.GetWage(new DateTime(2020, 2, 18), 0));
			Assert.AreEqual(30.30M, cbWageSelector.GetWage(new DateTime(2020, 2, 19), 0));
			Assert.AreEqual(30.30M, cbWageSelector.GetWage(new DateTime(2020, 2, 20), 0));
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
			context.Add(new CrewBossWage { Wage = 10.10M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 3) });
			context.Add(new CrewBossWage { Wage = 20.20M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 10), IsDeleted = true });
			context.Add(new CrewBossWage { Wage = 30.30M, WorkerCountThreshold = 0, EffectiveDate = new DateTime(2020, 2, 17), IsDeleted = true });
			context.SaveChanges();

			if (context.CrewBossWages.Count() != 3) Assert.Inconclusive("Count of CrewBossWages is unexpected.");

			var cbWageSelector = new CrewBossWageSelector(context);

			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 3), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 4), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 5), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 6), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 7), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 8), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 9), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 10), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 11), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 12), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 13), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 14), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 15), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 16), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 17), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 18), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 19), 0));
			Assert.AreEqual(10.10M, cbWageSelector.GetWage(new DateTime(2020, 2, 20), 0));
		}
	}
}
