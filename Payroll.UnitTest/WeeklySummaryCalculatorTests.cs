using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Payroll.Domain;
using System.Linq;

namespace Payroll.UnitTest
{
    [TestClass]
    public class WeeklySummaryCalculatorTests
    {
        [TestMethod]
        public void BasicTest()
        {
            var dailySummary = new List<DailySummary>();
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });

            dailySummary.Add(new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });

            var weeklyEffectiveRateCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklyEffectiveRateCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(2, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15M));
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15.85M));

        }

        [TestMethod]
        public void MultipleWeekEndings()
        {
            var dailySummary = new List<DailySummary>();
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });

            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 24), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 25), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 26), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 27), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 28), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });
            dailySummary.Add(new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 29), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M });

            var weeklyEffectiveRateCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklyEffectiveRateCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(2, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15M));
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15.85M));

        }
    }
}
