using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    [TestClass]
    public class PlantWeeklyOTHoursCalculatorTests
    {
        private PlantWeeklyOTHoursCalculator _ranchWeeklyOTHoursCalculator = new PlantWeeklyOTHoursCalculator();

        [TestMethod]
        public void WeeklyOTAfterFourtyHours()
        {
            var weeklySummaries = new List<WeeklySummary>
            {
                new WeeklySummary{ EmployeeId = "Employee1", Crew = (int)Plant.Cutler, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 60, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
                new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Plant.Kerman, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
                new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Plant.Office, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
                new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Plant.Reedley, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
                new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Plant.Sanger, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 26, TotalOverTimeHours = 8, TotalDoubleTimeHours = 2}
            };

            var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

            Assert.AreEqual(3, weeklyOverTimeHours.Count());
            Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 8 && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
            Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 8 && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
            Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee3" && x.Crew == 8 && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 0).Count());
        }
    }
}