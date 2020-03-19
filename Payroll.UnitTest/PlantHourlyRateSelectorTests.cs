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
		private readonly decimal _h2ARate = 13.92M;

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
			DateTime? shiftDate = null)
		{
			shiftDate ??= new DateTime(2020, 3, 11);
			var wageService = new MockMinimumWageService();
			wageService.Test_AddMinimumWage(shiftDate.Value, minimumWage);
			var rateSelector = new PlantHourlyRateSelector(wageService);

			return rateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, plant, shiftDate.Value);
		}

		[TestMethod]
		public void H2A_ReturnsH2ARate()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = 0M;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, new DateTime(2020, 3, 11));
			Assert.AreEqual(_h2ARate, hourlyRate);
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
				var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, new DateTime(2020, 3, 11));
				Assert.AreEqual(_h2ARate, hourlyRate);
			}
		}

		[TestMethod]
		public void H2A_SupercedesEmployeeRate()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = _h2ARate + 10;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, new DateTime(2020, 3, 11));
			Assert.AreEqual(_h2ARate, hourlyRate);
		}

		[TestMethod]
		public void H2A_SupercedesHourlyRateOverride()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourlyRate = _h2ARate + 10;
			var hourlyRateOverride = 0M;
			var isH2A = true;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, new DateTime(2020, 3, 11));
			Assert.AreEqual(_h2ARate, hourlyRate);
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
			wageSelector.Test_AddMinimumWage(new DateTime(2000, 1, 1), _h2ARate + 10);
			var hourlyRateSelector = new PlantHourlyRateSelector(wageSelector);
			var hourlyRate = hourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger, new DateTime(2020, 3, 11));

			Assert.AreEqual(_h2ARate, hourlyRate);
		}

		[TestMethod]
		public void NonH2A_NoSpecialLaborCode_HourlyRateOverride_ReturnsHourlyRateOverride()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var laborCode = -1;
			var employeeHourly = 15;
			var hourlyOverride = 42;
			var isH2A = false;

			var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourly, hourlyOverride, isH2A, Plant.Sanger, new DateTime(2020, 3, 11));
			Assert.AreEqual(hourlyOverride, hourlyRate);
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
		public void PayTypeProductivityOnlyPieces_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.ProductivityOnlyPieces));
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
		public void PayTypeSickLeave_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave));
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
		#endregion

		#region Labor Code Tests

		[TestMethod]
		public void LaborCode125()
		{
			Rate125Tests(125);
		}

		[TestMethod]
		public void LaborCode151()
		{
			/*
				[Plant] = 2 => [EmployeeHourlyRateCalc] + 2. 
				ELSE [EmployeeHourlyRateCalc]
				
				The formula for [EmployeeHourlyRateCalc] is:                        
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = 151;

			// Override
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Reedley));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Kerman));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Office));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Sanger));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M, plant: Plant.Unknown));

			// Employee Hourly Rate
			Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Reedley));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Cutler));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Kerman));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Office));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Sanger));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M, plant: Plant.Unknown));


			// Minimum Wage
			Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Reedley));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Cutler));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Kerman));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Office));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Sanger));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M, plant: Plant.Unknown));
		}

		[TestMethod]
		public void LaborCode312()
		{
			// Returns the 125 rate (see LaborCode125()).
			Rate125Tests(312);
		}

		[TestMethod]
		public void LaborCode535_Before20200302_Cutler_IgnoreH2A()
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
			var laborCode = 535;
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

		[TestMethod]
		public void LaborCode535_Before20200302_NonCutler_EnsuresMinimumH2ARate()
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
			var laborCode = 535;
			var startDate = new DateTime(2000, 1, 1);
			var endDate = new DateTime(2020, 3, 1);

			// Override above H2A return Override
			Assert.AreEqual((_h2ARate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: (_h2ARate + .5M), shiftDate: startDate));
			Assert.AreEqual((_h2ARate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: (_h2ARate + .5M), shiftDate: endDate));

			// Override below H2A returns H2A
			Assert.AreEqual((_h2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: (_h2ARate - 1M), shiftDate: startDate));
			Assert.AreEqual((_h2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: (_h2ARate - 1M), shiftDate: endDate));

			// No override and Employee Hourly Rate is max
			Assert.AreEqual((_h2ARate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: 0, hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual((_h2ARate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: 0, hourlyRateOverride: 0, shiftDate: endDate));

			// No override and Minimum Wage is max
			Assert.AreEqual((_h2ARate + 1.5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual((_h2ARate + 1.5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: 0, shiftDate: endDate));

			// No override and H2A Rate is max
			Assert.AreEqual((_h2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate - 1), minimumWage: (_h2ARate - .5M), hourlyRateOverride: 0, shiftDate: startDate));
			Assert.AreEqual((_h2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate - 1), minimumWage: (_h2ARate - .5M), hourlyRateOverride: 0, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode535_OnOrAfter20200302_Cutler_MaxOf_EmployeeHourlyRateCalc_MinimumPlusOne()
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
			var laborCode = 535;
			var plant = Plant.Cutler;
			var startDate = new DateTime(2020, 3, 2);
			var endDate = startDate.AddYears(5);

			// Override greater than minimum + 1
			Assert.AreEqual(17M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));

			// Override less than minimum + 1
			Assert.AreEqual(16.75M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 16M, shiftDate: startDate));
			Assert.AreEqual(16.75M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 16M, shiftDate: endDate));

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

		[TestMethod]
		public void LaborCode535_OnOrAfter20200302_NonCutler_MaxOf_EmployeeHourlyRateCalc_14_77()
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
			var laborCode = 535;
			var startDate = new DateTime(2020, 3, 2);
			var endDate = startDate.AddYears(5);

			// Override greater than 14.77
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: startDate));
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 17M, shiftDate: endDate));

			// Override less than 14.77
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: startDate));
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 10M, shiftDate: endDate));

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


		[TestMethod]
		public void LaborCode536()
		{
			/*
				Always returns 3 + [EmployeeHourlyRateCalc].  The formula for [EmployeeHourlyRateCalc] is:
						
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = 536;

			// Override
			Assert.AreEqual(18M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M));

			// Employee Hourly Rate
			Assert.AreEqual(18.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

			// Minimum Wage
			Assert.AreEqual(18.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
		}

		[TestMethod]
		public void LaborCode537()
		{
			/*
				Always returns 1.5 + [EmployeeHourlyRateCalc].  The formula for [EmployeeHourlyRateCalc] is:
						
						[Hourly Rate Override] > 0 => [Hourly Rate Override]
						ELSE MAX([Employee Hourly Rate], [Minimum Wage])
			*/
			var laborCode = 537;

			// Override
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M));

			// Employee Hourly Rate
			Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

			// Minimum Wage
			Assert.AreEqual(17.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
		}

		[TestMethod]
		public void LaborCode9503()
		{
			Rate503Tests(9503);
		}

		[TestMethod]
		public void LaborCode503()
		{
			Rate503Tests(503);
		}

		#endregion

		#region Helpers

		private void Rate125Tests(int laborCode)
		{
			Palletizer_Before20190527(laborCode);
			Palletizer_OnOrAfter20190527_Before20200302(laborCode);
			Palletizer_OnOrAfter20200302(laborCode);
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
			Assert.AreEqual(12.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(12.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

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
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

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
			Assert.AreEqual(10.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is above 13
			Assert.AreEqual(10.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(10.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M, shiftDate: endDate, plant: Plant.Cutler));

			// Minimum Wage is above Override
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate, plant: Plant.Cutler));

			// Employee Hourly Rate is above Override
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(14.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void Palletizer_OnOrAfter20200302(int laborCode)
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
			var endDate = startDate.AddYears(5);

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
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(14.77M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

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
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate, plant: Plant.Cutler));

			// Override is above 14.77
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate, plant: Plant.Cutler));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate, plant: Plant.Cutler));
		}

		private void Rate503Tests(int laborCode)
		{
			BucketLoader_Before20190527(laborCode);
			BucketLoader_OnOrAfter20190527_Before20200302(laborCode);
			BucketLoader_OnOrAfter20200302(laborCode);
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
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

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
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

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

		private void BucketLoader_OnOrAfter20200302(int laborCode)
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
			var endDate = startDate.AddYears(5);

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
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: startDate));
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10, shiftDate: endDate));

			// Override is greatest
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 15M, shiftDate: endDate));

			// Override is less than minimum + 1
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14.75M, hourlyRateOverride: 15M, shiftDate: startDate));
			Assert.AreEqual(15.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14.75M, hourlyRateOverride: 15M, shiftDate: endDate));

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

		#endregion
	}
}
