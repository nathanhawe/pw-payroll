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
		private readonly RoundingService _roundingService = new RoundingService();
		private PlantWeeklyOTHoursCalculator _ranchWeeklyOTHoursCalculator;

		[TestInitialize]
		public void Setup()
		{
			_ranchWeeklyOTHoursCalculator = new PlantWeeklyOTHoursCalculator(_roundingService);
		}

		[TestMethod]
		public void WeeklyOTAfterFourtyHours()
		{
			var weeklySummaries = new List<WeeklySummary>
			{
				new WeeklySummary{ EmployeeId = "Employee1", Crew = (int)Plant.Cutler, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, TotalHours = 60, TotalOverTimeHours = 12, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee2", Crew = (int)Plant.Office, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 60, TotalOverTimeHours = 0, TotalDoubleTimeHours = 0},
				new WeeklySummary{ EmployeeId = "Employee3", Crew = (int)Plant.Sanger, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 62, TotalOverTimeHours = 20, TotalDoubleTimeHours = 2},
				new WeeklySummary{ EmployeeId = "Employee4", Crew = (int)Plant.Kerman, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 70, TotalOverTimeHours = 20, TotalDoubleTimeHours = 2},
				new WeeklySummary{ EmployeeId = "Employee5", Crew = (int)Plant.Reedley, WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8.5M, TotalHours = 56, TotalOverTimeHours = 8, TotalDoubleTimeHours = 0}
			};

			var weeklyOverTimeHours = _ranchWeeklyOTHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			Assert.AreEqual(4, weeklyOverTimeHours.Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee1" && x.Crew == (int)Plant.Cutler && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee2" && x.Crew == (int)Plant.Office && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 20).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee4" && x.Crew == (int)Plant.Kerman && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
			Assert.AreEqual(1, weeklyOverTimeHours.Where(x => x.EmployeeId == "Employee5" && x.Crew == (int)Plant.Reedley && x.WeekEndDate == new DateTime(2020, 2, 23) && x.OverTimeHours == 8).Count());
		}
	}
}