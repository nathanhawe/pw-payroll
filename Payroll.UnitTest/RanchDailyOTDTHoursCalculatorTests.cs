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
	public class RanchDailyOTDTHoursCalculatorTests
	{
		private RanchDailyOTDTHoursCalculator _ranchDailyOTDTHoursCalculator = new RanchDailyOTDTHoursCalculator();

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest__RegularSchedule_OverTimeBetweenEightAndTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());

		}

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest_RegularSchedule_DoubleTimeOverTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M).Count());
		}

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest_AlternativeSchedule_OverTimeBetweenTenAndTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 1.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 2M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 2M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 2M).Count());
		}

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest_AlternativeSchedule_DoubleTimeOverTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M).Count());
		}

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest_SeventhDay_OverTimeUpToEightHours()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 2).Count());
		}

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest_SeventhDay_DoubleTimeOverEightHours()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 0).Count());
		}

		[TestMethod]
		public void Crew_8_OfficeClerical_EastWest_IgnoresFiveEight()
		{
			var crew = (int)Crew.OfficeClerical_EastWest;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());
		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_RegularSchedule_OverTimeBetweenEightAndTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());

		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_RegularSchedule_DoubleTimeOverTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M).Count());
		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_AlternativeSchedule_OverTimeBetweenTenAndTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 1.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 2M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 2M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 2M).Count());
		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_AlternativeSchedule_DoubleTimeOverTwelveHours()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = true, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M).Count());
		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_IgnoresFiveEight()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());
		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_SeventhDay_OverTimeUpToEightHours()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 2).Count());
		}

		[TestMethod]
		public void Crew_9_OfficeClerical_South_SeventhDay_DoubleTimeOverEightHours()
		{
			var crew = (int)Crew.OfficeClerical_South;
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = crew, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 0).Count());
		}

		[TestMethod]
		public void RegularCrews_BeforeYear2019_OverTimeAfterTenHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10M && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 1.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 2M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 3M).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2019_OverTimeAfterNineAndOneHalfHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2019, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10M && x.OverTimeHours == .5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.OverTimeHours == 1.5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 2.49M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 2.5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 3.5M).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2020_OverTimeAfterNineHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10M && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.OverTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 2.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 3M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2021_OverTimeAfterEightAndOneHalfHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == .5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10M && x.OverTimeHours == 1.5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.OverTimeHours == 2.5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.49M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 3.5M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4.5M).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2022_OverTimeAfterEightHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10M && x.OverTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.OverTimeHours == 3).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());
		}

		[TestMethod]
		public void RegularCrews_BeforeYear2022_NoDoubleTimeOnNonSeventhDay()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2020, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2021, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13.99M && x.DoubleTimeHours == 0M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14M && x.DoubleTimeHours == 0M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15M && x.DoubleTimeHours == 0M).Count());
		}

		[TestMethod]
		public void RegularCrews_Year2022_DoubleTimeAfterTwelveHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 13.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 1, 1), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2022, 12, 31), AlternativeWorkWeek = false, FiveEight = false, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13.99M && x.DoubleTimeHours == 1.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14M && x.DoubleTimeHours == 2M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15M && x.DoubleTimeHours == 3M).Count());
		}

		[TestMethod]
		public void RegularCrews_NoNonSeventhDayDoubleTime()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 0).Count());
		}

		[TestMethod]
		public void RegularCrews_IgnoreAlternativeWorkWeek()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 10M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 11, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 1, 1), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, ShiftDate = new DateTime(2018, 12, 31), AlternativeWorkWeek = true, FiveEight = false, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 0 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 10M && x.OverTimeHours == 0 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11 && x.OverTimeHours == 1 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 1.99M && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 2M && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 3M && x.DoubleTimeHours == 0).Count());
		}

		[TestMethod]
		public void RegularCrews_SeventhDay_OverTimeUpToEightHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 0M).Count());
		}

		[TestMethod]
		public void RegularCrews_SeventhDay_DoubleTimeOverEightHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 0).Count());
		}


		[TestMethod]
		public void RegularCrews_FiveEight_OverTimeBetweenEightAndTwelveHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 2, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 3, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 4, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 5, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 6, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 7, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 1 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 2 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 3 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 4 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 5 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 6 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 7 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());

		}

		[TestMethod]
		public void RegularCrews_FiveEight_DoubleTimeOverTwelveHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 12, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 14, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 15, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = false, FiveEight = true, TotalHours = 16.25M, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12 && x.DoubleTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13 && x.DoubleTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 14 && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 15 && x.DoubleTimeHours == 3).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 16.25M && x.DoubleTimeHours == 4.25M).Count());
		}

		[TestMethod]
		public void RegularCrews_FiveEight_IgnoresAlternativeWorkWeek()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 8, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 8.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 9, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 11.99M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 12M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 12.01M, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, AlternativeWorkWeek = true, FiveEight = true, TotalHours = 13, OverTimeHours = 0, DoubleTimeHours = 0},
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8 && x.OverTimeHours == 0).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 8.01M && x.OverTimeHours == .01M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 9 && x.OverTimeHours == 1).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 11.99M && x.OverTimeHours == 3.99M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 12.01M && x.OverTimeHours == 4M).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.TotalHours == 13M && x.OverTimeHours == 4M).Count());
		}

		[TestMethod]
		public void RegularCrews_FiveEight_SeventhDay_OverTimeUpToEightHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 2).Count());
		}

		[TestMethod]
		public void RegularCrews_FiveEight_SeventhDay_DoubleTimeOverEightHours()
		{
			var dailySummaries = new List<DailySummary>
			{
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee1", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 17), TotalHours = 0, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 18), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 19), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 20), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 21), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 22), TotalHours = 1, OverTimeHours = 0, DoubleTimeHours = 0},
				new DailySummary{ Crew = 100, EmployeeId = "Employee2", FiveEight = true, WeekEndDate = new DateTime(2020, 2, 23), ShiftDate = new DateTime(2020, 2, 23), TotalHours = 10, OverTimeHours = 0, DoubleTimeHours = 0}
			};

			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee1" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 2).Count());
			Assert.AreEqual(1, dailySummaries.Where(x => x.EmployeeId == "Employee2" && x.ShiftDate == new DateTime(2020, 2, 23) && x.DoubleTimeHours == 0).Count());
		}
	}
}
