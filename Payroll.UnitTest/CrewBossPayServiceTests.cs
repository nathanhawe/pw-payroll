using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Service;
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
            var batch = MockBatch(id: 1);
            context.Add(batch);

            // Mock crew boss pay lines
            var hourlyTreeCB = MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 1, hoursWorked: 10, employeeId: "TestHourlyTrees", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyTrees);
            var hourlyVineCB = MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 2, hoursWorked: 10, employeeId: "TestHourlyVines", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.HourlyVines);
            var hourlySouthCB = MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 3, hoursWorked: 10, employeeId: "TestSouthHourly", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthHourly);
            var dailySouthCB = MockCrewBossPayLine(batchId: batch.Id, weekEndDate: new DateTime(2020, 2, 16), shiftDate: new DateTime(2020, 2, 10), crew: 4, hoursWorked: 10, employeeId: "TestSouthDaily", payMethod: Payroll.Domain.Constants.CrewBossPayMethod.SouthDaily);
            context.AddRange(hourlyTreeCB, hourlyVineCB, hourlySouthCB, dailySouthCB);

            // Mock ranch pay lines
            for (int i = 0; i < 30; i++)
            {
                context.Add(MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: hourlyTreeCB.WeekEndDate, shiftDate: hourlyTreeCB.ShiftDate, crew: hourlyTreeCB.Crew, employeeId: $"Crew{hourlyTreeCB.Crew.ToString()}#{i.ToString()}"));
                context.Add(MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: hourlyVineCB.WeekEndDate, shiftDate: hourlyVineCB.ShiftDate, crew: hourlyVineCB.Crew, employeeId: $"Crew{hourlyVineCB.Crew.ToString()}#{i.ToString()}"));
                context.Add(MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: hourlySouthCB.WeekEndDate, shiftDate: hourlySouthCB.ShiftDate, crew: hourlySouthCB.Crew, employeeId: $"Crew{hourlySouthCB.Crew.ToString()}#{i.ToString()}"));
                context.Add(MockRanchPayLine(batchId: batch.Id, hoursWorked: 10, weekEndDate: dailySouthCB.WeekEndDate, shiftDate: dailySouthCB.ShiftDate, crew: dailySouthCB.Crew, employeeId: $"Crew{dailySouthCB.Crew.ToString()}#{i.ToString()}"));
            }

            context.SaveChanges();
        }

        private RanchPayLine MockRanchPayLine(
            int id = 0,
            DateTime? dateCreated = null,
            DateTime? dateModified = null,
            bool isDeleted = false,
            int batchId = 0,
            int layoffId = 0,
            int quickBaseRecordId = 0,
            DateTime? weekEndDate = null,
            DateTime? shiftDate = null,
            int crew = 0,
            string employeeId = "TEST",
            int laborCode = 0,
            int blockId = 0,
            decimal hoursWorked = 0,
            string payType = "1-Regular",
            decimal pieces = 0,
            decimal pieceRate = 0,
            decimal hourlyRate = 0,
            decimal otDtWotRate = 0,
            decimal otDtWotHours = 0,
            decimal grossFromHours = 0,
            decimal grossFromPieces = 0,
            decimal otherGross = 0,
            bool alternativeWorkWeek = false,
            bool fiveEight = false)
        {
            dateCreated ??= DateTime.Now;
            dateModified ??= DateTime.Now;
            weekEndDate ??= new DateTime(2020, 1, 5);
            shiftDate ??= new DateTime(2020, 1, 5);

            return new RanchPayLine
            {
                Id = id,
                DateCreated = dateCreated.Value,
                DateModified = dateModified.Value,
                IsDeleted = isDeleted,
                BatchId = batchId,
                LayoffId = layoffId,
                QuickBaseRecordId = quickBaseRecordId,
                WeekEndDate = weekEndDate.Value,
                ShiftDate = shiftDate.Value,
                Crew = crew,
                EmployeeId = employeeId,
                LaborCode = laborCode,
                BlockId = blockId,
                HoursWorked = hoursWorked,
                PayType = payType,
                Pieces = pieces,
                PieceRate = pieceRate,
                HourlyRate = hourlyRate,
                OtDtWotRate = otDtWotRate,
                OtDtWotHours = otDtWotHours,
                GrossFromHours = grossFromHours,
                GrossFromPieces = grossFromPieces,
                OtherGross = otherGross,
                AlternativeWorkWeek = alternativeWorkWeek,
                FiveEight = fiveEight
            };
        }

        private CrewBossPayLine MockCrewBossPayLine(
            int id = 0,
            DateTime? dateCreated = null,
            DateTime? dateModified = null,
            bool isDeleted = false,
            int batchId = 0,
            int layoffId = 0,
            int quickBaseRecordId = 0,
            DateTime? weekEndDate = null,
            DateTime? shiftDate = null,
            int crew = 0,
            string employeeId = "TEST",
            string payMethod = "",
            int workerCount = 0,
            decimal hoursWorked = 0,
            decimal hourlyRate = 0,
            decimal gross = 0)
        {
            dateCreated ??= DateTime.Now;
            dateModified ??= DateTime.Now;
            weekEndDate ??= new DateTime(2020, 1, 5);
            shiftDate ??= new DateTime(2020, 1, 5);

            return new CrewBossPayLine
            {
                Id = id,
                DateCreated = dateCreated.Value,
                DateModified = dateModified.Value,
                IsDeleted = isDeleted,
                BatchId = batchId,
                LayoffId = layoffId,
                QuickBaseRecordId = quickBaseRecordId,
                WeekEndDate = weekEndDate.Value,
                ShiftDate = shiftDate.Value,
                Crew = crew,
                EmployeeId = employeeId,
                PayMethod = payMethod,
                WorkerCount = workerCount,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                Gross = gross
            };
        }

        private Batch MockBatch(
            int? id = null, 
            DateTime? dateCreated = null, 
            DateTime? dateModified = null, 
            bool? isDeleted = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            string state = null, 
            bool? isComplete = null, 
            string owner = null)
        {
            id ??= 0;
            dateCreated ??= DateTime.Now;
            dateModified ??= DateTime.Now;
            isDeleted ??= false;
            startDate ??= DateTime.Now;
            //endDate ??= DateTime.Now;
            //state ??= "Pending";
            isComplete ??= false;
            owner ??= "Test";

            return new Batch
            {
                Id = id.Value,
                DateCreated = dateCreated.Value,
                DateModified = dateModified.Value,
                IsDeleted = isDeleted.Value,
                StartDate = startDate,
                EndDate = endDate,
                State = state,
                IsComplete = isComplete.Value,
                Owner = owner
            };

        }
    }
}
