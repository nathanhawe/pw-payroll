using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    [TestClass]
    public class RanchSummaryServiceTests
    {
        private PayrollContext _context;
        private RanchSummaryService _ranchSummaryService;

        [TestInitialize]
        public void Setup()
        {
            var dbName = "RanchSummaryServiceTests";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new PayrollContext(options);
            _context.Database.EnsureCreated();

            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 27, lastCrew: 100, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", crew: 27, lastCrew: 100, hoursWorked: 10, totalGross: 172));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));

            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
            _context.Add(Helper.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", crew: 27, lastCrew: 100, hoursWorked: 10, totalGross: 150));


            _context.SaveChanges();
            _ranchSummaryService = new RanchSummaryService(_context);
        }
        [TestMethod]
        public void SumOfHoursWorked()
        {
            var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, ranchSummaries.Count());
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalHours == 50));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalHours == 50));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalHours == 50));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalHours == 50));
        }

        [TestMethod]
        public void SumOfTotalGross()
        {
            var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, ranchSummaries.Count());
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalGross == 750));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 750));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 750));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 750));
        }

        [TestMethod]
        public void SumOfCulturalHours_AreHoursForCrewsBelow61()
        {
            var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, ranchSummaries.Count());
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CulturalHours == 0));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 0));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 0));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 10));
        }

        [TestMethod]
        public void GroupsByWeekEndDate()
        {
            var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(2);

            Assert.AreEqual(8, ranchSummaries.Count());
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 142 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 60 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 10));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 142 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 60 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0));
            Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 10));
        }

        [TestMethod]
        public void GroupsByEmployeeId()
        {
            var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, ranchSummaries.Count());
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee1"));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee2"));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee3"));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee4"));
        }

        [TestMethod]
        public void GroupsByLastCrew()
        {
            var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

            Assert.AreEqual(4, ranchSummaries.Count());
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.LastCrew == 100));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.LastCrew == 142));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.LastCrew == 60));
            Assert.AreEqual(50, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.LastCrew == 100));
        }
    }
}
