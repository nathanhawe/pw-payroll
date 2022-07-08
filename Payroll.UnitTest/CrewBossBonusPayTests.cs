using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain.Constants;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class CrewBossBonusPayTests
	{
		private RoundingService _roundingService = new RoundingService();

		[TestMethod]
		public void NoBonusWhenNoPieceRates()
		{
			var dbName = "NoBonusWhenNoPieceRates";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup
			SetupPaylinesContext(context, 1, new DateTime(2022, 4, 17));
			var originalCount = context.RanchPayLines.Count();

			// Execute
			var service = new CrewBossBonusPayService(context, _roundingService);
			var results = service.CalculateCrewBossBonusPayLines(1, new DateTime(2022, 4, 17));

			// Validate
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());
			Assert.AreEqual(0, results.Count());

		}

		[TestMethod]
		public void NoBonusWhenWrongLaborCode()
		{
			var dbName = "NoBonusWhenWrongLaborCode";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup
			SetupPaylinesContext(context, 1, new DateTime(2022, 4, 17));
			var originalCount = context.RanchPayLines.Count();
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.Girdling, effectiveDate: new DateTime(2022, 1, 1), perTreeBonus: 0.08M));
			context.SaveChanges();

			// Execute
			var service = new CrewBossBonusPayService(context, _roundingService);
			var results = service.CalculateCrewBossBonusPayLines(1, new DateTime(2022, 4, 17));

			// Validate
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());
			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void NoBonusWhenBeforeEffectiveDate()
		{
			var dbName = "NoBonusWhenBeforeEffectiveDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup
			SetupPaylinesContext(context, 1, new DateTime(2022, 4, 17));
			var originalCount = context.RanchPayLines.Count();
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: new DateTime(2022, 4, 18), perTreeBonus: 0.08M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateThinning, effectiveDate: new DateTime(2022, 4, 18), perTreeBonus: 0.10M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: new DateTime(2022, 4, 18), perTreeBonus: 0.11M));
			context.SaveChanges();

			// Execute
			var service = new CrewBossBonusPayService(context, _roundingService);
			var results = service.CalculateCrewBossBonusPayLines(1, new DateTime(2022, 4, 17));

			// Validate
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());
			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void NoBonusRecordWhenZeroDollar()
		{
			var dbName = "NoBonusRecordWhenZeroDollar";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup
			SetupPaylinesContext(context, 1, new DateTime(2022, 4, 17));
			var originalCount = context.RanchPayLines.Count();
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: new DateTime(2022, 4, 11), perTreeBonus: 0));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateThinning, effectiveDate: new DateTime(2022, 4, 11), perTreeBonus: 0));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: new DateTime(2022, 4, 11), perTreeBonus: 0));
			context.SaveChanges();

			// Execute
			var service = new CrewBossBonusPayService(context, _roundingService);
			var results = service.CalculateCrewBossBonusPayLines(1, new DateTime(2022, 4, 17));

			// Validate
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());
			Assert.AreEqual(0, results.Count());
		}

		[TestMethod]
		public void CorrectBonusWhenActivePieceRates()
		{
			var dbName = "CorrectBonusWhenActivePieceRates";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var weekEndDate = new DateTime(2022, 4, 17);

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup
			SetupPaylinesContext(context, 1, new DateTime(2022, 4, 17));
			var originalCount = context.RanchPayLines.Count();
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: new DateTime(2022, 3, 28), perTreeBonus: 0.08M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateHarvest_Bucket, effectiveDate: new DateTime(2022, 4, 15), perTreeBonus: 0.09M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateThinning, effectiveDate: new DateTime(2022, 4, 11), perTreeBonus: 0.10M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRateThinning, effectiveDate: new DateTime(2022, 4, 15), perTreeBonus: 0.11M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: new DateTime(2022, 4, 11), perTreeBonus: 0.12M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.PieceRatePruningWinter, effectiveDate: new DateTime(2022, 4, 15), perTreeBonus: 0.13M));
			context.Add(EntityMocker.MockCrewBossBonusPieceRate(laborCode: (int)RanchLaborCode.Thinning, effectiveDate: new DateTime(2022, 4, 15), perTreeBonus: 0M));
			context.SaveChanges();

			// Execute
			var service = new CrewBossBonusPayService(context, _roundingService);
			var results = service.CalculateCrewBossBonusPayLines(1, new DateTime(2022, 4, 17));

			// Validate
			
			Assert.AreEqual(originalCount, context.RanchPayLines.Count());


			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 11) && x.BlockId == 0 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 70 && x.PieceRate == .08M && x.GrossFromPieces == 5.6M && x.TotalGross == 5.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 12) && x.BlockId == 1 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 70 && x.PieceRate == .1M && x.GrossFromPieces == 7M && x.TotalGross == 7M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 13) && x.BlockId == 2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 70 && x.PieceRate == .12M && x.GrossFromPieces == 8.4M && x.TotalGross == 8.4M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 14) && x.BlockId == 3 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 70 && x.PieceRate == .08M && x.GrossFromPieces == 5.6M && x.TotalGross == 5.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 15) && x.BlockId == 4 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 70 && x.PieceRate == .11M && x.GrossFromPieces == 7.7M && x.TotalGross == 7.7M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 16) && x.BlockId == 5 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 70 && x.PieceRate == .13M && x.GrossFromPieces == 9.1M && x.TotalGross == 9.1M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB100" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 17) && x.BlockId == 6 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 70 && x.PieceRate == .09M && x.GrossFromPieces == 6.3M && x.TotalGross == 6.3M).Count());

			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 11) && x.BlockId == 0 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 70 && x.PieceRate == .08M && x.GrossFromPieces == 5.6M && x.TotalGross == 5.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 12) && x.BlockId == 1 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 70 && x.PieceRate == .1M && x.GrossFromPieces == 7M && x.TotalGross == 7M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 13) && x.BlockId == 2 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 70 && x.PieceRate == .12M && x.GrossFromPieces == 8.4M && x.TotalGross == 8.4M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 14) && x.BlockId == 3 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 70 && x.PieceRate == .08M && x.GrossFromPieces == 5.6M && x.TotalGross == 5.6M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 15) && x.BlockId == 4 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 70 && x.PieceRate == .11M && x.GrossFromPieces == 7.7M && x.TotalGross == 7.7M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 16) && x.BlockId == 5 && x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter && x.Pieces == 70 && x.PieceRate == .13M && x.GrossFromPieces == 9.1M && x.TotalGross == 9.1M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB101" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 17) && x.BlockId == 6 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 70 && x.PieceRate == .09M && x.GrossFromPieces == 6.3M && x.TotalGross == 6.3M).Count());

			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 11) && x.BlockId == 0 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 34 && x.PieceRate == .08M && x.GrossFromPieces == 2.72M && x.TotalGross == 2.72M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 11) && x.BlockId == 7 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 36 && x.PieceRate == .08M && x.GrossFromPieces == 2.88M && x.TotalGross == 2.88M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 12) && x.BlockId == 1 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 34 && x.PieceRate == .1M && x.GrossFromPieces == 3.4M && x.TotalGross == 3.4M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 12) && x.BlockId == 8 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 36 && x.PieceRate == .1M && x.GrossFromPieces == 3.6M && x.TotalGross == 3.6M).Count());
			Assert.AreEqual(0, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 13)).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 14) && x.BlockId == 3 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 34 && x.PieceRate == .08M && x.GrossFromPieces == 2.72M && x.TotalGross == 2.72M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 14) && x.BlockId == 10 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 36 && x.PieceRate == .08M && x.GrossFromPieces == 2.88M && x.TotalGross == 2.88M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 15) && x.BlockId == 4 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 34 && x.PieceRate == .11M && x.GrossFromPieces == 3.74M && x.TotalGross == 3.74M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 15) && x.BlockId == 11 && x.LaborCode == (int)RanchLaborCode.PieceRateThinning && x.Pieces == 36 && x.PieceRate == .11M && x.GrossFromPieces == 3.96M && x.TotalGross == 3.96M).Count());
			Assert.AreEqual(0, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 16)).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 17) && x.BlockId == 6 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 34 && x.PieceRate == .09M && x.GrossFromPieces == 3.06M && x.TotalGross == 3.06M).Count());
			Assert.AreEqual(1, results.Where(x => x.PayType == PayType.ProductionIncentiveBonus && x.EmployeeId == "CB102" && x.WeekEndDate == weekEndDate && x.ShiftDate == new DateTime(2022, 4, 17) && x.BlockId == 13 && x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket && x.Pieces == 36 && x.PieceRate == .09M && x.GrossFromPieces == 3.24M && x.TotalGross == 3.24M).Count());

			Assert.AreEqual(0, results.Where(x => x.Crew == 103).Count()); // No Crew
			Assert.AreEqual(0, results.Where(x => x.Crew == 104).Count()); // Crew boss disqualified from quality bonus

			Assert.AreEqual(24, results.Count());
		}
	


		public void SetupPaylinesContext(PayrollContext context, int batchId, DateTime weekEndDate)
		{
			var dates = GetWeekDates(weekEndDate);
			int crew, laborCode;
						
			// Crew 100
			crew = 100;
			for(int i = 0; i < dates.Count; i++)
			{
				if(i % 3 == 0)
				{
					laborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket;
				}
				else if(i % 3 == 1)
				{
					laborCode = (int)RanchLaborCode.PieceRateThinning;
				}
				else
				{
					laborCode = (int)RanchLaborCode.PieceRatePruningWinter;
				}
				context.Add(EntityMocker.MockCrewBossPayLine(batchId: batchId, weekEndDate: weekEndDate, shiftDate: dates[i], crew: crew, employeeId: $"CB{crew.ToString()}", payMethod: CrewBossPayMethod.HourlyTrees, hoursWorked: 8));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee0", hoursWorked: 8, pieces: 10, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee1", hoursWorked: 8, pieces: 15, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee2", hoursWorked: 8, pieces: 20, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee3", hoursWorked: 8, pieces: 25, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
			}

			// Crew 101
			crew = 101;
			for (int i = 0; i < dates.Count; i++)
			{
				if (i % 3 == 0)
				{
					laborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket;
				}
				else if (i % 3 == 1)
				{
					laborCode = (int)RanchLaborCode.PieceRateThinning;
				}
				else
				{
					laborCode = (int)RanchLaborCode.PieceRatePruningWinter;
				}
				context.Add(EntityMocker.MockCrewBossPayLine(batchId: batchId, weekEndDate: weekEndDate, shiftDate: dates[i], crew: crew, employeeId: $"CB{crew.ToString()}", payMethod: CrewBossPayMethod.SouthHourly, hoursWorked: 8));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee0", hoursWorked: 8, pieces: 10, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee1", hoursWorked: 8, pieces: 15, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee2", hoursWorked: 8, pieces: 20, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee3", hoursWorked: 8, pieces: 25, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
			}

			// Crew 102 - Split shift
			crew = 102;
			for (int i = 0; i < dates.Count; i++)
			{
				if (i % 3 == 0)
				{
					laborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket;
				}
				else if (i % 3 == 1)
				{
					laborCode = (int)RanchLaborCode.PieceRateThinning;
				}
				else
				{
					laborCode = (int)RanchLaborCode.Thinning;
				}

				context.Add(EntityMocker.MockCrewBossPayLine(batchId: batchId, weekEndDate: weekEndDate, shiftDate: dates[i], crew: crew, employeeId: $"CB{crew.ToString()}", payMethod: CrewBossPayMethod.HourlyTrees, hoursWorked: 4));
				context.Add(EntityMocker.MockCrewBossPayLine(batchId: batchId, weekEndDate: weekEndDate, shiftDate: dates[i], crew: crew, employeeId: $"CB{crew.ToString()}", payMethod: CrewBossPayMethod.HourlyTrees, hoursWorked: 4));

				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee0", hoursWorked: 4, pieces: 5, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee1", hoursWorked: 4, pieces: 7, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee2", hoursWorked: 4, pieces: 10, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee3", hoursWorked: 4, pieces: 12, blockId: i, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee0", hoursWorked: 4, pieces: 5, blockId: i + 7, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee1", hoursWorked: 4, pieces: 8, blockId: i + 7, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee2", hoursWorked: 4, pieces: 10, blockId: i + 7, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee3", hoursWorked: 4, pieces: 13, blockId: i + 7, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
			}

			// Crew 103 - No Crew Boss
			crew = 103;
			for (int i = 0; i < dates.Count; i++)
			{
				if (i % 3 == 0)
				{
					laborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket;
				}
				else if (i % 3 == 1)
				{
					laborCode = (int)RanchLaborCode.PieceRateThinning;
				}
				else
				{
					laborCode = (int)RanchLaborCode.PieceRatePruningWinter;
				}

				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee0", hoursWorked: 8, pieces: 10, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee1", hoursWorked: 8, pieces: 15, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee2", hoursWorked: 8, pieces: 20, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee3", hoursWorked: 8, pieces: 25, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
			}

			// Crew 104 - CB Disqualified
			crew = 104;
			for (int i = 0; i < dates.Count; i++)
			{
				if (i % 3 == 0)
				{
					laborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket;
				}
				else if (i % 3 == 1)
				{
					laborCode = (int)RanchLaborCode.PieceRateThinning;
				}
				else
				{
					laborCode = (int)RanchLaborCode.PieceRatePruningWinter;
				}
				context.Add(EntityMocker.MockCrewBossPayLine(batchId: batchId, weekEndDate: weekEndDate, shiftDate: dates[i], crew: crew, employeeId: $"CB{crew.ToString()}", payMethod: CrewBossPayMethod.SouthHourly, hoursWorked: 4, isDisqualifiedFromQualityBonus: true));
				context.Add(EntityMocker.MockCrewBossPayLine(batchId: batchId, weekEndDate: weekEndDate, shiftDate: dates[i], crew: crew, employeeId: $"CB{crew.ToString()}", payMethod: CrewBossPayMethod.SouthHourly, hoursWorked: 4, isDisqualifiedFromQualityBonus: false));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee0", hoursWorked: 8, pieces: 10, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee1", hoursWorked: 8, pieces: 15, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee2", hoursWorked: 8, pieces: 20, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
				context.Add(EntityMocker.MockRanchPayLine(batchId: batchId, crew: crew, employeeId: "Employee3", hoursWorked: 8, pieces: 25, shiftDate: dates[i], weekEndDate: weekEndDate, laborCode: laborCode));
			}
			context.SaveChanges();
		}

		public List<DateTime> GetWeekDates(DateTime weekEndDate)
		{
			var dates = new List<DateTime>();

			for(int i = -6; i < 0; i++)
			{
				dates.Add(weekEndDate.AddDays(i));
			}
			dates.Add(weekEndDate);
			
			return dates;
		}
	}
}
