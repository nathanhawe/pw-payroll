using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Domain;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    public class PlantMinimumMakeUpCalculatorTests
    {
        private PlantMinimumMakeUpCalculator _minimumMakeUpCalculator = new PlantMinimumMakeUpCalculator();

        [TestMethod]
        public void EffectiveRateIsLessThanMinimum_GenerateMinimumMakeUps()
        {
            var dailySummaries = new List<DailySummary>()
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 7, TotalGross = 100.03M, TotalHours = 14.29M},
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 6.5M, TotalGross = 260, TotalHours = 40},
                new DailySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 15, EffectiveHourlyRate = 14.99M, TotalGross = 599.6M, TotalHours = 40}
            };

            var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(dailySummaries);

            Assert.AreEqual(3, minimumMakeUps.Count());
            Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee1" && x.Gross == 14.29M));
            Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee2" && x.Gross == 60M));
            Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee3" && x.Gross == .4M));
        }

        [TestMethod]
        public void EffectiveRateIsEqualToMinimum_DoNotGenerate()
        {
            var dailySummaries = new List<DailySummary>()
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 114.32M, TotalHours = 14.29M},
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 320, TotalHours = 40},
                new DailySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 15, EffectiveHourlyRate = 15M, TotalGross = 600M, TotalHours = 40}
            };

            var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(dailySummaries);

            Assert.AreEqual(0, minimumMakeUps.Count());
        }

        [TestMethod]
        public void EffectiveRateIsGreaterThanMinimum_DoNotGenerate()
        {
            var dailySummaries = new List<DailySummary>()
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 114.32M, TotalHours = 14.29M},
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 320, TotalHours = 40},
                new DailySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 15, EffectiveHourlyRate = 15M, TotalGross = 600M, TotalHours = 40}
            };

            var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(dailySummaries);

            Assert.AreEqual(0, minimumMakeUps.Count());
        }
    }
}
