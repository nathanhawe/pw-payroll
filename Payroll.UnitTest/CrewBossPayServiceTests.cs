using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Linq;

namespace Payroll.UnitTest
{
	[TestClass]
	public class CrewBossPayServiceTests
	{
		private RoundingService _roundingService = new RoundingService();

		[TestMethod]
		public void UpdatesCrewBossPayLines_Before20210208()
		{
			var dbName = "UpdatesCrewBossPayLines_Before20210208";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest_Before20210208(dbName);

			using var context = new PayrollContext(options);
			var crewBossWageService = new CrewBossWageService(context);
			var southCrewBossWageService = new SouthCrewBossWageService(context);
			var cbService = new CrewBossPayService(context, crewBossWageService, southCrewBossWageService, _roundingService);

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

		[TestMethod]
		public void CreatesRanchPayLines_Before20210208()
		{
			var dbName = "CreatesRanchPayLines";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest_Before20210208(dbName);

			using var context = new PayrollContext(options);
			var crewBossWageService = new CrewBossWageService(context);
			var southCrewBossWageService = new SouthCrewBossWageService(context);
			var cbService = new CrewBossPayService(context, crewBossWageService, southCrewBossWageService, _roundingService);

			var ranchPayLines = cbService.CalculateCrewBossPay(1);

			Assert.AreEqual(4, ranchPayLines.Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyTrees" && x.ShiftDate == new DateTime(2020, 2, 10) && x.Crew == 1 && x.HoursWorked == 10 && x.HourlyRate == 21.5M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 215 && x.PayType == PayType.CBHourlyTrees && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyVines" && x.ShiftDate == new DateTime(2020, 2, 10) && x.Crew == 2 && x.HoursWorked == 10 && x.HourlyRate == 21.5M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 215 && x.PayType == PayType.CBHourlyVines && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthHourly" && x.ShiftDate == new DateTime(2020, 2, 10) && x.Crew == 3 && x.HoursWorked == 10 && x.HourlyRate == 17.9M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 179 && x.PayType == PayType.CBSouthHourly && x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthDaily" && x.ShiftDate == new DateTime(2020, 2, 10) && x.Crew == 4 && x.HoursWorked == 10 && x.HourlyRate == 0M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 170.05M && x.PayType == PayType.CBSouthDaily && !x.FiveEight).Count());
		}


		[TestMethod]
		public void UpdatesCrewBossPayLines_OnOrAfter20210208()
		{
			var dbName = "UpdatesCrewBossPayLines_OnOrAfter20210208";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest_OnOrAfter20210208(dbName);

			using var context = new PayrollContext(options);
			var crewBossWageService = new CrewBossWageService(context);
			var southCrewBossWageService = new SouthCrewBossWageService(context);
			var cbService = new CrewBossPayService(context, crewBossWageService, southCrewBossWageService, _roundingService);

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

			// South Daily and Hourly now both become hourly and paid based on worker count
			Assert.AreEqual(21.5M, hourlyTreeCB.HourlyRate);
			Assert.AreEqual(21.5M, hourlyVineCB.HourlyRate);
			Assert.AreEqual(24.75M, hourlySouthCB.HourlyRate);
			Assert.AreEqual(24.75M, dailySouthCB.HourlyRate);

			Assert.AreEqual(215M, hourlyTreeCB.Gross);
			Assert.AreEqual(215M, hourlyVineCB.Gross);
			Assert.AreEqual(247.5M, hourlySouthCB.Gross);
			Assert.AreEqual(247.5M, dailySouthCB.Gross);

		}

		[TestMethod]
		public void CreatesRanchPayLines_OnOrAfter20210208()
		{
			var dbName = "CreatesRanchPayLines_OnOrAfter20210208";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest_OnOrAfter20210208(dbName);

			using var context = new PayrollContext(options);
			var crewBossWageService = new CrewBossWageService(context);
			var southCrewBossWageService = new SouthCrewBossWageService(context);
			var cbService = new CrewBossPayService(context, crewBossWageService, southCrewBossWageService, _roundingService);

			var ranchPayLines = cbService.CalculateCrewBossPay(1);

			// As of 2/8/2021 South Houry and Daily both produce an hourly rate based on the crew count.
			Assert.AreEqual(4, ranchPayLines.Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyTrees" && x.ShiftDate == new DateTime(2021, 2, 8) && x.Crew == 1 && x.HoursWorked == 10 && x.HourlyRate == 21.5M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 215 && x.PayType == PayType.CBHourlyTrees && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyVines" && x.ShiftDate == new DateTime(2021, 2, 8) && x.Crew == 2 && x.HoursWorked == 10 && x.HourlyRate == 21.5M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 215 && x.PayType == PayType.CBHourlyVines && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthHourly" && x.ShiftDate == new DateTime(2021, 2, 8) && x.Crew == 3 && x.HoursWorked == 10 && x.HourlyRate == 24.75M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 247.5M && x.PayType == PayType.CBSouthHourly && x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthDaily" && x.ShiftDate == new DateTime(2021, 2, 8) && x.Crew == 4 && x.HoursWorked == 10 && x.HourlyRate == 24.75M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 247.5M && x.PayType == PayType.CBSouthDaily && !x.FiveEight).Count());
		}

