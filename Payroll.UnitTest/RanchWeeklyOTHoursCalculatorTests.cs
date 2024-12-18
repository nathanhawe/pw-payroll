﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	public class RanchWeeklyOTHoursCalculatorTests
	{
		private readonly RoundingService _roundingService = new RoundingService();
		private RanchWeeklyOTHoursCalculator _ranchWeeklyOTHoursCalculator;

		[TestInitialize]
		public void Setup()
		{
			_ranchWeeklyOTHoursCalculator = new RanchWeeklyOTHoursCalculator(_roundingService);
		}

		[TestMethod]
		public void Crew_Eight_OfficeClerical_EastWest_WeeklyOTAfterFourtyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = (int)Crew.OfficeClerical_EastWest, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 60, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Crew.OfficeClerical_EastWest, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Crew.OfficeClerical_EastWest, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Crew.OfficeClerical_EastWest, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Crew.OfficeClerical_EastWest, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 26, TotalOverTimeHours = 8, TotalDoubleTimeHours = 2}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(2, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == (int)Crew.OfficeClerical_EastWest && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == (int)Crew.OfficeClerical_EastWest && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			
		}

		[TestMethod]
		public void Crew_Nine_OfficeClerical_South_WeeklyOTAfterFourtyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = (int)Crew.OfficeClerical_South, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 60, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Crew.OfficeClerical_South, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Crew.OfficeClerical_South, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Crew.OfficeClerical_South, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Crew.OfficeClerical_South, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 26, TotalOverTimeHours = 8, TotalDoubleTimeHours = 2}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(2, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == (int)Crew.OfficeClerical_South && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == (int)Crew.OfficeClerical_South && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
		}

		[TestMethod]
		public void RegularCrews_FiveEight_WeeklyOTAfterFourtyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 60, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 30, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 26, TotalOverTimeHours = 8, TotalDoubleTimeHours = 2}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(2, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
		}

		[TestMethod]
		public void RegularCrews_BeforeYear2019_WeeklyOTAfterSixtyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2018, 1, 1), MinimumWage = 8, TotalHours = 72, TotalOverTimeHours = 10, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2018, 1, 1), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2018, 1, 1), MinimumWage = 8.5M, TotalHours = 36, TotalOverTimeHours = 4.5M, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2018, 12, 31), MinimumWage = 8, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2018, 12, 31), MinimumWage = 8.5M, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(2, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2018, 1, 1) && x.OverTimeHours == 2).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2018, 1, 1) && x.OverTimeHours == 1.5M).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2019_WeeklyOTAfterFiftyFiveHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2019, 1, 1), MinimumWage = 8, TotalHours = 72, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2019, 1, 1), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2019, 1, 1), MinimumWage = 8.5M, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2019, 12, 31), MinimumWage = 8, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2019, 12, 31), MinimumWage = 8.5M, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(3, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2019, 1, 1) && x.OverTimeHours == 5).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2019, 1, 1) && x.OverTimeHours == 5).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee3" && x.Crew == 100 && x.WeekEndDate == new DateTime(2019, 12, 31) && x.OverTimeHours == 5).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2020_WeeklyOTAfterFiftyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2020, 1, 1), MinimumWage = 8, TotalHours = 72, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2020, 1, 1), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2020, 1, 1), MinimumWage = 8.5M, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2020, 12, 31), MinimumWage = 8, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2020, 12, 31), MinimumWage = 8.5M, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(3, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2020, 1, 1) && x.OverTimeHours == 10).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2020, 1, 1) && x.OverTimeHours == 10).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee3" && x.Crew == 100 && x.WeekEndDate == new DateTime(2020, 12, 31) && x.OverTimeHours == 10).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2021_WeeklyOTAfterFourtyFiveHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2021, 1, 1), MinimumWage = 8, TotalHours = 72, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2021, 1, 1), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2021, 1, 1), MinimumWage = 8.5M, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2021, 12, 31), MinimumWage = 8, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2021, 12, 31), MinimumWage = 8.5M, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(3, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2021, 1, 1) && x.OverTimeHours == 15).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2021, 1, 1) && x.OverTimeHours == 15).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee3" && x.Crew == 100 && x.WeekEndDate == new DateTime(2021, 12, 31) && x.OverTimeHours == 15).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2022_WeeklyOTAfterFourtyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2022, 1, 1), MinimumWage = 8, TotalHours = 72, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2022, 1, 1), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2022, 1, 1), MinimumWage = 8.5M, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2099, 12, 31), MinimumWage = 8, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2099, 12, 31), MinimumWage = 8.5M, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(3, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2022, 1, 1) && x.OverTimeHours == 20).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2022, 1, 1) && x.OverTimeHours == 20).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee3" && x.Crew == 100 && x.WeekEndDate == new DateTime(2099, 12, 31) && x.OverTimeHours == 20).Count());
		}

		[TestMethod]
		public void EmitLastBlockIdAndLaborCodeOrderedByMinimumWage()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2022, 1, 1), MinimumWage = 8, TotalHours = 72, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0, LastBlockId = 42, LastLaborCode = 109},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2022, 1, 1), MinimumWage = 8, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0, LastBlockId = 43, LastLaborCode = 212},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2022, 1, 1), MinimumWage = 8.5M, TotalHours = 36, TotalOverTimeHours = 6, TotalDoubleTimeHours = 0, LastBlockId = 44, LastLaborCode = 260},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2099, 12, 31), MinimumWage = 8, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0, LastBlockId = 45, LastLaborCode = 330},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = 100, FiveEight = false, WeekEndDate = new DateTime(2099, 12, 31), MinimumWage = 8.5M, TotalHours = 42, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0, LastBlockId = 46, LastLaborCode = 108}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(3, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == 100 && x.WeekEndDate == new DateTime(2022, 1, 1) && x.OverTimeHours == 20 && x.BlockId == 42 && x.LaborCode == 109).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == 100 && x.WeekEndDate == new DateTime(2022, 1, 1) && x.OverTimeHours == 20 && x.BlockId == 44 && x.LaborCode == 260).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee3" && x.Crew == 100 && x.WeekEndDate == new DateTime(2099, 12, 31) && x.OverTimeHours == 20 && x.BlockId == 46 && x.LaborCode == 108).Count());
		}
	}
}