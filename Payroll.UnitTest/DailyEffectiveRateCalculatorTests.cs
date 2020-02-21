using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Data;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Payroll.Domain.Constants;
using System.Linq;

namespace Payroll.UnitTest
{
    [TestClass]
    public class DailyEffectiveRateCalculatorTests
    {

        [TestMethod]
        public void Regular()
        {
            var dbName = "Regular";
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


            var dailyEffectiveRateCalculator = new DailyEffectiveRateCalculator(context);
            var rates = dailyEffectiveRateCalculator.GetDailyEffectiveRates(batch.Id);

            Assert.AreEqual(6, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 150 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 165 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 15).Count());
        }

        [TestMethod]
        public void Pieces()
        {
            var dbName = "Pieces";
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

            var dailyEffectiveRateCalculator = new DailyEffectiveRateCalculator(context);
            var rates = dailyEffectiveRateCalculator.GetDailyEffectiveRates(batch.Id);

            Assert.AreEqual(6, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 35.2M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 27.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 240.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 24.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 9 && x.TotalGross == 352 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 39.11M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 9 && x.TotalGross == 270.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 30.01M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 9 && x.TotalGross == 240.05M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.67M).Count());
            
        }

        [TestMethod]
        public void RegularNonProductiveTime()
        {
            var dbName = "RegularNonProductiveTime";
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


            var dailyEffectiveRateCalculator = new DailyEffectiveRateCalculator(context);
            var rates = dailyEffectiveRateCalculator.GetDailyEffectiveRates(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 100 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 10).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 11 && x.TotalGross == 110 && x.NonProductiveTime == .1M && x.EffectiveHourlyRate == 10).Count());
        }

        [TestMethod]
        public void PiecesNonProductiveTime()
        {
            var dbName = "PiecesNonProductiveTime";
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

            var dailyEffectiveRateCalculator = new DailyEffectiveRateCalculator(context);
            var rates = dailyEffectiveRateCalculator.GetDailyEffectiveRates(batch.Id);

            Assert.AreEqual(2, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 352 && x.NonProductiveTime == .5M && x.EffectiveHourlyRate == 35.2M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 10 && x.TotalGross == 270.05M && x.NonProductiveTime == .25M && x.EffectiveHourlyRate == 27.01M).Count());
        }

        [TestMethod]
        public void CrewBoss()
        {
            var dbName = "CrewBoss";
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


            var dailyEffectiveRateCalculator = new DailyEffectiveRateCalculator(context);
            var rates = dailyEffectiveRateCalculator.GetDailyEffectiveRates(batch.Id);

            Assert.AreEqual(3, rates.Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 17) && x.TotalHours == 10 && x.TotalGross == 265 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 18) && x.TotalHours == 11 && x.TotalGross == 291.5M && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 26.5M).Count());
            Assert.AreEqual(1, rates.Where(x => x.EmployeeId == "CrewBoss1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.ShiftDate == new DateTime(2020, 2, 19) && x.TotalHours == 10 && x.TotalGross == 382 && x.NonProductiveTime == 0 && x.EffectiveHourlyRate == 38.2M).Count());
        }
    }
}
