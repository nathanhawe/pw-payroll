﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void CorrectlySumTotalHours()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, OverTimeHours = 2 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 13M, OverTimeHours = 2, DoubleTimeHours = 1},
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(1, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 63M));
        }

        [TestMethod]
        public void CorrectlySumTotalGross()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180, OverTimeHours = 2 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 13M, TotalGross = 195, OverTimeHours = 2, DoubleTimeHours = 1},
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(1, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalGross == 945M));
        }

        [TestMethod]
        public void CorrectlySumOverTimeHours()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, OverTimeHours = 2 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 13M, OverTimeHours = 2, DoubleTimeHours = 1},
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(1, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalOverTimeHours == 5M));
        }

        [TestMethod]
        public void CorrectlySumDoubleTimeHours()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M},
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 13M, OverTimeHours = 2, DoubleTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 14M, OverTimeHours = 2, DoubleTimeHours = 2 },
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(1, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalDoubleTimeHours == 3M));
        }

        [TestMethod]
        public void CorrectlyCalculatesWeeklyEffectiveRate()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, OverTimeHours = 2 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },

                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M },
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M },
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M },
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M }
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(2, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15M));
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15.85M));
        }

        [TestMethod]
        public void MultipleWeekEndings()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, OverTimeHours = 2 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },

                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 24), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 25), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 26), TotalHours = 10M, TotalGross = 167M, NonProductiveTime = 0M, EffectiveHourlyRate = 16.7M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 27), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 28), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 3, 1), ShiftDate = new DateTime(2020, 2, 29), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M }
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(2, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15M));
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15.85M));

        }

        [TestMethod]
        public void MultipleMinimumWages()
        {
            var dailySummary = new List<DailySummary>
            {
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.5M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.5M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.5M },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.75M, OverTimeHours = 1 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.75M, OverTimeHours = 2 },
                new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.75M },
            };

            var weeklySummaryCalculator = new WeeklySummaryCalculator();
            var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary);

            Assert.AreEqual(2, weeklySummary.Count());
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 27M && x.NonProductiveTime == 0M && x.TotalGross == 405M && x.EffectiveHourlyRate == 15M && x.MinimumWage == 8.5M));
            Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 33M && x.NonProductiveTime == 0M && x.TotalGross == 495M && x.EffectiveHourlyRate == 15M && x.MinimumWage == 8.75M));
        }
    }
}