		[TestMethod]
		public void UpdatesCrewBossPayLines_OnOrAfter20210531()
		{
			var dbName = "UpdatesCrewBossPayLines_OnOrAfter20210531";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest_OnOrAfter20210531(dbName);

			using var context = new PayrollContext(options);
			var crewBossWageService = new CrewBossWageService(context);
			var southCrewBossWageService = new SouthCrewBossWageService(context);
			var cbService = new CrewBossPayService(context, crewBossWageService, southCrewBossWageService, _roundingService);

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

			// South Daily and Hourly now both become hourly and paid based on worker count
			Assert.AreEqual(21.5M, hourlyTreeCB.HourlyRate);
			Assert.AreEqual(21.5M, hourlyVineCB.HourlyRate);
			Assert.AreEqual(24.75M, hourlySouthCB.HourlyRate);
			Assert.AreEqual(24.75M, dailySouthCB.HourlyRate);

			Assert.AreEqual(118.25M, hourlyTreeCB.Gross);
			Assert.AreEqual(118.25M, hourlyVineCB.Gross);
			Assert.AreEqual(136.13M, hourlySouthCB.Gross);
			Assert.AreEqual(136.13M, dailySouthCB.Gross);
		}

		[TestMethod]
		public void CreatesRanchPayLines_OnOrAfter20210531()
		{
			var dbName = "CreatesRanchPayLines_OnOrAfter20210531";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			MockCBTest_OnOrAfter20210531(dbName);

			using var context = new PayrollContext(options);
			var crewBossWageService = new CrewBossWageService(context);
			var southCrewBossWageService = new SouthCrewBossWageService(context);
			var cbService = new CrewBossPayService(context, crewBossWageService, southCrewBossWageService, _roundingService);

			var ranchPayLines = cbService.CalculateCrewBossPay(1);

			// As of 5/31/2021 heat-related supplement records are calculated like normal but have no
			// hours worked and use 8.7 pay type.
			Assert.AreEqual(8, ranchPayLines.Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyTrees" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 1 && x.HoursWorked == 5.5M && x.HourlyRate == 21.5M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 118.25M && x.PayType == PayType.CBHourlyTrees && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyVines" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 2 && x.HoursWorked == 5.5M && x.HourlyRate == 21.5M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 118.25M && x.PayType == PayType.CBHourlyVines && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthHourly" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 3 && x.HoursWorked == 5.5M && x.HourlyRate == 24.75M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 136.13M && x.PayType == PayType.CBSouthHourly && x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthDaily" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 4 && x.HoursWorked == 5.5M && x.HourlyRate == 24.75M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 136.13M && x.PayType == PayType.CBSouthDaily && !x.FiveEight).Count());

			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyTrees" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 1 && x.HoursWorked == 0M && x.HourlyRate == 0M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 96.75M && x.PayType == PayType.CBHeatRelatedSupplement && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestHourlyVines" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 2 && x.HoursWorked == 0M && x.HourlyRate == 0M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 96.75M && x.PayType == PayType.CBHeatRelatedSupplement && !x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthHourly" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 3 && x.HoursWorked == 0M && x.HourlyRate == 0M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 111.38M && x.PayType == PayType.CBHeatRelatedSupplement && x.FiveEight).Count());
			Assert.AreEqual(1, ranchPayLines.Where(x => x.BatchId == 1 && x.EmployeeId == "TestSouthDaily" && x.ShiftDate == new DateTime(2021, 5, 31) && x.Crew == 4 && x.HoursWorked == 0M && x.HourlyRate == 0M && x.GrossFromHours == 0 && x.GrossFromPieces == 0 && x.OtherGross == 111.38M && x.PayType == PayType.CBHeatRelatedSupplement && !x.FiveEight).Count());
		}

		private void MockCBTest_Before20210208(string dbName)
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
			var hourlySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 3, hoursWorked: 10, employeeId: "TestSouthHourly", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthHourly, fiveEight: true);
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

		private void MockCBTest_OnOrAfter20210208(string dbName)
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
			var hourlyTreeCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 2, 14), shiftDate: new DateTime(2021, 2, 8), crew: 1, hoursWorked: 10, employeeId: "TestHourlyTrees", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyTrees);
			var hourlyVineCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 2, 14), shiftDate: new DateTime(2021, 2, 8), crew: 2, hoursWorked: 10, employeeId: "TestHourlyVines", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyVines);
			var hourlySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 2, 14), shiftDate: new DateTime(2021, 2, 8), crew: 3, hoursWorked: 10, employeeId: "TestSouthHourly", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthHourly, fiveEight: true);
			var dailySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 2, 14), shiftDate: new DateTime(2021, 2, 8), crew: 4, hoursWorked: 10, employeeId: "TestSouthDaily", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthDaily);
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

		private void MockCBTest_OnOrAfter20210531(string dbName)
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
			var heatThreshold = 10M;
			var hourlyTreeCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 6, 6), shiftDate: new DateTime(2021, 5, 31), crew: 1, hoursWorked: 5.5M, employeeId: "TestHourlyTrees", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyTrees, heatRelatedSupplement: true, heatRelatedSupplementTotalHoursCap: heatThreshold);
			var hourlyVineCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 6, 6), shiftDate: new DateTime(2021, 5, 31), crew: 2, hoursWorked: 5.5M, employeeId: "TestHourlyVines", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyVines, heatRelatedSupplement: true, heatRelatedSupplementTotalHoursCap: heatThreshold);
			var hourlySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 6, 6), shiftDate: new DateTime(2021, 5, 31), crew: 3, hoursWorked: 5.5M, employeeId: "TestSouthHourly", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthHourly, fiveEight: true, heatRelatedSupplement: true, heatRelatedSupplementTotalHoursCap: heatThreshold);
			var dailySouthCB = EntityMocker.MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2021, 6, 6), shiftDate: new DateTime(2021, 5, 31), crew: 4, hoursWorked: 5.5M, employeeId: "TestSouthDaily", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthDaily, heatRelatedSupplement: true, heatRelatedSupplementTotalHoursCap: heatThreshold);
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
