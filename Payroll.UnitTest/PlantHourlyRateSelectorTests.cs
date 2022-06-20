using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Domain.Constants;
using Payroll.Service;
using Payroll.UnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class PlantHourlyRateSelectorTests
	{
		private readonly decimal _minimumWage = 10M;
		private MockMinimumWageService _mockMinimumWageService;
		private PlantHourlyRateSelector _plantHourlyRateSelector;
		private readonly decimal _currentH2ARate = 17.51M;

		[TestInitialize]
		public void Setup()
		{
			_mockMinimumWageService = new MockMinimumWageService();
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2000, 1, 1), _minimumWage);
			_plantHourlyRateSelector = new PlantHourlyRateSelector(_mockMinimumWageService);
		}

		private decimal DefaultTest(
			string payType = Payroll.Domain.Constants.PayType.Regular,
			int laborCode = -1,
			decimal employeeHourlyRate = 14,
			decimal hourlyRateOverride = 0,
			bool isH2A = false,
			decimal minimumWage = 10M,
			Plant plant = Domain.Constants.Plant.Sanger,
			DateTime? shiftDate = null,
			decimal payLineHourlyRate = 0,
			string positionTitle = "")
		{
			shiftDate ??= new DateTime(2020, 3, 11);
			var wageService = new MockMinimumWageService();
			wageService.Test_AddMinimumWage(shiftDate.Value, minimumWage);
			var rateSelector = new PlantHourlyRateSelector(wageService);

			return rateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, plant, shiftDate.Value, payLineHourlyRate, positionTitle);
		}

		[TestMethod]
		public void HourlyRateOverrideSupercedes_H2A()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = _currentH2ARate + 10M;
			var isH2A = true;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, DateTime.Now, 0, "");
			Assert.AreEqual(hourlyRateOverride, hourlyRate);
		}

		[TestMethod]
		public void H2A_Before20200518_Returns13_92()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, new DateTime(2020, 5, 17), 0, "");
			Assert.AreEqual(13.92M, hourlyRate);
		}

		[TestMethod]
		public void H2A_OnOrAfter20200518_Before20210426_Returns14_77()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;
			var startDate = new DateTime(2020, 5, 18);
			var endDate = new DateTime(2021, 4, 25);

			Assert.AreEqual(14.77M, _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, startDate, 0, ""));
			Assert.AreEqual(14.77M, _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, endDate, 0, ""));
		}

		[TestMethod]
		public void H2A_OnOrAfter20210426_Before20220509_Returns16_05()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;
			var startDate = new DateTime(2021, 4, 26);
			var endDate = new DateTime(2022, 5, 8);

			Assert.AreEqual(16.05M, _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, startDate, 0, ""));
			Assert.AreEqual(16.05M, _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, endDate, 0, ""));
		}

		[TestMethod]
		public void H2A_OnOrAfter20220509_Returns17_51()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;
			var startDate = new DateTime(2022, 5, 09);
			var endDate = startDate.AddYears(5);

			Assert.AreEqual(17.51M, _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, startDate, 0, ""));
			Assert.AreEqual(17.51M, _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, endDate, 0, ""));
		}

		[TestMethod]
		public void H2A_SupercedesLaborCode()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			for (int laborCode = 0; laborCode < 1000; laborCode++)
			{
				var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, DateTime.Now, 0, "");
				Assert.AreEqual(_currentH2ARate, hourlyRate);
			}
		}

		[TestMethod]
		public void H2A_SupercedesEmployeeRate()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = _currentH2ARate + 10;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, DateTime.Now, 0, "");
			Assert.AreEqual(_currentH2ARate, hourlyRate);
		}

		[TestMethod]
		public void H2A_SupercedesMinimumWage()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			var wageSelector = new MockMinimumWageService();
			wageSelector.Test_AddMinimumWage(new DateTime(2000, 1, 1), _currentH2ARate + 10);
			var hourlyRateSelector = new PlantHourlyRateSelector(wageSelector);
			var hourlyRate = hourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, DateTime.Now, 0, "");

			Assert.AreEqual(_currentH2ARate, hourlyRate);
		}

		[TestMethod]
		public void NonH2A_NoSpecialLaborCode_NoHourlyRateOverride_ReturnsMaxOf_EmployeeHourlyRate_MinimumWage()
		{

			// Employee Hourly Rate is greater
			Assert.AreEqual(15.5M, DefaultTest(employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

			// Minimum Wage is greater
			Assert.AreEqual(15.75M, DefaultTest(employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
		}

		#region Pay Type Tests

		[TestMethod]
		public void PayTypeRegular_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Regular));
		}

		[TestMethod]
		public void PayTypeOverTime_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.OverTime));
		}

		[TestMethod]
		public void PayTypeDoubleTime_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.DoubleTime));
		}

		[TestMethod]
		public void PayTypePieces_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.Pieces));
		}

		[TestMethod]
		public void PayTypeHourlyPlusPieces_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.HourlyPlusPieces));
		}

		[TestMethod]
		public void PayTypeProductionIncentiveBonus_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.ProductionIncentiveBonus));
		}

		[TestMethod]
		public void PayTypeMinimumAssurance_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceRegular_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_Regular));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceOT_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_OverTime));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceDT_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_DoubleTime));
		}

		[TestMethod]
		public void PayTypeMinimumAssuranceWOT_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.MinimumAssurance_WeeklyOverTime));
		}

		[TestMethod]
		public void PayTypeVacation_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Vacation));
		}

		[TestMethod]
		public void PayTypeHoliday_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Holiday));
		}

		[TestMethod]
		public void PayTypeBereavement_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.Bereavement));
		}

		[TestMethod]
		public void PayTypeSickLeave_ReturnsPayLineHourlyRate()
		{
			Assert.IsTrue(12.45M == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: 12.45M, hourlyRateOverride: 0M));

			// H2A supercedes the previously set value for HourlyRate (assumed to be the 90-day rate from PSL)
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: (_currentH2ARate + 1M), hourlyRateOverride: 0M, isH2A: true, shiftDate: DateTime.Now));

			// Hourly Rate Override supercedes everything
			Assert.IsTrue(14.75M == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: 12.45M, hourlyRateOverride: 14.75M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: 12.45M, hourlyRateOverride: 5M));
			Assert.IsTrue((_currentH2ARate + 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: (_currentH2ARate + 1M), hourlyRateOverride: (_currentH2ARate + 2), isH2A: true, shiftDate: DateTime.Now));
		}

		[TestMethod]
		public void PayTypeCovid19_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRate()
		{
			// Employee Hourly Rate is greatest
			Assert.IsTrue(14M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: 12.45M, employeeHourlyRate: 14M, minimumWage: 10M));

			// Pay Line Hourly Rate is greatest
			Assert.IsTrue(14.75M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 10M));

			// Minimum Wage is Greatest
			Assert.IsTrue(15.00M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M));

			// H2A supercedes the comparison
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: (_currentH2ARate + 1), employeeHourlyRate: (_currentH2ARate + 2), minimumWage: (_currentH2ARate + 3), isH2A: true, shiftDate: DateTime.Now));
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: (_currentH2ARate + 3), employeeHourlyRate: (_currentH2ARate + 1), minimumWage: (_currentH2ARate + 2), isH2A: true, shiftDate: DateTime.Now));
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: (_currentH2ARate + 2), employeeHourlyRate: (_currentH2ARate + 3), minimumWage: (_currentH2ARate + 1), isH2A: true, shiftDate: DateTime.Now));

			// Hourly rate Override supercedes
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: (_currentH2ARate + 1), employeeHourlyRate: (_currentH2ARate + 2), minimumWage: (_currentH2ARate + 3), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: (_currentH2ARate + 3), employeeHourlyRate: (_currentH2ARate + 1), minimumWage: (_currentH2ARate + 2), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: (_currentH2ARate + 2), employeeHourlyRate: (_currentH2ARate + 3), minimumWage: (_currentH2ARate + 1), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
		}

		[TestMethod]
		public void PayTypeCovid19WageContinutation_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRate()
		{
			// Employee Hourly Rate is greatest
			Assert.IsTrue(14M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: 12.45M, employeeHourlyRate: 14M, minimumWage: 10M));

			// Pay Line Hourly Rate is greatest
			Assert.IsTrue(14.75M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 10M));

			// Minimum Wage is Greatest
			Assert.IsTrue(15.00M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M));

			// H2A supercedes the comparison
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: (_currentH2ARate + 1), employeeHourlyRate: (_currentH2ARate + 2), minimumWage: (_currentH2ARate + 3), isH2A: true, shiftDate: DateTime.Now));
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: (_currentH2ARate + 3), employeeHourlyRate: (_currentH2ARate + 1), minimumWage: (_currentH2ARate + 2), isH2A: true, shiftDate: DateTime.Now));
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: (_currentH2ARate + 2), employeeHourlyRate: (_currentH2ARate + 3), minimumWage: (_currentH2ARate + 1), isH2A: true, shiftDate: DateTime.Now));

			// Hourly rate Override supercedes
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: (_currentH2ARate + 1), employeeHourlyRate: (_currentH2ARate + 2), minimumWage: (_currentH2ARate + 3), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: (_currentH2ARate + 3), employeeHourlyRate: (_currentH2ARate + 1), minimumWage: (_currentH2ARate + 2), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: (_currentH2ARate + 2), employeeHourlyRate: (_currentH2ARate + 3), minimumWage: (_currentH2ARate + 1), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
		}

		[TestMethod]
		public void PayTypeCovid19W_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRate()
		{
			// Employee Hourly Rate is greatest
			Assert.IsTrue(14M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: 12.45M, employeeHourlyRate: 14M, minimumWage: 10M));

			// Pay Line Hourly Rate is greatest
			Assert.IsTrue(14.75M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 10M));

			// Minimum Wage is Greatest
			Assert.IsTrue(15.00M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M));

			// H2A supercedes the comparison
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: (_currentH2ARate + 1), employeeHourlyRate: (_currentH2ARate + 2), minimumWage: (_currentH2ARate + 3), isH2A: true, shiftDate: DateTime.Now));
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: (_currentH2ARate + 3), employeeHourlyRate: (_currentH2ARate + 1), minimumWage: (_currentH2ARate + 2), isH2A: true, shiftDate: DateTime.Now));
			Assert.IsTrue(_currentH2ARate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: (_currentH2ARate + 2), employeeHourlyRate: (_currentH2ARate + 3), minimumWage: (_currentH2ARate + 1), isH2A: true, shiftDate: DateTime.Now));

			// Hourly rate Override supercedes
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue(5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: 14.75M, employeeHourlyRate: 14M, minimumWage: 15M, hourlyRateOverride: 5M));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: (_currentH2ARate + 1), employeeHourlyRate: (_currentH2ARate + 2), minimumWage: (_currentH2ARate + 3), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: (_currentH2ARate + 3), employeeHourlyRate: (_currentH2ARate + 1), minimumWage: (_currentH2ARate + 2), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
			Assert.IsTrue((_currentH2ARate - 2) == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: (_currentH2ARate + 2), employeeHourlyRate: (_currentH2ARate + 3), minimumWage: (_currentH2ARate + 1), isH2A: true, hourlyRateOverride: (_currentH2ARate - 2), shiftDate: DateTime.Now));
		}

		[TestMethod]
		public void PayTypeCrewBossDaily_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBDaily));
		}

		[TestMethod]
		public void PayTypeCrewBossPerWorker_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBPerWorker));
		}

		[TestMethod]
		public void PayTypeCrewBossHourlyTrees_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBHourlyTrees));
		}

		[TestMethod]
		public void PayTypeCrewBossHourlyVines_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBHourlyVines));
		}

		[TestMethod]
		public void PayTypeCrewBossSouthDaily_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBSouthDaily));
		}

		[TestMethod]
		public void PayTypeCrewBossSouthHourly_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBSouthHourly));
		}

		[TestMethod]
		public void PayTypeCommission_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBCommission));
		}

		[TestMethod]
		public void PayTypeWeeklyOverTime_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.WeeklyOverTime));
		}

		[TestMethod]
		public void PayTypeAdjustment_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.Adjustment));
		}

		[TestMethod]
		public void PayTypeSpecialAdjustment_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.SpecialAdjustment));
		}

		[TestMethod]
		public void PayTypeCompTime_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.CompTime));
		}

		[TestMethod]
		public void PayTypeReportingPay_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.ReportingPay));
		}

		[TestMethod]
		public void PayTypeReportPayAdjustment_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.ReportingPayAdjustment));
		}

		[TestMethod]
		public void PayTypeBonus_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.Bonus));
		}

		[TestMethod]
		public void PayTypePremiumPay_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.PremiumPay));
		}
		#endregion

		#region Labor Code Tests

		[TestMethod]
		public void UnhandledLaborCode_ReturnsEmployeeHourlyRateCalc()
		{
			var laborCode = 0;

			// Employee hourly rate calc returns Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M));

			// Employee hourly rate calc returns Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

			// Employee hourly rate calc returns Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
		}

		[TestMethod]
		public void LaborCode_115_PackerNoPieces_L1_OnOrAfter20220418_ReturnsMinimum()
		{
			var startDate = new DateTime(2022, 4, 18);
			var endDate = startDate.AddYears(5);
			int laborCode = (int)PlantLaborCode.PackerNoPieces;
			string positionTitle = PositionTitle.PackerL1;

			// Override 
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));

			// Employee Hourly Rate is greater than Minimum
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));

			// Minimum is greater than Employee Hourly
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));

		}

		[TestMethod]
		public void LaborCode_115_PackerNoPieces_L2_OnOrAfter20220418_Returns15_5()
		{
			var startDate = new DateTime(2022, 4, 18);
			var endDate = startDate.AddYears(5);
			int laborCode = (int)PlantLaborCode.PackerNoPieces;
			string positionTitle = PositionTitle.PackerL2;

			// 15.5 starts on 4/18/2022
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate.AddDays(-1), positionTitle: positionTitle));

			// Override 
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));

			// Employee Hourly Rate is greater than 15.5
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.75M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.75M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.75M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.75M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.75M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.75M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));
			
			// Minimum Wage is grater than 15.5
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));

			// 15.5 rate only applies to Cutler
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate, positionTitle: positionTitle));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate, positionTitle: positionTitle));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 0M, minimumWage: 15M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate, positionTitle: positionTitle));
		}


		[TestMethod]
		public void LaborCode_125_Palletizing()
		{
			Rate125Tests((int)PlantLaborCode.Palletizing);
		}

		[TestMethod]
		public void LaborCode_151_FreshCut_Before20200518()
		{
			/*
				[Plant] = 2 => [EmployeeHourlyRateCalc] + 2. 
				ELSE [EmployeeHourlyRateCalc]
				
				The formula for [EmployeeHourlyRateCalc] is:                        
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2000, 1, 1);
			var endDate = new DateTime(2020, 5, 17);
			var laborCode = (int)PlantLaborCode.FreshCut;

			// Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Unknown, shiftDate: startDate));

			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Unknown, shiftDate: endDate));

			// Employee Hourly Rate
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: startDate));

			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: endDate));


			// Minimum Wage
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: startDate));

			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_151_FreshCut_OnOrAfter20200518()
		{
			/*
				[EmployeeHourlyRateCalc] + 2. 
			*/
			var startDate = new DateTime(2020, 5, 18);
			var endDate = startDate.AddYears(5);
			var laborCode = (int)PlantLaborCode.FreshCut;

			// Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Unknown, shiftDate: startDate));

			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Unknown, shiftDate: endDate));

			// Employee Hourly Rate
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: startDate));

			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: endDate));


			// Minimum Wage
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: startDate));

			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler, shiftDate: endDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Unknown, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_312_ReceivingAndMarkingGrapes()
		{
			// Returns the 125 rate (see LaborCode125()).
			Rate125Tests((int)PlantLaborCode.ReceivingAndMarkingGrapes);
		}

		[TestMethod]
		public void LaborCode_535_NightSanitation()
		{
			LaborCode_535_NightSanitation_Before20200302_Cutler_IgnoreH2A();
			LaborCode_535_NightSanitation_Before20200302_NonCutler_EnsuresMinimumH2ARate();
			LaborCode_535_NightSanitation_OnOrAfter20200302_Before20210322_Cutler_MaxOf_EmployeeHourlyRateCalc_MinimumPlusOne();
			LaborCode_535_NightSanitation_OnOrAfter20200302_Before20210322_NonCutler_MaxOf_EmployeeHourlyRateCalc_14_77();
			LaborCode_535_NightSanitation_OnOrAfter20210322_Before20220418();
			LaborCode_535_NightSanitation_OnOrAfter20220418();
		}

		[TestMethod]
		public void LaborCode_536_NightShiftSupervision_Before20201019()
		{
			/*
				Always returns 3 + [EmployeeHourlyRateCalc].  The formula for [EmployeeHourlyRateCalc] is:
						
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = (int)PlantLaborCode.NightShiftSupervision;
			var shiftDate = new DateTime(2020, 10, 18);

			// Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: shiftDate));

			// Employee Hourly Rate
			Assert.AreEqual(18.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: shiftDate));

			// Minimum Wage
			Assert.AreEqual(18.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: shiftDate));
		}

		[TestMethod]
		public void LaborCode_536_NightShiftSupervision_OnOrAfter20201019_Before20210322()
		{
			// Always returns the employee hourly rate
			var laborCode = (int)PlantLaborCode.NightShiftSupervision;
			var startDate = new DateTime(2020, 10, 19);
			var endDate = new DateTime(2021, 3, 21);

			// Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: endDate));

			// Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_536_NightShiftSupervision_OnOrAfter20210322()
		{
			/*
				Always returns 3 + [EmployeeHourlyRateCalc] when at plants 3, 4, and 11 from May 1 through October 15 
				otherwise returns [EmployeeHourlyRateClac].  The formula for [EmployeeHourlyRateCalc] is:
						
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = (int)PlantLaborCode.NightShiftSupervision;
			var startYear = 2021;

			/* Before May 1 or after October 15 is [EmployeeHourlyRateCalc] */
			// Override
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));


			// Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			// Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));


			/* Between May 1 and October 15 inclusive the rate for plants 3, 4, and 11 is the calculated employee hourly rate + 3. All other location return the calculated employee rate */
			// Override
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));

			// Employee Hourly Rate
			Assert.AreEqual(18.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(18.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(18.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(18.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(18.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(18.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			// Minimum Wage
			Assert.AreEqual(18.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(18.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(18.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(18.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(18.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(18.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));

		}

		[TestMethod]
		public void LaborCode_536_NightShiftSupervision_OnOrAfter20220418()
		{
			/*
				Always returns 2 + [EmployeeHourlyRateCalc] when at plants 2, 3, 4, and 11 from May 1 through October 15 
				otherwise returns [EmployeeHourlyRateClac].  The formula for [EmployeeHourlyRateCalc] is:
						
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = (int)PlantLaborCode.NightShiftSupervision;
			var startYear = 2022;

			/* Before May 1 or after October 15 is [EmployeeHourlyRateCalc] */
			// Override
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 16)));


			// Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			// Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 16)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 4, 30)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 16)));


			/* Between May 1 and October 15 inclusive the rate for plants 2, 3, 4, and 11 is the calculated employee hourly rate + 2. All other location return the calculated employee rate */
			// Override
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			// Employee Hourly Rate
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.5M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			// Minimum Wage
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(17.75M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Office, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear, 10, 15)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 5, 1)));
			Assert.AreEqual(15.75M, DefaultTest(plant: Plant.Unknown, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: new DateTime(startYear + 5, 10, 15)));

		}


		[TestMethod]
		public void LaborCode_602_Covid19PreScreening_Before20200427()
		{
			// Always returns the employee hourly rate
			var laborCode = (int)PlantLaborCode.Covid19PreScreening;
			var shiftDate = new DateTime(2020, 4, 26);

			// Employee hourly rate calc returns Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: shiftDate));

			// Employee hourly rate calc returns Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: shiftDate));

			// Employee hourly rate calc return Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: shiftDate));
		}

		[TestMethod]
		public void LaborCode_602_Covid19PreScreening_OnOrAfter20200427()
		{
			// Always returns 1.5 + [EmployeeHourlyRateCalc]
			var laborCode = (int)PlantLaborCode.Covid19PreScreening;
			var startDate = new DateTime(2020, 4, 27);
			var endDate = startDate.AddYears(5);

			// Employee hourly rate calc returns Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: endDate));

			// Employee hourly rate calc returns Employee Hourly Rate
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee hourly rate calc return Minimum Wage
			Assert.AreEqual(17.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(17.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_603_Covid19Sanitation_Before20200701()
		{
			// Always returns the employee hourly rate
			var laborCode = (int)PlantLaborCode.Covid19Sanitation;
			var shiftDate = new DateTime(2020, 6, 30);

			// Employee hourly rate calc returns Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: shiftDate));

			// Employee hourly rate calc returns Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: shiftDate));

			// Employee hourly rate calc return Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: shiftDate));
		}

		[TestMethod]
		public void LaborCode_603_Covid19Sanitation_OnOrAfter20200701()
		{
			// Always returns the greater of 14.5 and the calculated employee hourly rate
			var laborCode = (int)PlantLaborCode.Covid19Sanitation;
			var startDate = new DateTime(2020, 7, 1);
			var endDate = startDate.AddYears(5);

			// Employee hourly rate calc returns Override
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 13M, shiftDate: startDate));
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 13M, shiftDate: endDate));

			// 603 Rate is greater than Employee Hourly Rate and Minimum wage => return 603
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10M, minimumWage: 13M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10M, minimumWage: 14M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee hourly rate calc returns Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee hourly rate calc return Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_537_NightShiftAuditor()
		{
			/*
				Always returns 1.5 + [EmployeeHourlyRateCalc].  The formula for [EmployeeHourlyRateCalc] is:
						
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = (int)PlantLaborCode.NightShiftAuditor;

			// Override
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M));

			// Employee Hourly Rate
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

			// Minimum Wage
			Assert.AreEqual(17.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
		}

		[TestMethod]
		public void LaborCode_9503_Receiving_Break()
		{
			Rate503Tests((int)PlantLaborCode.Receiving_Break);
		}

		[TestMethod]
		public void LaborCode_503_ReceivingFreshFruit()
		{
			Rate503Tests((int)PlantLaborCode.ReceivingFreshFruit);
		}

		#endregion

		#region Helpers

		private void Rate125Tests(int laborCode)
		{
			Palletizer_Before20190527(laborCode);
			Palletizer_OnOrAfter20190527_Before20200302(laborCode);
			Palletizer_OnOrAfter20200302_Before20200518(laborCode);
			Palletizer_OnOrAfter20200518_Before20210322(laborCode);
			Palletizer_OnOrAfter20210322_Before20220418(laborCode);
			Palletizer_OnOrAfter20220418(laborCode);
		}

		private void Palletizer_Before20190527(int laborCode)
		{
			/* 
				[125 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12.5)
					[Shift Date] < #3/2/2020# => 
						[Plant] = 11 => [Minimum Wage] + 0.5
						ELSE Max([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => MAX([EmployeeHourlyRateCalc], [Minimum Wage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
					
					The formula for [EmployeeHourlyRateCalc] is:
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2000, 1, 1);
			var endDate = new DateTime(2019, 5, 26);

			// Minimum Wage and Employee Hourly Rate are below 12.5
			Assert.AreEqual(12.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(12.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is above 12.5
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is above 12.5
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 12.5
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is above 12.5
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Minimum Wage is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Employee Hourly Rate is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));
		}

		private void Palletizer_OnOrAfter20190527_Before20200302(int laborCode)
		{
			/* 
				[125 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12.5)
					[Shift Date] < #3/2/2020# => 
						[Plant] = 11 => [Minimum Wage] + 0.5
						ELSE Max([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => MAX([EmployeeHourlyRateCalc], [Minimum Wage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
					
					The formula for [EmployeeHourlyRateCalc] is:
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2019, 5, 27);
			var endDate = new DateTime(2020, 3, 1);

			// Minimum Wage and Employee Hourly Rate are below 13
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is above 13
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is above 13
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 13
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is above 13
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Minimum Wage is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Employee Hourly Rate is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));

			/* Cutler always returns minimum wage + .5 */
			// Minimum Wage and Employee Hourly Rate are below 13
			Assert.AreEqual(10.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above 13
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate is above 13
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Override, Minimum Wage, and Employee Hourly Rate are below 13
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is above 13
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void Palletizer_OnOrAfter20200302_Before20200518(int laborCode)
		{
			/* 
				[125 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12.5)
					[Shift Date] < #3/2/2020# => 
						[Plant] = 11 => [Minimum Wage] + 0.5
						ELSE Max([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => MAX([EmployeeHourlyRateCalc], [Minimum Wage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
					
					The formula for [EmployeeHourlyRateCalc] is:
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2020, 3, 2);
			var endDate = new DateTime(2020, 5, 17);

			/* All plants other than Cutler always return the greater of [Employee Hourly Rate] and 14.77 */
			// No override, employee and minimum below 14.77
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 14.77
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate));

			/* Cutler always returns the greater of [Employee Hourly Rate] and minimum wage + 1 */
			// No override, employee and minimum below 14.77
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above 14.77
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Override, Minimum Wage, and Employee Hourly Rate are below 14.77
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void Palletizer_OnOrAfter20200518_Before20210322(int laborCode)
		{
			/* 
				Plants 3 and 4 return the greater of [Employee Hourly Rate] and 14.77
				Plant 11 returns the greater of [Employee Hourly Rate] and 14.5
			*/
			var startDate = new DateTime(2020, 5, 18);
			var endDate = new DateTime(2021, 3, 21);

			/* All plants other than Cutler always return the greater of [Employee Hourly Rate] and 14.77 */
			// No override, employee and minimum below 14.77
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 14.77
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate));

			/* Cutler always returns the greater of [Employee Hourly Rate] and minimum wage + 1.5 */
			// No override, employee and minimum below 14.77
			Assert.AreEqual(11.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(11.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above 14.77
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Override, Minimum Wage, and Employee Hourly Rate are below 14.77
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void Palletizer_OnOrAfter20210322_Before20220418(int laborCode)
		{
			/* 
				Plants 3, 4, and 11 return the greater of [Employee Hourly Rate] and 16.05
			*/
			var startDate = new DateTime(2021, 3, 22);
			var endDate = new DateTime(2022, 4, 17);

			/* Plants 3, 4, and 11 always return the greater of [Employee Hourly Rate] and 16.05 */
			// No override, employee and minimum below 16.05
			Assert.AreEqual(16.05M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(16.05M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(16.05M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: endDate));

			// Minimum Wage is above 16.05
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 17M, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 17M, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 17M, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 17M, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 17M, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 17M, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: endDate));

			// Employee Hourly Rate is above 16.05
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 16.05
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Cutler, shiftDate: endDate));

			// Override is above 16.05
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Cutler, shiftDate: endDate));


			/* Locations other than 3, 4, and 11 return [Employee Hourly Rate] */
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: endDate));

		}

		private void Palletizer_OnOrAfter20220418(int laborCode)
		{
			/* 
				Plants 3, 4, and 11 return the greater of [Employee Hourly Rate] and minimum + 1.5
			*/
			var startDate = new DateTime(2022, 4, 18);
			var endDate = startDate.AddYears(5);

			// No override, employee below minimum + 1.5
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: endDate));

			// Employee Hourly Rate is above minimum + 1
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 15M, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 15M, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 15M, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 15M, hourlyRateOverride: 0, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 15M, hourlyRateOverride: 0, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 15M, hourlyRateOverride: 0, plant: Plant.Cutler, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, plant: Plant.Cutler, shiftDate: endDate));

			// Override is above minimum + 1.5
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Sanger, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Kerman, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Cutler, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Sanger, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Kerman, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 17M, plant: Plant.Cutler, shiftDate: endDate));


			/* Locations other than 3, 4, and 11 return [Employee Hourly Rate] */
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: endDate));

		}


		private void Rate503Tests(int laborCode)
		{
			BucketLoader_Before20190527(laborCode);
			BucketLoader_OnOrAfter20190527_Before20200302(laborCode);
			BucketLoader_OnOrAfter20200302_Before20210322(laborCode);
			BucketLoader_OnOrAfter20210322_Before20220418(laborCode);
			BucketLoader_OnOrAfter20220418(laborCode);
		}

		private void BucketLoader_Before20190527(int laborCode)
		{
			/* 
				[503 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12)
					[Shift Date] < #3/2/2020# =>  MAX([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => [EmployeeHourlyRateCalc]
						ELSE MAX([EmployeeHourlyRateCalc], Minimum + 1)
					
					The formula for [EmployeeHourlyRateCalc] is:
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2000, 1, 1);
			var endDate = new DateTime(2019, 5, 26);

			// Minimum Wage and Employee Hourly Rate are below 12
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is above 12
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is above 12
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 12
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is above 12
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Minimum Wage is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Employee Hourly Rate is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));
		}

		private void BucketLoader_OnOrAfter20190527_Before20200302(int laborCode)
		{
			/* 
				[503 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12)
					[Shift Date] < #3/2/2020# =>  MAX([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => [EmployeeHourlyRateCalc]
						ELSE MAX([EmployeeHourlyRateCalc], Minimum + 1)
					
					The formula for [EmployeeHourlyRateCalc] is:
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2019, 5, 27);
			var endDate = new DateTime(2020, 3, 1);

			// Minimum Wage and Employee Hourly Rate are below 13
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is above 13
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is above 13
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are below 13
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is above 13
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Minimum Wage is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));

			// Employee Hourly Rate is above Override
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate));
			Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate));
		}

		private void BucketLoader_OnOrAfter20200302_Before20210322(int laborCode)
		{
			/* 
				[503 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12)
					[Shift Date] < #3/2/2020# =>  MAX([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => [EmployeeHourlyRateCalc]
						ELSE MAX([EmployeeHourlyRateCalc], Minimum + 1)
					
					The formula for [EmployeeHourlyRateCalc] is:
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var startDate = new DateTime(2020, 3, 2);
			var endDate = new DateTime(2021, 3, 21);

			/* All plants other than Cutler always return Max of [EmployeeHourlyRateCalc] and minimum + 1 */
			// No override, employee and minimum equal
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is greater
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is greater than minimum +1
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is less than minimum +1
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14.5M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14.5M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is greatest
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate));

			// Override is less than minimum + 1
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14.75M, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14.75M, hourlyRateOverride: 15M, shiftDate: endDate));

			/* Cutler always returns [EmployeeHourlyRateCalc] */
			// No override, employee and minimum equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above greater
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate greater
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Override, Minimum Wage, and Employee Hourly Rate are equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is greater
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate, plant: Plant.Cutler));

			// Override is least
			Assert.AreEqual(8M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 8M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(8M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 8M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void BucketLoader_OnOrAfter20210322_Before20220418(int laborCode)
		{
			var startDate = new DateTime(2021, 3, 22);
			var endDate = new DateTime(2022, 4, 17);

			/* All plants other than Cutler always return Max of minimum wage, [EmployeeHourlyRateCalc],  15 */
			// No override, employee and minimum less than 15
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate));

			// Minimum Wage is greater and 15
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 16M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 16M, hourlyRateOverride: 0, shiftDate: endDate));

			// Employee Hourly Rate is greater than 15
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 16M, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 16M, hourlyRateOverride: 0, shiftDate: endDate));

			// Override, Minimum Wage, and Employee Hourly Rate are equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is greatest
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: endDate));

			// Override is less than employee hourly rate, minimum, and 15
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: endDate));

			/* Cutler always returns [EmployeeHourlyRateCalc] */
			// No override, employee and minimum equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above greater
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate greater
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 10M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));

			// Override, Minimum Wage, and Employee Hourly Rate are equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is greater
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate, plant: Plant.Cutler));

			// Override is least
			Assert.AreEqual(8M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 8M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(8M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 8M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void BucketLoader_OnOrAfter20220418(int laborCode)
		{
			var startDate = new DateTime(2022, 4, 18);
			var endDate = startDate.AddYears(5);

			/* All plants return max of minimum wage + 1, [EmployeeHourlyRateCalc] */
			// No override, employee rate less than minimum + 1
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Sanger));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Kerman));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Sanger));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 15, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Kerman));


			// Employee Hourly Rate is greater than minimum + 1
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Sanger));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: startDate, plant: Plant.Kerman));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Cutler));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Sanger));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 15M, hourlyRateOverride: 0, shiftDate: endDate, plant: Plant.Kerman));

			// Override, Minimum Wage, and Employee Hourly Rate are equal
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Sanger));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Kerman));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Sanger));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Kerman));

			// Override is greatest
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: startDate, plant: Plant.Sanger));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: startDate, plant: Plant.Kerman));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: endDate, plant: Plant.Cutler));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: endDate, plant: Plant.Sanger));
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 16M, shiftDate: endDate, plant: Plant.Kerman));

			// Override is less than employee hourly rate and minimum + 1
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: startDate, plant: Plant.Sanger));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: startDate, plant: Plant.Kerman));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: endDate, plant: Plant.Cutler));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: endDate, plant: Plant.Sanger));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 16.5M, minimumWage: 14.75M, hourlyRateOverride: 10M, shiftDate: endDate, plant: Plant.Kerman));

		}

		private void LaborCode_535_NightSanitation_Before20200302_Cutler_IgnoreH2A()
		{
			/*
				[535 Rate] = 
					[Shift Date] < #3-2-2020# =>
						[Plant]=11 => [EmployeeHourlyRateCalc]
						[EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
						ELSE [EmployeeHourlyRateCalc]
					ELSE
						[Plant]=11 => MAX([EmployeeHourlyRateCalc], [MinimumWage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
			*/
			var laborCode = (int)PlantLaborCode.NightSanitation;
			var plant = Plant.Cutler;
			var startDate = new DateTime(2000, 1, 1);
			var endDate = new DateTime(2020, 3, 1);

			// Override
			Assert.AreEqual(15M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, shiftDate: endDate));

			// Employee Hourly Rate
			Assert.AreEqual(15.5M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(15.75M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));
		}

		private void LaborCode_535_NightSanitation_Before20200302_NonCutler_EnsuresMinimumH2ARate()
		{
			/*
				[535 Rate] = 
					[Shift Date] < #3-2-2020# =>
						[Plant]=11 => [EmployeeHourlyRateCalc]
						[EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
						ELSE [EmployeeHourlyRateCalc]
					ELSE
						[Plant]=11 => MAX([EmployeeHourlyRateCalc], [MinimumWage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
			*/
			var laborCode = (int)PlantLaborCode.NightSanitation;
			var startDate = new DateTime(2000, 1, 1);
			var endDate = new DateTime(2020, 3, 1);
			var historicalH2ARate = 13.92M;

			// Override above H2A return Override
			Assert.AreEqual((historicalH2ARate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: (historicalH2ARate + 1.5M), hourlyRateOverride: (historicalH2ARate + .5M), shiftDate: startDate));
			Assert.AreEqual((historicalH2ARate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: (historicalH2ARate + 1.5M), hourlyRateOverride: (historicalH2ARate + .5M), shiftDate: endDate));

			// Override below H2A returns Override
			Assert.AreEqual((historicalH2ARate - 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: (historicalH2ARate + 1.5M), hourlyRateOverride: (historicalH2ARate - 1M), shiftDate: startDate));
			Assert.AreEqual((historicalH2ARate - 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: (historicalH2ARate + 1.5M), hourlyRateOverride: (historicalH2ARate - 1M), shiftDate: endDate));

			// No override and Employee Hourly Rate is max
			Assert.AreEqual((historicalH2ARate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: 0, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual((historicalH2ARate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: 0, hourlyRateOverride: 0, shiftDate: endDate));

			// No override and Minimum Wage is max
			Assert.AreEqual((historicalH2ARate + 1.5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: (historicalH2ARate + 1.5M), hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual((historicalH2ARate + 1.5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate + 1), minimumWage: (historicalH2ARate + 1.5M), hourlyRateOverride: 0, shiftDate: endDate));

			// No override and H2A Rate is max
			Assert.AreEqual((historicalH2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate - 1), minimumWage: (historicalH2ARate - .5M), hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual((historicalH2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (historicalH2ARate - 1), minimumWage: (historicalH2ARate - .5M), hourlyRateOverride: 0, shiftDate: endDate));
		}

		private void LaborCode_535_NightSanitation_OnOrAfter20200302_Before20210322_Cutler_MaxOf_EmployeeHourlyRateCalc_MinimumPlusOne()
		{
			/*
				[535 Rate] = 
					[Shift Date] < #3-2-2020# =>
						[Plant]=11 => [EmployeeHourlyRateCalc]
						[EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
						ELSE [EmployeeHourlyRateCalc]
					ELSE
						[Plant]=11 => MAX([EmployeeHourlyRateCalc], [MinimumWage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
			*/
			var laborCode = (int)PlantLaborCode.NightSanitation;
			var plant = Plant.Cutler;
			var startDate = new DateTime(2020, 3, 2);
			var endDate = new DateTime(2021, 3, 21);

			// Override greater than minimum + 1
			Assert.AreEqual(17M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));

			// Override less than minimum + 1
			Assert.AreEqual(16M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 16M, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 16M, shiftDate: endDate));

			// Employee Hourly Rate greater than minimum +1
			Assert.AreEqual(15.5M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee Hourly Rate less than minimum + 1
			Assert.AreEqual(16M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage greatest
			Assert.AreEqual(16.75M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16.75M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));
		}

		private void LaborCode_535_NightSanitation_OnOrAfter20200302_Before20210322_NonCutler_MaxOf_EmployeeHourlyRateCalc_14_77()
		{
			/*
				[535 Rate] = 
					[Shift Date] < #3-2-2020# =>
						[Plant]=11 => [EmployeeHourlyRateCalc]
						[EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
						ELSE [EmployeeHourlyRateCalc]
					ELSE
						[Plant]=11 => MAX([EmployeeHourlyRateCalc], [MinimumWage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
			*/
			var laborCode = (int)PlantLaborCode.NightSanitation;
			var startDate = new DateTime(2020, 3, 2);
			var endDate = new DateTime(2021, 3, 21);

			// Override greater than 14.77
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));

			// Override less than 14.77
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));

			// Employee Hourly Rate greater than 14.77
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee Hourly Rate less than 14.77
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 12.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 12.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage greater than 14.77
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage less than 14.77
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));
		}


		private void LaborCode_535_NightSanitation_OnOrAfter20210322_Before20220418()
		{

			var laborCode = (int)PlantLaborCode.NightSanitation;
			var startDate = new DateTime(2021, 3, 22);
			var endDate = new DateTime(2022, 4, 17);

			/* CUTLER is unchanged*/
			// Override greater than minimum + 1
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));

			// Override less than minimum + 1
			Assert.AreEqual(16M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 16M, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 16M, shiftDate: endDate));

			// Employee Hourly Rate greater than minimum +1
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(15.5M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee Hourly Rate less than minimum + 1
			Assert.AreEqual(16M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage greatest
			Assert.AreEqual(16.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16.75M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, shiftDate: endDate));


			/* Plants 3 and 4 return greater of the calculated employee hourly rate and 16.05*/

			// Override greater than 16.05
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));

			// Override less than 16.05
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));

			// Employee Hourly Rate greater than 16.05
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 17M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee Hourly Rate less than 16.05
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 12.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 12.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 12.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 12.5M, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage greater than 16.05
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 17M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 17M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 17M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(17M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 17M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage less than 16.05
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(16.05M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));

			/* Locations other than 3, 4, and 11 return [Employee Hourly Rate] */
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Reedley, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Reedley, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: endDate));
		}

		private void LaborCode_535_NightSanitation_OnOrAfter20220418()
		{

			var laborCode = (int)PlantLaborCode.NightSanitation;
			var startDate = new DateTime(2022, 4, 18);
			var endDate = startDate.AddYears(5);
			var rate = 17.51M;

			/* All plants return greater of the calculated employee hourly rate and */

			// Override greater than 17.51
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: rate + 1, shiftDate: endDate));

			// Override less than 17.51
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));

			// Employee Hourly Rate greater than 17.51
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: rate + 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Employee Hourly Rate less than 17.51
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: rate - 1, minimumWage: 10M, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage greater than 17.51
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate + 1, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: rate + 1, hourlyRateOverride: 0M, shiftDate: endDate));

			// Minimum Wage less than 17.51
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: startDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Sanger, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Kerman, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Cutler, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));
			Assert.AreEqual(rate, DefaultTest(plant: Plant.Reedley, laborCode: laborCode, employeeHourlyRate: 10.0M, minimumWage: 12.75M, hourlyRateOverride: 0M, shiftDate: endDate));

			/* Locations other than 3, 4, and 11 return [Employee Hourly Rate] */
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: startDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Office, shiftDate: endDate));
			Assert.AreEqual(10M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0, plant: Plant.Unknown, shiftDate: endDate));
		}

		#endregion
	}
}
