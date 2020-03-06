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
    public class PlantDailyOTDTHoursCalculatorTests
    {
        private PlantDailyOTDTHoursCalculator _ranchDailyOTDTHoursCalculator = new PlantDailyOTDTHoursCalculator();

        [TestMethod]
        public void RegularSchedule_OverTimeBetweenEightAndTwelveHours()
        {
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = false, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = false, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = false, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = false, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = false, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = false, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = false, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, AlternativeWorkWeek = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, AlternativeWorkWeek = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = false, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = false, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
            };

            _ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M));

        }

        [TestMethod]
        public void RegularSchedule_DoubleTimeOverTwelveHours()
        {
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = false, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, AlternativeWorkWeek = false, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = false, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
            };

            _ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M));
        }

        [TestMethod]
        public void AlternativeSchedule_OverTimeBetweenTenAndTwelveHours()
        {
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = true, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = true, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = true, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = true, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = true, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = true, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = true, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, AlternativeWorkWeek = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, AlternativeWorkWeek = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
            };

            _ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 1.99M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 2M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 2M));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 2M));
        }

        [TestMethod]
        public void AlternativeSchedule_DoubleTimeOverTwelveHours()
        {
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary{ Crew = (int)Plant.Cutler, AlternativeWorkWeek = true, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, AlternativeWorkWeek = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, AlternativeWorkWeek = true, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, AlternativeWorkWeek = true, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, AlternativeWorkWeek = true, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
            };

            _ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3));
            Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M));
        }

        [TestMethod]
        public void SeventhDay_OverTimeUpToEightHours()
        {
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary{ Crew = (int)Plant.Cutler, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
            };

            _ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

            Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8));
            Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 0));
        }

        [TestMethod]
        public void SeventhDay_DoubleTimeOverEightHours()
        {
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary{ Crew = (int)Plant.Cutler, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Cutler, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Kerman, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Office, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Reedley, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
                new DailySummary{ Crew = (int)Plant.Sanger, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
            };

            _ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

            Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 2));
            Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 0));
        }
    }
}
