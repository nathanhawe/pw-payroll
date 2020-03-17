using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    [TestClass]
    public class PlantSummaryServiceTests
    {
        private PayrollContext _context;
        private PlantSummaryService _plantSummaryService;

        [TestInitialize]
        public void Setup()
        {
            var dbName = "PlantSummaryServiceTests";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new PayrollContext(options);
            _context.Database.EnsureCreated();

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));

            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
            _context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));


            _context.SaveChanges();
            _plantSummaryService = new PlantSummaryService(_context);
        }
        [TestMethod]
        public void SumOfHoursWorked()
        {
            var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, plantSummaries.Count());
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalHours == 50));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalHours == 50));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalHours == 50));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalHours == 50));
        }

        [TestMethod]
        public void SumOfTotalGross()
        {
            var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, plantSummaries.Count());
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalGross == 750));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 750));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 750));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 750));
        }

        [TestMethod]
        public void GroupsByWeekEndDate()
        {
            var plantSummaries = _plantSummaryService.CreateSummariesForBatch(2);

            Assert.AreEqual(8, plantSummaries.Count());
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750));
            Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750));
        }

        [TestMethod]
        public void GroupsByEmployeeId()
        {
            var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, plantSummaries.Count());
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee1"));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee2"));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee3"));
            Assert.AreEqual(50, plantSummaries.Where(x => x.EmployeeId == "Employee4"));
        }
    }
}
