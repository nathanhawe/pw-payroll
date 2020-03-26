using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Domain;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class RanchMinimumMakeUpCalculatorTests
	{
		private RoundingService _roundingService = new RoundingService();
		private RanchMinimumMakeUpCalculator _minimumMakeUpCalculator;

		[TestInitialize]
		public void Setup()
		{
			_minimumMakeUpCalculator = new RanchMinimumMakeUpCalculator(_roundingService);
		}

		[TestMethod]
		public void EffectiveRateIsLessThanMinimum_GenerateMinimumMakeUps()
		{
			var weeklySummaries = new List<WeeklySummary>()
			{
				new WeeklySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 7, TotalGross = 100.03M, TotalHours = 14.29M},
				new WeeklySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 6.5M, TotalGross = 260, TotalHours = 40},
				new WeeklySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 6.5M, TotalGross = 130, TotalHours = 20},
				new WeeklySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 15, EffectiveHourlyRate = 14.99M, TotalGross = 299.8M, TotalHours = 20}
			};

			var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(weeklySummaries);

			Assert.AreEqual(4, minimumMakeUps.Count());
			Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee1" && x.Gross == 14.29M).Count());
			Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee2" && x.Gross == 60M).Count());
			Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee3" && x.Gross == 30M).Count());
			Assert.AreEqual(1, minimumMakeUps.Where(x => x.EmployeeId == "Employee3" && x.Gross == .2M).Count());
		}

		[TestMethod]
		public void EffectiveRateIsEqualToMinimum_DoNotGenerate()
		{
			var weeklySummaries = new List<WeeklySummary>()
			{
				new WeeklySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 114.32M, TotalHours = 14.29M},
				new WeeklySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 320, TotalHours = 40},
				new WeeklySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8M, TotalGross = 160M, TotalHours = 20},
				new WeeklySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 15, EffectiveHourlyRate = 15M, TotalGross = 300M, TotalHours = 20}
			};

			var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(weeklySummaries);

			Assert.AreEqual(0, minimumMakeUps.Count());
		}

		[TestMethod]
		public void EffectiveRateIsGreaterThanMinimum_DoNotGenerate()
		{
			var weeklySummaries = new List<WeeklySummary>()
			{
				new WeeklySummary { EmployeeId = "Employee1", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 114.32M, TotalHours = 14.29M},
				new WeeklySummary { EmployeeId = "Employee2", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8, TotalGross = 320, TotalHours = 40},
				new WeeklySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 8, EffectiveHourlyRate = 8M, TotalGross = 160M, TotalHours = 20},
				new WeeklySummary { EmployeeId = "Employee3", WeekEndDate = new DateTime(2020, 2, 23), MinimumWage = 15, EffectiveHourlyRate = 15M, TotalGross = 300M, TotalHours = 20}
			};

			var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(weeklySummaries);

			Assert.AreEqual(0, minimumMakeUps.Count());
		}
	}
}
