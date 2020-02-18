using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain.Constants;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    [TestClass]
    public class PaidSickLeaveServiceTests
    {
        [TestMethod]
        public void UpdateTracking_Ranch()
        {
            var dbName = "UpdateTracking_Ranch";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock a new batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 0, grossFromPieces: 100, otherGross: 0, payType: PayType.Pieces));
            
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, grossFromHours: 0, grossFromPieces: 0, otherGross: 179, payType: PayType.CBDaily));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 0, otherGross: 193.5M, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 215, payType: PayType.CBHourlyVines));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.CBCommission));
            
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 200, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, grossFromHours: 160, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, grossFromHours: 0, grossFromPieces: 0, otherGross: 179, payType: PayType.CBDaily));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 0, otherGross: 144, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 160, payType: PayType.CBHourlyVines));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.CBCommission));

            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.UpdateTracking(batch.Id, Company.Ranches);

            Assert.AreEqual(12, context.PaidSickLeaves.Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
        }

        [TestMethod]
        public void UpdateTracking_Ranch_UpdateExistingPSL()
        {
            var dbName = "UpdateTracking_Ranch_Update_UpdateExistingPSL";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock a new batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock existing PSL
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 0, grossFromPieces: 100, otherGross: 0, payType: PayType.Pieces));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, grossFromHours: 0, grossFromPieces: 0, otherGross: 179, payType: PayType.CBDaily));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 0, otherGross: 193.5M, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 215, payType: PayType.CBHourlyVines));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.CBCommission));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 200, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, grossFromHours: 160, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, grossFromHours: 0, grossFromPieces: 0, otherGross: 179, payType: PayType.CBDaily));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 0, otherGross: 144, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 160, payType: PayType.CBHourlyVines));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.CBCommission));

            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.UpdateTracking(batch.Id, Company.Ranches);

            Assert.AreEqual(12, context.PaidSickLeaves.Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
        }

        [TestMethod]
        public void UpdateTracking_Ranch_IncludeOnlyHourPiecesCBPayTypes()
        {
            var dbName = "UpdateTracking_Ranch_IncludeOnlyHourPiecesCBPayTypes";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock a new batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock ranch pay lines
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 50, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 0, grossFromPieces: 100, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, grossFromHours: 0, grossFromPieces: 0, otherGross: 179, payType: PayType.CBDaily));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 0, otherGross: 193.5M, payType: PayType.CBHourlyTrees));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 215, payType: PayType.CBHourlyVines));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.CBCommission));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 0, grossFromPieces: 100, otherGross: 0, payType: PayType.OverTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, grossFromHours: 80, grossFromPieces: 0, otherGross: 0, payType: PayType.DoubleTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, grossFromHours: 0, grossFromPieces: 0, otherGross: 179, payType: PayType.MinimumAssurance));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, grossFromHours: 0, grossFromPieces: 0, otherGross: 193.5M, payType: PayType.MinimumAssurance_Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 0, grossFromPieces: 0, otherGross: 215, payType: PayType.MinimumAssurance_OverTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.MinimumAssurance_DoubleTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.MinimumAssurance_WeeklyOverTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.Vacation));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.Holiday));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.WeeklyOverTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, grossFromHours: 0, grossFromPieces: 0, otherGross: 123.45M, payType: PayType.Adjustment));

            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.UpdateTracking(batch.Id, Company.Ranches);

            Assert.AreEqual(6, context.PaidSickLeaves.Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());
        }
        
        [TestMethod]
        public void UpdateUsage_Ranch_AddNew()
        {
            var dbName = "UpdateUsage_Ranch_AddNew";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock Batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock PSL Request
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 150, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, grossFromHours: 60, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, grossFromHours: 7.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 3.75M, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));

            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.UpdateUsage(batch.Id, Company.Ranches);

            Assert.AreEqual(3, context.PaidSickLeaves.Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10));
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4));
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .75M));
        }

        [TestMethod]
        public void UpdateUsage_Ranch_OverwriteExisting()
        {
            var dbName = "UpdateUsage_Ranch_OverwriteExisting";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock Batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock PSL Request
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 150, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, grossFromHours: 60, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, grossFromHours: 7.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 3.75M, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));

            // Mock Existing PSL lines
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), company: Company.Ranches, hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 1));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), company: Company.Ranches, hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 2));
            context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), company: Company.Ranches, hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 3));

            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.UpdateUsage(batch.Id, Company.Ranches);

            Assert.AreEqual(3, context.PaidSickLeaves.Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == 10));
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == 4));
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == .75M));
        }

        [TestMethod]
        public void UpdateUsage_Ranch_SelectOnlyPSLType()
        {
            var dbName = "UpdateUsage_Ranch_SelectOnlyPSLType";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock Batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock PSL Request
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 150, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, grossFromHours: 60, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, grossFromHours: 7.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, grossFromHours: 3.75M, grossFromPieces: 0, otherGross: 0, payType: PayType.SickLeave));

            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 150, grossFromPieces: 0, otherGross: 0, payType: PayType.Regular));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 150, grossFromPieces: 0, otherGross: 0, payType: PayType.OverTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, grossFromHours: 150, grossFromPieces: 0, otherGross: 0, payType: PayType.DoubleTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, grossFromHours: 60, grossFromPieces: 0, otherGross: 0, payType: PayType.Pieces));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, grossFromHours: 60, grossFromPieces: 0, otherGross: 0, payType: PayType.Vacation));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, grossFromHours: 60, grossFromPieces: 0, otherGross: 0, payType: PayType.Holiday));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, grossFromHours: 7.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.Adjustment));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, grossFromHours: 7.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.WeeklyOverTime));
            context.Add(Helper.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, grossFromHours: 7.5M, grossFromPieces: 0, otherGross: 0, payType: PayType.MinimumAssurance));

            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.UpdateUsage(batch.Id, Company.Ranches);

            Assert.AreEqual(3, context.PaidSickLeaves.Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10));
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4));
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .75M));
        }

        

        [TestMethod]
        public void CalculateNinetyDay_Ranch()
        {
            var dbName = "CalculateNinetyDay_Ranch";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Mock Batch
            var batch = Helper.MockBatch(id: 1);
            context.Add(batch);

            // Mock Existing PSL lines
            var hourlyRate = 15M;
            var baseDate = new DateTime(2020, 1, 1);
            for (int i = 0; i < 120; i++)
            {
                var currentDate = baseDate.AddDays(i);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday) continue;

                context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee1", shiftDate: currentDate, company: Company.Ranches, hours: 10, gross: 10 * hourlyRate));
                context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee2", shiftDate: currentDate, company: Company.Ranches, hours: 10, gross: 10 * (hourlyRate + 2.5M)));
                context.Add(Helper.MockPaidSickLeave(batchId: batch.Id, EmployeeId: "Employee3", shiftDate: currentDate, company: Company.Ranches, hours: 10, gross: 10 * (hourlyRate - 2.5M)));
            }
            context.SaveChanges();

            var pslService = new PaidSickLeaveService(context);
            pslService.CalculateNinetyDay(batch.Id, new DateTime(2020, 3, 30), new DateTime(2020, 4, 5));

            Assert.AreEqual(21, context.PaidSickLeaves.Where(x => x.NinetyDayGross > 0 && x.NinetyDayHours > 0));

            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 760 && x.NinetyDayGross == 11400).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 770 && x.NinetyDayGross == 11550).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());

            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 760 && x.NinetyDayGross == 13300).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 770 && x.NinetyDayGross == 13475).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());

            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 760 && x.NinetyDayGross == 9500).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 770 && x.NinetyDayGross == 9625).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
            Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
        }


        [TestMethod]
        public void UpdateTracking_Plant()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void UpdateUsage_Plant()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CalculateNinetyDay_Plant()
        {
            throw new NotImplementedException();
        }

    }
}
