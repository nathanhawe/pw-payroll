using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Data;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Payroll.Domain.Constants;
using System.Linq;
using Payroll.UnitTest.Mocks;
using Payroll.UnitTest.Helpers;
using Payroll.Domain;

namespace Payroll.UnitTest
{
	[TestClass]
	public class DailySummaryCalculatorTests
	{
		private Decimal _mockCrewLaborWage = 14M;
		private MockMinimumWageService _mockMinimumWageService = new MockMinimumWageService();
		private RoundingService _roundingService = new RoundingService();
		private MockCrewLaborWageService _mockCrewLaborWageService;

		[TestInitialize]
		public void Init()
		{
			_mockCrewLaborWageService = new MockCrewLaborWageService(_mockCrewLaborWage);
		}
		#region Ranch Pay Line Tests

		[TestMethod]
		public void RanchPayLine_Cultural_SelectsMinimumWageForShiftDate()
		{
			var dbName = "RanchPayLine_Cultural_SelectsMinimumWageForShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup mock minimum wages
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), 9M);

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == 8.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == 8.75M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == 9M).Count());
		}

		[TestMethod]
		public void RanchPayLine_Crew_SelectsGreaterOfCrewAndMinimumWageForShiftDate()
		{
			var dbName = "RanchPayLine_Crew_SelectsGreaterOfCrewAndMinimumWageForShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup mock minimum wages
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), _mockCrewLaborWage + 1M);

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 100));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 100));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 200));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3000));
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == _mockCrewLaborWage).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == _mockCrewLaborWage).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == (_mockCrewLaborWage + 1M)).Count());
		}


		[TestMethod]
		public void RanchPayLine_UsesLastOfCrewSortedByRecordId()
		{
			var dbName = "RanchPayLine_UsesLastOfCrewSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, crew: 4));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, crew: 4));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, crew: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, crew: 6));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == 2).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == 3).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == 4).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == 5).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == 6).Count());
		}

		[TestMethod]
		public void RanchPayLine_UsesLastBlockIdSortedByRecordId()
		{
			var dbName = "RanchPayLine_UsesLastBlockIdSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, blockId: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, blockId: 2));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, blockId: 1));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, blockId: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, blockId: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, blockId: 3));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, blockId: 4));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, blockId: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, blockId: 6));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, blockId: 7));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, blockId: 8));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, blockId: 9));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, blockId: 10));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, blockId: 11));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, blockId: 12));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, blockId: 13));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, blockId: 14));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, blockId: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, blockId: 1));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, blockId: 3));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, blockId: 30));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, blockId: 100));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, blockId: 90));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, blockId: 50));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, blockId: 50));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, blockId: 51));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, blockId: 52));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, blockId: 53));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, blockId: 54));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, blockId: 55));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, blockId: 99));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, blockId: 17));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, blockId: 42));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastBlockId == 2).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastBlockId == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastBlockId == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastBlockId == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastBlockId == 3).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastBlockId == 42).Count());
		}

		[TestMethod]
		public void RanchPayLine_UsesLastLaborCodeSortedByRecordId()
		{
			var dbName = "RanchPayLine_UsesLastLaborCodeSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, laborCode: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, laborCode: 2));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, laborCode: 1));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, laborCode: 1));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, laborCode: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, laborCode: 3));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, laborCode: 4));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, laborCode: 5));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, laborCode: 6));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, laborCode: 7));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, laborCode: 8));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, laborCode: 9));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, laborCode: 10));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, laborCode: 11));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, laborCode: 12));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, laborCode: 13));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, laborCode: 14));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, laborCode: 2));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, laborCode: 1));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, laborCode: 3));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, laborCode: 30));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, laborCode: 100));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, laborCode: 90));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, laborCode: 50));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, laborCode: 50));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, laborCode: 51));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, laborCode: 52));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, laborCode: 53));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, laborCode: 54));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, laborCode: 55));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, laborCode: 99));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, laborCode: 17));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, laborCode: 42));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastLaborCode == 2).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastLaborCode == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastLaborCode == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastLaborCode == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastLaborCode == 3).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastLaborCode == 42).Count());
		}

		[TestMethod]
		public void RanchPayLine_UsesLastOfFiveEightSortedByRecordId()
		{
			var dbName = "RanchPayLine_UsesLastOfFiveEightSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, fiveEight: true));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, fiveEight: true));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, fiveEight: true));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, fiveEight: true));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, fiveEight: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, fiveEight: true));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, fiveEight: false));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.FiveEight == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.FiveEight == true).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.FiveEight == true).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.FiveEight == true).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.FiveEight == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.FiveEight == false).Count());
		}

		[TestMethod]
		public void RanchPayLine_GroupsByEmployee()
		{
			var dbName = "RanchPayLine_GroupsByEmployee";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2").Count());
		}

		[TestMethod]
		public void RanchPayLine_GroupsByWeekEnding()
		{
			var dbName = "RanchPayLine_GroupsByWeekEnding";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
		}

		[TestMethod]
		public void RanchPayLine_GroupsByShiftDate()
		{
			var dbName = "RanchPayLine_GroupsByShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(4, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
		}

		[TestMethod]
		public void RanchPayLine_GroupsByAlternativeWorkWeek()
		{
			var dbName = "RanchPayLine_GroupsByAlternativeWorkWeek";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: true));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == true).Count());
		}

		[TestMethod]
		public void RanchPayLine_Regular()
		{
			var dbName = "RanchPayLine_Regular";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: 50M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: 50M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, otherGross: 0, totalGross: 110M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: 10M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: 2.5M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: 2.5M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: 2.5M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: 2.5M, payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: 75M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: 75M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, otherGross: 0, totalGross: 165M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: 15M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: 4.95M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: 4.95M, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, otherGross: 0, totalGross: 5.1M, payType: PayType.Regular));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
		}

		[TestMethod]
		public void RanchPayLine_Pieces()
		{
			var dbName = "RanchPayLine_Pieces";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0+352+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M+140+0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0+352+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M+140+0), payType: PayType.Pieces));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void RanchPayLine_ProductionIncentiveBonus()
		{
			var dbName = "RanchPayLine_ProductionIncentiveBonus";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var payType = PayType.ProductionIncentiveBonus;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0 + 352 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M + 0 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M + 140 + 0), payType: payType));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0 + 352 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M + 0 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M + 140 + 0), payType: payType));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void RanchPayLine_RegularNonProductiveTime()
		{
			var dbName = "RanchPayLine_RegularNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, otherGross: 0, totalGross: (45+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, otherGross: 0, totalGross: (107.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, otherGross: 0, totalGross: (1.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void RanchPayLine_PiecesNonProductiveTime()
		{
			var dbName = "RanchPayLine_PiecesNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9.5M, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0+352+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.08M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0M, grossFromPieces: 0, otherGross: 0, totalGross: (0M+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M+0+0), payType: PayType.Regular));


			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 37.05M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 27.70M).Count());
		}

		[TestMethod]
		public void RanchPayLine_IgnoresPremiumPay()
		{
			var dbName = "RanchPayLine_IgnoresPremiumPay";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, otherGross: 0, totalGross: (45 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0 + 0 + 0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, otherGross: 0, totalGross: (107.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0 + 0 + 0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, otherGross: 0, totalGross: (1.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0 + 0 + 0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 20M, grossFromHours: 100, grossFromPieces: 100, otherGross: 100, totalGross: (100 + 100 + 100), payType: PayType.PremiumPay));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void RanchPayLine_CrewBoss()
		{
			var dbName = "RanchPayLine_CrewBoss";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 265, totalGross: 265, payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 291.5M, totalGross: (0+0+291.5M), payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 4, grossFromHours: 0, grossFromPieces: 0, otherGross: 82, totalGross: (0+0+82), payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6, grossFromHours: 0, grossFromPieces: 0, otherGross: 300, totalGross: (0+0+300), payType: PayType.CBCommission));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 265 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 291.5M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 382 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 38.2M).Count());
		}

		#endregion

		#region Ranch Adjustment Line Tests

		[TestMethod]
		public void RanchAdjustmentLine_Cultural_SelectsMinimumWageForShiftDate()
		{
			var dbName = "RanchAdjustmentLine_Cultural_SelectsMinimumWageForShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup mock minimum wages
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), 9M);

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == 8.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == 8.75M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == 9M).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_Crew_SelectsGreaterOfCrewAndMinimumWageForShiftDate()
		{
			var dbName = "RanchAdjustmentLine_Crew_SelectsGreaterOfCrewAndMinimumWageForShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup mock minimum wages
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), _mockCrewLaborWage + 1M);

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 100));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 100));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 200));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3000));
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == _mockCrewLaborWage).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == _mockCrewLaborWage).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == (_mockCrewLaborWage + 1M)).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_UsesLastOfCrewSortedByRecordId()
		{
			var dbName = "RanchAdjustmentLine_UsesLastOfCrewSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, crew: 4));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, crew: 4));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, crew: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, crew: 6));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == 2).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == 3).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == 4).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == 5).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == 6).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_UsesLastOfFiveEightSortedByRecordId()
		{
			var dbName = "RanchAdjustmentLine_UsesLastOfFiveEightSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, fiveEight: true));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, fiveEight: true));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, fiveEight: true));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, fiveEight: true));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, fiveEight: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, fiveEight: true));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, fiveEight: false));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.FiveEight == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.FiveEight == true).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.FiveEight == true).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.FiveEight == true).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.FiveEight == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.FiveEight == false).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_UsesLastOfBlockIdSortedByRecordId()
		{
			var dbName = "RanchAdjustmentLine_UsesLastOfBlockIdSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, blockId: 0));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, blockId: 1));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, blockId: 2));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, blockId: 3));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, blockId: 4));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, blockId: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, blockId: 6));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, blockId: 7));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, blockId: 8));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, blockId: 120));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, blockId: 9));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, blockId: 11));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, blockId: 14));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, blockId: 13));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, blockId: 22));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, blockId: 42));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, blockId: 12));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, blockId: 1000));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, blockId: 99));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, blockId: 22));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, blockId: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, blockId: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, blockId: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, blockId: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, blockId: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, blockId: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, blockId: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, blockId: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, blockId: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, blockId: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, blockId: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, blockId: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, blockId: 42));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastBlockId == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastBlockId == 2).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastBlockId == 12).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastBlockId == 99).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastBlockId == 22).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastBlockId == 42).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_UsesLastOfLaborCodeSortedByRecordId()
		{
			var dbName = "RanchAdjustmentLine_UsesLastOfLaborCodeSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, laborCode: 0));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, laborCode: 1));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, laborCode: 2));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, laborCode: 3));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, laborCode: 4));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, laborCode: 5));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, laborCode: 6));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, laborCode: 7));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, laborCode: 8));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, laborCode: 120));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, laborCode: 9));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, laborCode: 11));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, laborCode: 14));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, laborCode: 13));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, laborCode: 22));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, laborCode: 42));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, laborCode: 12));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, laborCode: 1000));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, laborCode: 99));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, laborCode: 22));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, laborCode: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, laborCode: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, laborCode: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, laborCode: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, laborCode: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, laborCode: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, laborCode: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, laborCode: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, laborCode: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, laborCode: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, laborCode: 99));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, laborCode: 98));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, laborCode: 42));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastLaborCode == 1).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastLaborCode == 2).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastLaborCode == 12).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.LastLaborCode == 99).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.LastLaborCode == 22).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.LastLaborCode == 42).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_GroupsByEmployee()
		{
			var dbName = "RanchAdjustmentLine_GroupsByEmployee";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2").Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_GroupsByWeekEnding()
		{
			var dbName = "RanchAdjustmentLine_GroupsByWeekEnding";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_GroupsByShiftDate()
		{
			var dbName = "RanchAdjustmentLine_GroupsByShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(4, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_GroupsByAlternativeWorkWeek()
		{
			var dbName = "RanchAdjustmentLine_GroupsByAlternativeWorkWeek";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: true));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == true).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_Regular()
		{
			var dbName = "RanchAdjustmentLine_Regular";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, otherGross: 0, totalGross: (110+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: (75+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: (75+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, otherGross: 0, totalGross: (165+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: (4.95M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: (4.95M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, otherGross: 0, totalGross: (5.1M+0+0), payType: PayType.Regular));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_Pieces()
		{
			var dbName = "RanchAdjustmentLine_Pieces";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0+352+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M+140+0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0+352+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M+140+0), payType: PayType.Pieces));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void RanchAdjustmentLine_ProductionIncentiveBonus()
		{
			var dbName = "RanchAdjustmentLine_ProductionIncentiveBonus";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var payType = PayType.ProductionIncentiveBonus;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0 + 352 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M + 0 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M + 140 + 0), payType: payType));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0 + 352 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M + 0 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0 + 170 + 0), payType: payType));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, totalGross: (100.05M + 140 + 0), payType: payType));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void RanchAdjustmentLine_RegularNonProductiveTime()
		{
			var dbName = "RanchAdjustmentLine_RegularNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, otherGross: 0, totalGross: (45+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, otherGross: 0, totalGross: (107.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, otherGross: 0, totalGross: (1.5M+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_PiecesNonProductiveTime()
		{
			var dbName = "RanchAdjustmentLine_PiecesNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9.5M, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, totalGross: (0+352+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.08M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, totalGross: (0+170+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0M, grossFromPieces: 0, otherGross: 0, totalGross: (0M+0+0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, totalGross: (100.05M+0+0), payType: PayType.Regular));


			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 37.05M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 27.70M).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_IgnoresPremiumPay()
		{
			var dbName = "RanchAdjustmentLine_IgnoresPremiumPay";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, otherGross: 0, totalGross: (45 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0 + 0 + 0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, otherGross: 0, totalGross: (107.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0 + 0 + 0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, otherGross: 0, totalGross: (1.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, totalGross: (0 + 0 + 0), payType: PayType.Regular, laborCode: (int)RanchLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 20M, grossFromHours: 100, grossFromPieces: 100, otherGross: 100, totalGross: (100 + 100 + 100), payType: PayType.PremiumPay));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_CrewBoss()
		{
			var dbName = "RanchAdjustmentLine_CrewBoss";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 265, totalGross: (0+0+265), payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 291.5M, totalGross: (0+0+291.5M), payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 4, grossFromHours: 0, grossFromPieces: 0, otherGross: 82, totalGross: (0+0+82), payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6, grossFromHours: 0, grossFromPieces: 0, otherGross: 300, totalGross: (0+0+300), payType: PayType.CBCommission));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 265 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 291.5M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 382 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 38.2M).Count());
		}

		[TestMethod]
		public void RanchAdjustmentLine_SelectOnlyNewLines()
		{
			var dbName = "RanchAdjustmentLine_SelectOnlyNewLines";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, otherGross: 0, totalGross: (110 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: (75 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: (75 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, otherGross: 0, totalGross: (165 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: (4.95M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: (4.95M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, otherGross: 0, totalGross: (5.1M + 0 + 0), payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, otherGross: 0, totalGross: (110 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, totalGross: (10 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, totalGross: (2.5M + 0 + 0), payType: PayType.Regular));

			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: (75 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, totalGross: (75 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, otherGross: 0, totalGross: (165 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, totalGross: (15 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: (4.95M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, totalGross: (4.95M + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, otherGross: 0, totalGross: (5.1M + 0 + 0), payType: PayType.Regular));
			
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Ranches);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
		}

		#endregion

		#region Plant Pay Line Tests

		[TestMethod]
		public void PlantPayLine_SelectsMinimumWageForShiftDate()
		{
			var dbName = "PlantPayLine_SelectsMinimumWageForShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup mock minimum wages
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), 9M);

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17));
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == 8.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == 8.75M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == 9M).Count());
		}

		[TestMethod]
		public void PlantPayLine_UsesLastOfPlantSortedByRecordId()
		{
			var dbName = "PlantPayLine_UsesLastOfPlantSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, plant: (int)Plant.Reedley));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, plant: (int)Plant.Reedley));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, plant: (int)Plant.Sanger));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, plant: (int)Plant.Office));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, plant: (int)Plant.Office));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, plant: (int)Plant.Unknown));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == (int)Plant.Reedley).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == (int)Plant.Kerman).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == (int)Plant.Sanger).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == (int)Plant.Office).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == (int)Plant.Cutler).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == (int)Plant.Unknown).Count());
		}

		[TestMethod]
		public void PlantPayLine_GroupsByEmployee()
		{
			var dbName = "PlantPayLine_GroupsByEmployee";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2").Count());
		}

		[TestMethod]
		public void PlantPayLine_GroupsByWeekEnding()
		{
			var dbName = "PlantPayLine_GroupsByWeekEnding";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
		}

		[TestMethod]
		public void PlantPayLine_GroupsByShiftDate()
		{
			var dbName = "PlantPayLine_GroupsByShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(4, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
		}

		[TestMethod]
		public void PlantPayLine_GroupsByAlternativeWorkWeek()
		{
			var dbName = "PlantPayLine_GroupsByAlternativeWorkWeek";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: true));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == true).Count());
		}

		[TestMethod]
		public void PlantPayLine_Regular()
		{
			var dbName = "PlantPayLine_Regular";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (110+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (165+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (5.1M+0+0+0), payType: PayType.Regular));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
		}

		[TestMethod]
		public void PlantPayLine_Pieces()
		{
			var dbName = "PlantPayLine_Pieces";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 0, otherGross: 0, totalGross: (0+352+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+140+0+0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 0, otherGross: 0, totalGross: (0+352+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+140+0+0), payType: PayType.Pieces));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void PlantPayLine_Pieces_WithIncentive()
		{
			var dbName = "PlantPayLine_Pieces_WithIncentive";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 42.42M, otherGross: 0, totalGross: (0+352+42.42M+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 0M, grossFromPieces: 0, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+0+100.05M+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 0M, grossFromPieces: 140, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+140+100.05M+0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 37.37M, otherGross: 0, totalGross: (0+352+37.37M+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 0M, grossFromPieces: 0, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+0+100.05M+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 0M, grossFromPieces: 140, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+140+100.05M+0), payType: PayType.Pieces));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 394.42M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.44M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 389.37M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 43.26M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void PlantPayLine_RegularNonProductiveTime()
		{
			var dbName = "PlantPayLine_RegularNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (45+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (107.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (1.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void PlantPayLine_PiecesNonProductiveTime()
		{
			var dbName = "PlantPayLine_PiecesNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9.5M, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 15, otherGross: 0, totalGross: (0+352+15+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.08M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 15, otherGross: 0, totalGross: (0+170+15+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0M+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+0+0+0), payType: PayType.Regular));


			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 367 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 38.63M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 285.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 29.24M).Count());
		}

		[TestMethod]
		public void PlantPayLine_IgnoresPremiumPay()
		{
			var dbName = "PlantPayLine_IgnoresPremiumPay";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (45 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0 + 0 + 0 + 0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (107.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0 + 0 + 0 + 0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (1.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0 + 0 + 0 + 0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 20M, grossFromHours: 100, grossFromPieces: 100, grossFromIncentive: 100, otherGross: 100, totalGross: (100 + 100 + 100 + 100), payType: PayType.PremiumPay));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void PlantPayLine_CrewRateInMinimumAssurance_SelectsCrewRate()
		{
			var dbName = "PlantPayLine_CrewRateInMinimumAssurance_SelectsCrewRate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (110 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (165 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (5.1M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));

			context.SaveChanges();

			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), 9M);

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummaries(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15 && x.MinimumWage == 14).Count());
		}

		#endregion

		#region Plant Adjustment Line Tests

		[TestMethod]
		public void PlantAdjustmentLine_SelectsMinimumWageForShiftDate()
		{
			var dbName = "PlantAdjustmentLine_SelectsMinimumWageForShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Setup mock minimum wages
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), 9M);

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17));
			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == 8.5M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == 8.75M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == 9M).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_UsesLastOfPlantSortedByRecordId()
		{
			var dbName = "PlantAdjustmentLine_UsesLastOfPlantSortedByRecordId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, plant: (int)Plant.Reedley));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, plant: (int)Plant.Reedley));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, plant: (int)Plant.Kerman));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, plant: (int)Plant.Sanger));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, plant: (int)Plant.Office));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, plant: (int)Plant.Office));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, plant: (int)Plant.Cutler));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, plant: (int)Plant.Unknown));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == (int)Plant.Reedley).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == (int)Plant.Kerman).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == (int)Plant.Sanger).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == (int)Plant.Office).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == (int)Plant.Cutler).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == (int)Plant.Unknown).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_GroupsByEmployee()
		{
			var dbName = "PlantAdjustmentLine_GroupsByEmployee";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2").Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_GroupsByWeekEnding()
		{
			var dbName = "PlantAdjustmentLine_GroupsByWeekEnding";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_GroupsByShiftDate()
		{
			var dbName = "PlantAdjustmentLine_GroupsByShiftDate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(4, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17)).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18)).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_GroupsByAlternativeWorkWeek()
		{
			var dbName = "PlantAdjustmentLine_GroupsByAlternativeWorkWeek";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: false));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: true));
			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == false).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.AlternativeWorkWeek == true).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_Regular()
		{
			var dbName = "PlantAdjustmentLine_Regular";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (110+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (165+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (5.1M+0+0+0), payType: PayType.Regular));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_Pieces()
		{
			var dbName = "PlantAdjustmentLine_Pieces";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 0, otherGross: 0, totalGross: (0+352+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+140+0+0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 0, otherGross: 0, totalGross: (0+352+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+140+0+0), payType: PayType.Pieces));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void PlantAdjustmentLine_Pieces_WithIncentive()
		{
			var dbName = "PlantAdjustmentLine_Pieces_WithIncentive";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 42.42M, otherGross: 0, totalGross: (0+352+42.42M+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 0M, grossFromPieces: 0, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+0+100.05M+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 0M, grossFromPieces: 140, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+140+100.05M+0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 37.37M, otherGross: 0, totalGross: (0+352+37.37M+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 0M, grossFromPieces: 0, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+0+100.05M+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 0, otherGross: 0, totalGross: (0+170+0+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 0M, grossFromPieces: 140, grossFromIncentive: 100.05M, otherGross: 0, totalGross: (0M+140+100.05M+0), payType: PayType.Pieces));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 394.42M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.44M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 41.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 389.37M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 43.26M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 410.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 45.56M).Count());

		}

		[TestMethod]
		public void PlantAdjustmentLine_RegularNonProductiveTime()
		{
			var dbName = "PlantAdjustmentLine_RegularNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (45+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (107.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (1.5M+0+0+0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_PiecesNonProductiveTime()
		{
			var dbName = "PlantAdjustmentLine_PiecesNonProductiveTime";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9.5M, grossFromHours: 0, grossFromPieces: 352, grossFromIncentive: 15, otherGross: 0, totalGross: (0+352+15+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.08M, grossFromHours: 0, grossFromPieces: 170, grossFromIncentive: 15, otherGross: 0, totalGross: (0+170+15+0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0M+0+0+0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (100.05M+0+0+0), payType: PayType.Regular));


			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(2, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 367 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 38.63M).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 285.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 29.24M).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_IgnorePremiumPay()
		{
			var dbName = "PlantAdjustmentLine_IgnorePremiumPay";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (45 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0 + 0 + 0 + 0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (107.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0 + 0 + 0 + 0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.NonProductiveTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (1.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (0 + 0 + 0 + 0), payType: PayType.Regular, laborCode: (int)PlantLaborCode.RecoveryTime));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 20M, grossFromHours: 100, grossFromPieces: 100, grossFromIncentive: 100, otherGross: 100, totalGross: (100 + 100 + 100 + 100), payType: PayType.PremiumPay));

			context.SaveChanges();


			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(3, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 95 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 107.5M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 109 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_SelectOnlyNewLines()
		{
			var dbName = "PlantAdjustmentLine_SelectOnlyNewLines";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (110 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (165 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (5.1M + 0 + 0 + 0), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (110 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (165 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, isOriginal: true, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (5.1M + 0 + 0 + 0), payType: PayType.Regular));

			context.SaveChanges();

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
		}

		[TestMethod]
		public void PlantAdjustmentLine_CrewRateInMinimumAssuranceFlag_SelectsCrewRate()
		{
			var dbName = "PlantAdjustmentLine_CrewRateInMinimumAssuranceFlag_SelectsCrewRate";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (50 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (110 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (10 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (2.5M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));

			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (75 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (165 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (15 + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (4.95M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));
			context.Add(EntityMocker.MockPlantAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, grossFromIncentive: 0, otherGross: 0, totalGross: (5.1M + 0 + 0 + 0), payType: PayType.Regular, useCrewLaborRateForMinimumAssurance: true));

			context.SaveChanges();


			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 17), 8.5M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 18), 8.75M);
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2020, 2, 19), 9M);

			var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService, _roundingService, _mockCrewLaborWageService);
			var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id, Company.Plants);

			Assert.AreEqual(6, rates.Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15 && x.MinimumWage == 14).Count());
			Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15 && x.MinimumWage == 14).Count());

		}

		#endregion
	}
}
