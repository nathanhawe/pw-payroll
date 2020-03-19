using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Linq;

namespace Payroll.UnitTest
{
	[TestClass]
	public class CrewBossPayServiceTests
	{
		[TestMethod]
		public void RateSelectionTests()
		{
			var dbName = "RateSelectionTests";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest(dbName);

			using var context = new PayrollContext(options);
			var cbWageSelector = new CrewBossWageSelector(context);
			var cbService = new CrewBossPayService(context, cbWageSelector);

			cbService.CalculateCrewBossPay(1);

			var hourlyTreeCB = context.CrewBossPayLines.Where(x => x.Crew == 1).FirstOrDefault();
			var hourlyVineCB = context.CrewBossPayLines.Where(x => x.Crew == 2).FirstOrDefault();
			var hourlySouthCB = context.CrewBossPayLines.Where(x => x.Crew == 3).FirstOrDefault();
			var dailySouthCB = context.CrewBossPayLines.Where(x => x.Crew == 4).FirstOrDefault();

			Assert.AreEqual(1, context.CrewBossPayLines.Where(x => x.Crew == 1).Count());
			Assert.AreEqual(1, context.CrewBossPayLines.Where(x => x.Crew == 2).Count());
			Assert.AreEqual(1, context.CrewBossPayLines.Where(x => x.Crew == 3).Count());
			Assert.AreEqual(1, context.CrewBossPayLines.Where(x => x.Crew == 4).Count());

			Assert.AreEqual(30, hourlyTreeCB.WorkerCount);
			Assert.AreEqual(30, hourlyVineCB.WorkerCount);
			Assert.AreEqual(30, hourlySouthCB.WorkerCount);
			Assert.AreEqual(30, dailySouthCB.WorkerCount);

			Assert.AreEqual(21.5M, hourlyTreeCB.HourlyRate);
			Assert.AreEqual(21.5M, hourlyVineCB.HourlyRate);
			Assert.AreEqual(17.9M, hourlySouthCB.HourlyRate);
			Assert.AreEqual(0, dailySouthCB.HourlyRate);

			Assert.AreEqual(215M, hourlyTreeCB.Gross);
			Assert.AreEqual(215M, hourlyVineCB.Gross);
			Assert.AreEqual(179M, hourlySouthCB.Gross);
			Assert.AreEqual(170.05M, dailySouthCB.Gross);

		}

		private void MockCBTest(string dbName)
		{
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock crew boss pay lines
			var hourlyTreeCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 1, hoursWorked: 10, employeeId: "TestHourlyTrees", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyTrees);
			var hourlyVineCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 2, hoursWorked: 10, employeeId: "TestHourlyVines", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyVines);
			var hourlySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 3, hoursWorked: 10, employeeId: "TestSouthHourly", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthHourly);
			var dailySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 4, hoursWorked: 10, employeeId: "TestSouthDaily", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthDaily);
			context.AddRange(hourlyTreeCB, hourlyVineCB, hourlySouthCB, dailySouthCB);

			// Mock ranch pay lines
			for (int i = 0; i < 30; i++)
			{
				context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: hourlyTreeCB.WeekEndDate, shiftDate: hourlyTreeCB.ShiftDate, crew: hourlyTreeCB.Crew, employeeId: $"Crew{hourlyTreeCB.Crew.ToString()}#{i.ToString()}"));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: hourlyVineCB.WeekEndDate, shiftDate: hourlyVineCB.ShiftDate, crew: hourlyVineCB.Crew, employeeId: $"Crew{hourlyVineCB.Crew.ToString()}#{i.ToString()}"));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: hourlySouthCB.WeekEndDate, shiftDate: hourlySouthCB.ShiftDate, crew: hourlySouthCB.Crew, employeeId: $"Crew{hourlySouthCB.Crew.ToString()}#{i.ToString()}"));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: dailySouthCB.WeekEndDate, shiftDate: dailySouthCB.ShiftDate, crew: dailySouthCB.Crew, employeeId: $"Crew{dailySouthCB.Crew.ToString()}#{i.ToString()}"));
			}

			context.SaveChanges();
		}
	}
}
