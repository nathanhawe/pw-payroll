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

namespace Payroll.UnitTest
{
    [TestClass]
    public class DailySummaryCalculatorTests
    {
        private MockMinimumWageService _mockMinimumWageService;
        
        [TestInitialize]
        public void Setup()
        {
            _mockMinimumWageService = new MockMinimumWageService();
        }

        #region Ranch Pay Line Tests

        [TestMethod]
        public void RanchPayLine_SelectsMinimumWageForShiftDate()
        {
            var dbName = "RanchPayLine_SelectsMinimumWageForShiftDate";
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));
            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == 8.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == 8.75M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == 9M).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, crew: 4));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, crew: 4));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, crew: 5));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, crew: 6));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

            Assert.AreEqual(6, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == 1).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == 2).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == 3).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.Crew == 4).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.Crew == 5).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.Crew == 6).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, fiveEight: true));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, fiveEight: true));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, fiveEight: true));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, fiveEight: true));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, fiveEight: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, fiveEight: true));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, fiveEight: false));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: false));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: true));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, payType: PayType.HourlyPlusPieces));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, payType: PayType.HourlyPlusPieces));

            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

            Assert.AreEqual(6, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 240.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 24.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 240.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.67M).Count());
            
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 380));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 381));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 380));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9.5M, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 380));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.08M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 381));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));


            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

            Assert.AreEqual(2, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 35.2M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 27.01M).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 265, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 291.5M, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 4, grossFromHours: 0, grossFromPieces: 0, otherGross: 82, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6, grossFromHours: 0, grossFromPieces: 0, otherGross: 300, payType: PayType.CBCommission));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummaries(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 265 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 291.5M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 382 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 38.2M).Count());
        }

        #endregion

        #region Ranch Adjustment Line Tests

        [TestMethod]
        public void RanchAdjustmentLine_SelectsMinimumWageForShiftDate()
        {
            var dbName = "RanchAdjustmentLine_SelectsMinimumWageForShiftDate";
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));
            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.MinimumWage == 8.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.MinimumWage == 8.75M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.MinimumWage == 9M).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, crew: 1));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, crew: 1));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, crew: 2));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, crew: 3));

            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, crew: 4));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, crew: 4));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, crew: 5));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, crew: 6));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 1, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 2, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 3, fiveEight: true));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 4, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 5, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 6, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 7, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 8, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 9, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 10, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 11, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 12, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 13, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 14, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 15, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 16, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 17, fiveEight: true));

            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 18, fiveEight: true));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, quickBaseRecordId: 19, fiveEight: true));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, quickBaseRecordId: 20, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 21, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 22, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 23, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 24, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 25, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 26, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 27, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 28, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 29, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 30, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 31, fiveEight: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 32, fiveEight: true));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), payType: PayType.Regular, quickBaseRecordId: 33, fiveEight: false));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

            Assert.AreEqual(6, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.FiveEight == false).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.FiveEight == true).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.FiveEight == true).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.FiveEight == true).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.FiveEight == false).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.FiveEight == false).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 3, 1), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));

            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), payType: PayType.Regular, alternativeWorkWeek: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: false));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), payType: PayType.Regular, alternativeWorkWeek: true));
            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 110, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));

            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 75, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 165, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 15, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .33M, grossFromHours: 4.95M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .34M, grossFromHours: 5.1M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, payType: PayType.HourlyPlusPieces));

            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 3.33M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee2", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 5.67M, grossFromHours: 100.05M, grossFromPieces: 140, otherGross: 0, payType: PayType.HourlyPlusPieces));

            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

            Assert.AreEqual(6, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 240.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 24.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 240.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.67M).Count());

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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 4.5M, grossFromHours: 45, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 380));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10.75M, grossFromHours: 107.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 381));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 1, grossFromHours: 10, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 2.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .15M, grossFromHours: 1.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: .1M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 380));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9.5M, grossFromHours: 0, grossFromPieces: 352, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: .5M, grossFromHours: 0, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 380));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 3.08M, grossFromHours: 0, grossFromPieces: 170, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: .25M, grossFromHours: 0M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular, laborCode: 381));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "Employee1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 6.67M, grossFromHours: 100.05M, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));


            context.SaveChanges();

            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

            Assert.AreEqual(2, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 35.2M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 27.01M).Count());
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
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 265, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 291.5M, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 4, grossFromHours: 0, grossFromPieces: 0, otherGross: 82, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchAdjustmentLine(batchId: batch.Id, employeeId: "CrewBoss1", weekEndDate: new DateTime(2020, 2, 23), shiftDate: new DateTime(2020, 2, 19), hoursWorked: 6, grossFromHours: 0, grossFromPieces: 0, otherGross: 300, payType: PayType.CBCommission));

            context.SaveChanges();


            var dailySummaryCalculator = new DailySummaryCalculator(context, _mockMinimumWageService);
            var rates = dailySummaryCalculator.GetDailySummariesFromAdjustments(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 265 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 291.5M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 382 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 38.2M).Count());
        }

        #endregion
    }
}
