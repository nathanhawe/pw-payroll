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
	public class PaidSickLeaveServiceTests
	{
		[TestMethod]
		public void UpdateTracking_Ranch_AddNewPSL()
		{
			var dbName = "UpdateTracking_Ranch";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 50, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 100, payType: PayType.Pieces));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 80, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 80, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: 179, payType: PayType.CBDaily));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: 193.5M, payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: 215, payType: PayType.CBHourlyVines));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: 123.45M, payType: PayType.CBCommission));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: 200, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: 160, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, totalGross: 179, payType: PayType.CBDaily));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, totalGross: 144, payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: 160, payType: PayType.CBHourlyVines));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, totalGross: 123.45M, payType: PayType.CBCommission));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Ranches);

			Assert.AreEqual(12, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
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
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock existing PSL
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));


			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 50, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 100, payType: PayType.Pieces));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 80, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 80, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: 179, payType: PayType.CBDaily));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: 193.5M, payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: 215, payType: PayType.CBHourlyVines));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: 123.45M, payType: PayType.CBCommission));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: 200, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: 160, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, totalGross: 179, payType: PayType.CBDaily));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, totalGross: 144, payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: 160, payType: PayType.CBHourlyVines));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, totalGross: 123.45M, payType: PayType.CBCommission));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Ranches);

			Assert.AreEqual(12, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
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
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.HourlyPlusPieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: (0 + 0 + 179), payType: PayType.CBDaily));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: (0 + 0 + 193.5M), payType: PayType.CBHourlyTrees));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (0 + 0 + 215), payType: PayType.CBHourlyVines));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.CBCommission));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.OverTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.DoubleTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: (0 + 0 + 179), payType: PayType.MinimumAssurance));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: (0 + 0 + 193.5M), payType: PayType.MinimumAssurance_Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (0 + 0 + 215), payType: PayType.MinimumAssurance_OverTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.MinimumAssurance_DoubleTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.MinimumAssurance_WeeklyOverTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.Vacation));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.Holiday));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.WeeklyOverTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.Adjustment));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Ranches);

			Assert.AreEqual(6, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());
		}

		[TestMethod]
		public void UpdateTracking_Ranch_IgnoreDeleted()
		{
			var dbName = "UpdateTracking_Ranch_IgnoreDeleted";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock ranch pay lines
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 50, payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 100, payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: 100, payType: PayType.Pieces, isDeleted: true));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Ranches);

			Assert.AreEqual(1, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
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
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Ranches);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .75M).Count());
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
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			// Mock Existing PSL lines
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 1));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 2));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 3));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Ranches);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == .75M).Count());
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
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.OverTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.DoubleTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.Vacation));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.Holiday));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.Adjustment));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.WeeklyOverTime));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.MinimumAssurance));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Ranches);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .75M).Count());
		}

		[TestMethod]
		public void UpdateUsage_Ranch_IgnoreDeleted()
		{
			var dbName = "UpdateUsage_Ranch_IgnoreDeleted";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Existing PSL lines
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 1));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 2, isDeleted: true));

			// Mock PSL Request
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave, isDeleted: true));
			context.Add(EntityMocker.MockRanchPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave, isDeleted: true));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Ranches);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4 && !x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Ranches && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 2 && x.IsDeleted).Count());

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
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Existing PSL lines
			var hourlyRate = 15M;
			var baseDate = new DateTime(2020, 1, 1);
			for (int i = 0; i < 120; i++)
			{
				var currentDate = baseDate.AddDays(i);
				if (currentDate.DayOfWeek == DayOfWeek.Sunday) continue;

				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee1", shiftDate: currentDate, hours: 10, gross: 10 * hourlyRate));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee2", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate + 2.5M)));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee3", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate - 2.5M)));
			}
			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.CalculateNinetyDay(batch.Id, Company.Ranches, new DateTime(2020, 3, 30), new DateTime(2020, 4, 5));

			Assert.AreEqual(21, context.PaidSickLeaves.Where(x => x.NinetyDayGross > 0 && x.NinetyDayHours > 0).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 760 && x.NinetyDayGross == 11400).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 770 && x.NinetyDayGross == 11550).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 760 && x.NinetyDayGross == 13300).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 770 && x.NinetyDayGross == 13475).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 760 && x.NinetyDayGross == 9500).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 770 && x.NinetyDayGross == 9625).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
		}

		[TestMethod]
		public void CalculateNinetyDay_Ranch_IgnoreDeleted()
		{
			var dbName = "CalculateNinetyDay_Ranch_IgnoreDeleted";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Existing PSL lines
			var hourlyRate = 15M;
			var baseDate = new DateTime(2020, 1, 1);
			for (int i = 0; i < 120; i++)
			{
				var currentDate = baseDate.AddDays(i);
				if (currentDate.DayOfWeek == DayOfWeek.Sunday) continue;

				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee1", shiftDate: currentDate, hours: 10, gross: 10 * hourlyRate));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee2", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate + 2.5M)));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Ranches, EmployeeId: "Employee3", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate - 2.5M), isDeleted: true));
			}
			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.CalculateNinetyDay(batch.Id, Company.Ranches, new DateTime(2020, 3, 30), new DateTime(2020, 4, 5));

			Assert.AreEqual(14, context.PaidSickLeaves.Where(x => x.NinetyDayGross > 0 && x.NinetyDayHours > 0).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 760 && x.NinetyDayGross == 11400).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 770 && x.NinetyDayGross == 11550).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 760 && x.NinetyDayGross == 13300).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 770 && x.NinetyDayGross == 13475).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Ranches && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
		}


		[TestMethod]
		public void UpdateTracking_Plant_AddNewPSL()
		{
			var dbName = "UpdateTracking_Plant";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: (0 + 179 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: (0 + 193.5M + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (0 + 215 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 123.45M + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (0 + 200 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (160 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, totalGross: (0 + 179 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, totalGross: (0 + 144 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (0 + 160 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, totalGross: (0 + 123.45M + 0), payType: PayType.Pieces));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Plants);

			Assert.AreEqual(12, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
		}

		[TestMethod]
		public void UpdateTracking_Plant_UpdateExistingPSL()
		{
			var dbName = "UpdateTracking_Plant_UpdateExistingPSL";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hours: 0, gross: 0));

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: (0 + 179 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: (0 + 193.5M + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (0 + 215 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 123.45M + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (0 + 200 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (160 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, totalGross: (0 + 179 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, totalGross: (0 + 144 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (0 + 160 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, totalGross: (0 + 123.45M + 0), payType: PayType.Pieces));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Plants);

			Assert.AreEqual(12, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
		}

		[TestMethod]
		public void UpdateTracking_Plant_IncludeOnlyHoursAndPieces()
		{
			var dbName = "UpdateTracking_Plant_IncludeOnlyHoursAndPieces";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: (0 + 179 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: (0 + 193.5M + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (0 + 215 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 123.45M + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (0 + 200 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (160 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 8, totalGross: (0 + 179 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 9, totalGross: (0 + 144 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 10, totalGross: (0 + 160 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 11, totalGross: (0 + 123.45M + 0), payType: PayType.Pieces));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.OverTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (80 + 0 + 0), payType: PayType.DoubleTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 8, totalGross: (0 + 0 + 179), payType: PayType.MinimumAssurance));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee4", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 9, totalGross: (0 + 0 + 193.5M), payType: PayType.MinimumAssurance_Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee5", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (0 + 0 + 215), payType: PayType.MinimumAssurance_OverTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.MinimumAssurance_DoubleTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.MinimumAssurance_WeeklyOverTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.Vacation));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.Holiday));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.WeeklyOverTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee6", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 11, totalGross: (0 + 0 + 123.45M), payType: PayType.Adjustment));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Plants);

			Assert.AreEqual(12, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 9 && x.Gross == 193.5M).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 215).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 11 && x.Gross == 123.45M).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 200).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 8 && x.Gross == 179).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee4" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 9 && x.Gross == 144).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee5" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 160).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee6" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 11 && x.Gross == 123.45M).Count());
		}

		[TestMethod]
		public void UpdateTracking_Plant_IgnoreDeleted()
		{
			var dbName = "UpdateTracking_Plant_IgnoreDeleted";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock a new batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock plant pay lines
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (50 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 5, totalGross: (0 + 100 + 0), payType: PayType.Pieces, isDeleted: true));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateTracking(batch.Id, Company.Plants);

			Assert.AreEqual(1, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150).Count());
		}

		[TestMethod]
		public void UpdateUsage_Plant_AddNew()
		{
			var dbName = "UpdateUsage_Plant_AddNew";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Plants);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .75M).Count());
		}

		[TestMethod]
		public void UpdateUsage_Plant_OverwriteExisting()
		{
			var dbName = "UpdateUsage_Plant_OverwriteExisting";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			// Mock Existing PSL lines
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 1));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 2));
			context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hours: 10, gross: 150, ninetyDayHours: 1000, ninetyDayGross: 15000, hoursUsed: 3));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Plants);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 1000 && x.NinetyDayGross == 15000 && x.HoursUsed == .75M).Count());
		}

		[TestMethod]
		public void UpdateUsage_Plant_SelectOnlyPSLType()
		{
			var dbName = "UpdateUsage_Plant_SelectOnlyPSLType";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.Regular));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.OverTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.DoubleTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.Pieces));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.Vacation));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.Holiday));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.Adjustment));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.WeeklyOverTime));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.MinimumAssurance));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Plants);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .75M).Count());
		}

		[TestMethod]
		public void UpdateUsage_Plant_IgnoreDeleted()
		{
			var dbName = "UpdateUsage_Plant_IgnoreDeleted";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock PSL Request
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee1", shiftDate: new DateTime(2020, 2, 17), hoursWorked: 10, totalGross: (150 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee2", shiftDate: new DateTime(2020, 2, 18), hoursWorked: 4, totalGross: (60 + 0 + 0), payType: PayType.SickLeave));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .5M, totalGross: (7.5M + 0 + 0), payType: PayType.SickLeave, isDeleted: true));
			context.Add(EntityMocker.MockPlantPayLine(batchId: batch.Id, employeeId: "Employee3", shiftDate: new DateTime(2020, 2, 19), hoursWorked: .25M, totalGross: (3.75M + 0 + 0), payType: PayType.SickLeave));

			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.UpdateUsage(batch.Id, Company.Plants);

			Assert.AreEqual(3, context.PaidSickLeaves.Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 17) && x.HoursUsed == 10).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 18) && x.HoursUsed == 4).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.BatchId == batch.Id && x.Company == Company.Plants && x.EmployeeId == "Employee3" && x.ShiftDate == new DateTime(2020, 2, 19) && x.HoursUsed == .25M).Count());
		}

		[TestMethod]
		public void CalculateNinetyDay_Plant()
		{
			var dbName = "CalculateNinetyDay_Plant";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Existing PSL lines
			var hourlyRate = 15M;
			var baseDate = new DateTime(2020, 1, 1);
			for (int i = 0; i < 120; i++)
			{
				var currentDate = baseDate.AddDays(i);
				if (currentDate.DayOfWeek == DayOfWeek.Sunday) continue;

				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee1", shiftDate: currentDate, hours: 10, gross: 10 * hourlyRate));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee2", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate + 2.5M)));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee3", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate - 2.5M)));
			}
			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.CalculateNinetyDay(batch.Id, Company.Plants, new DateTime(2020, 3, 30), new DateTime(2020, 4, 5));

			Assert.AreEqual(21, context.PaidSickLeaves.Where(x => x.NinetyDayGross > 0 && x.NinetyDayHours > 0).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 760 && x.NinetyDayGross == 11400).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 770 && x.NinetyDayGross == 11550).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 760 && x.NinetyDayGross == 13300).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 770 && x.NinetyDayGross == 13475).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 760 && x.NinetyDayGross == 9500).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 770 && x.NinetyDayGross == 9625).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 9750).Count());
		}

		[TestMethod]
		public void CalculateNinetyDay_IgnoreDeleted()
		{
			var dbName = "CalculateNinetyDay_IgnoreDeleted";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			using var context = new PayrollContext(options);
			context.Database.EnsureCreated();

			// Mock Batch
			var batch = EntityMocker.MockBatch(id: 1);
			context.Add(batch);

			// Mock Existing PSL lines
			var hourlyRate = 15M;
			var baseDate = new DateTime(2020, 1, 1);
			for (int i = 0; i < 120; i++)
			{
				var currentDate = baseDate.AddDays(i);
				if (currentDate.DayOfWeek == DayOfWeek.Sunday) continue;

				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee1", shiftDate: currentDate, hours: 10, gross: 10 * hourlyRate));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee2", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate + 2.5M)));
				context.Add(EntityMocker.MockPaidSickLeave(batchId: batch.Id, company: Company.Plants, EmployeeId: "Employee3", shiftDate: currentDate, hours: 10, gross: 10 * (hourlyRate - 2.5M), isDeleted: true));
			}
			context.SaveChanges();

			var pslService = new PaidSickLeaveService(context);
			pslService.CalculateNinetyDay(batch.Id, Company.Plants, new DateTime(2020, 3, 30), new DateTime(2020, 4, 5));

			Assert.AreEqual(14, context.PaidSickLeaves.Where(x => x.NinetyDayGross > 0 && x.NinetyDayHours > 0).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 760 && x.NinetyDayGross == 11400).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 770 && x.NinetyDayGross == 11550).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 150 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee1" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 11700).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 760 && x.NinetyDayGross == 13300).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 770 && x.NinetyDayGross == 13475).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 175 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee2" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 5) && x.Hours == 0 && x.Gross == 0 && x.NinetyDayHours == 780 && x.NinetyDayGross == 13650).Count());

			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 30) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 3, 31) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 1) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 2) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 3) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
			Assert.AreEqual(1, context.PaidSickLeaves.Where(x => x.EmployeeId == "Employee3" && x.Company == Company.Plants && x.ShiftDate == new DateTime(2020, 4, 4) && x.Hours == 10 && x.Gross == 125 && x.NinetyDayHours == 0 && x.NinetyDayGross == 0 && x.IsDeleted).Count());
		}
	}
}
