using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Payroll.Domain;
using System.Linq;
using Payroll.Domain.Constants;

namespace Payroll.UnitTest
{
	[TestClass]
	public class PlantWeeklySummaryCalculatorTests
	{
		private RoundingService _roundingService = new RoundingService();
		
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

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 10 },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(1, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 63M).Count());
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

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 9.99M },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 9.98M },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 9.97M },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 9.96M },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 9.95M },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 9.94M },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(1, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalGross == 1004.79M).Count());
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

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 10 },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(1, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalOverTimeHours == 5M).Count());
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

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 10 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 10 },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(1, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalDoubleTimeHours == 3M).Count());
		}

		[TestMethod]
		public void CorrectlyCalculatesWeeklyEffectiveRate()
		{
			var dailySummary = new List<DailySummary>
			{
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 15.38M, OverTimeHours = 2  },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 150M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 17.14M, OverTimeHours = 1  },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 150M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 19.35M },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 150M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 15M, OverTimeHours = 2, DoubleTimeHours = 1 },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 150M, NonProductiveTime = .5M, MinimumWage = 15M, EffectiveHourlyRate = 15M, OverTimeHours = 2, DoubleTimeHours = 2 },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = .5M, MinimumWage = 15M, EffectiveHourlyRate = 15.79M, OverTimeHours = 2  },

				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 8M, TotalGross = 167M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 21.55M },
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 8M, TotalGross = 167M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 21.55M },
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 167M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 21.55M },
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 8M, TotalGross = 150M, NonProductiveTime = .25M, MinimumWage = 15M, EffectiveHourlyRate = 19.35M },
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 8M, TotalGross = 150M, NonProductiveTime = .5M, MinimumWage = 15M, EffectiveHourlyRate = 20M },
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 8M, TotalGross = 150M, NonProductiveTime = .5M, MinimumWage = 15M, EffectiveHourlyRate = 20M }
			};

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 11.25M },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 22.5M },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(2, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 2M && x.TotalGross == 933.75M && x.EffectiveHourlyRate == 16.10M).Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 48M && x.NonProductiveTime == 2M && x.TotalGross == 951M && x.EffectiveHourlyRate == 20.67M).Count());
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

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 0 },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(2, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15M).Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 3, 1) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 951M && x.EffectiveHourlyRate == 15.85M).Count());

		}

		[TestMethod]
		public void UseGreatestMinimumWage()
		{
			var dailySummary = new List<DailySummary>
			{
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.5M },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 9M, TotalGross = 135M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.5M },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 8M, TotalGross = 120M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.5M },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 11M, TotalGross = 165M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 8.75M, OverTimeHours = 1 },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 12M, TotalGross = 180M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 7.5M, OverTimeHours = 2 },
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 10M, TotalGross = 150M, NonProductiveTime = 0M, EffectiveHourlyRate = 15M, MinimumWage = 6.75M },
			};
			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 0 },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(1, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.TotalHours == 60M && x.NonProductiveTime == 0M && x.TotalGross == 900M && x.EffectiveHourlyRate == 15M && x.MinimumWage == 8.75M).Count());
		}

		[TestMethod]
		public void UsesLastOfCrewByShiftDate()
		{
			var dailySummary = new List<DailySummary>
			{
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = 1},
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = 2},
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), Crew = 3},
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = 4},
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = 6},
				new DailySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = 5},
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = 6},
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = 5},
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), Crew = 4},
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = 3},
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = 2},
				new DailySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = 1},
			};

			var minimumMakeUps = new List<MinimumMakeUp>
			{
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), Crew = (int)Plant.Kerman, Gross = 0 },
				new MinimumMakeUp { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), Crew = (int)Plant.Kerman, Gross = 0 },
			};

			var weeklySummaryCalculator = new PlantWeeklySummaryCalculator(_roundingService);
			var weeklySummary = weeklySummaryCalculator.GetWeeklySummary(dailySummary, minimumMakeUps);

			Assert.AreEqual(2, weeklySummary.Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.Crew == 5).Count());
			Assert.AreEqual(1, weeklySummary.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 2, 23) && x.Crew == 1).Count());
		}
	}
}
