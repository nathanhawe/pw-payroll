﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Payroll.Data;
using Payroll.Domain.Constants;
using Payroll.Service;
using Payroll.Service.Interface;
using Payroll.UnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class RanchHourlyRateSelectorTests
	{
		private readonly decimal _crewLaborRate = 15.15M;
		private readonly decimal _culturalLaborRate = 14.5M;
		private readonly decimal _minimumWageRate = 14M;
		private ICrewLaborWageService _mockCrewLaborWageService;
		private ICulturalLaborWageService _mockCulturalLaborWageService;
		private RanchHourlyRateSelector _ranchHourlyRateSelector;
		private MockMinimumWageService _mockMinimumWageService;

		[TestInitialize]
		public void Setup()
		{
			// Create mocks of the wage selector services that return the same value across all dates.
			_mockCrewLaborWageService = new MockCrewLaborWageService(_crewLaborRate);
			_mockCulturalLaborWageService = new MockCulturalLaborWageService(_culturalLaborRate);


			// Create a mock of the minimum wage selector that always returns less than the crew labor rate
			_mockMinimumWageService = new MockMinimumWageService();
			_mockMinimumWageService.Test_AddMinimumWage(new DateTime(2000, 1, 1), _minimumWageRate);

			// Setup common instance of rate selector to test
			_ranchHourlyRateSelector = new RanchHourlyRateSelector(_mockCrewLaborWageService, _mockMinimumWageService, _mockCulturalLaborWageService);
		}

		private decimal DefaultTest(
			string payType = Payroll.Domain.Constants.PayType.Regular,
			int crew = 100,
			int laborCode = -1,
			decimal employeeHourlyRate = 14,
			decimal hourlyRateOverride = 0,
			DateTime? shiftDate = null,
			decimal payLineHourlyRate = 0,
			decimal minimumWage = 10)
		{
			shiftDate ??= new DateTime(2020, 1, 1);
			
			// Setup new services to mock the passed in minimum wage
			var minimumWageService = new MockMinimumWageService();
			minimumWageService.Test_AddMinimumWage(shiftDate.Value, minimumWage);
			var rateSelector = new RanchHourlyRateSelector(_mockCrewLaborWageService, minimumWageService, _mockCulturalLaborWageService);

			return rateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourlyRate, hourlyRateOverride, shiftDate.Value, payLineHourlyRate);
		}

		[TestMethod]
		public void HourlyRateOverride_ReturnsHourlyRateOverride()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var crew = 100;
			var laborCode = -1;
			var employeeHourly = 15;
			var hourlyOverride = 42M;
			var shiftDate = new DateTime(2020, 1, 1);

			var hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate, 0);
			Assert.AreEqual(hourlyOverride, hourlyRate);
		}

		[TestMethod]
		public void HourlyRateOverride_SupercedesLaborCodeAndCrew()
		{

			var payType = Payroll.Domain.Constants.PayType.Regular;
			var crew = 100;
			var laborCode = (int)RanchLaborCode.CrewHelper;
			var employeeHourly = 15;
			var hourlyOverride = 42;
			var shiftDate = new DateTime(2020, 1, 1);

			// Labor code 116 rate is currently set to 12 dollars but hourly rate override will ensure it is 42
			var hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate, 0);
			Assert.AreEqual(hourlyOverride, hourlyRate);

			// Crew 100 should receive the regular crew labor rate of 15 but hourly rate override will ensure it is 42
			laborCode = -1;
			hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate, 0);
			Assert.AreEqual(hourlyOverride, hourlyRate);
		}

		[TestMethod]
		public void HourlyRateOverride_SupercedesMinimumWage()
		{
			var payType = Payroll.Domain.Constants.PayType.Regular;
			var crew = 100;
			var laborCode = -1;
			var employeeHourly = 15;
			var hourlyOverride = _minimumWageRate - 1;
			var shiftDate = new DateTime(2020, 1, 1);

			var hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(payType, crew, laborCode, employeeHourly, hourlyOverride, shiftDate, 0);
			Assert.AreEqual(hourlyOverride, hourlyRate);
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
		public void PayTypeHourlyPlusPieces_ReturnsNonZero()
		{
			Assert.IsTrue(0 < DefaultTest(payType: Payroll.Domain.Constants.PayType.HourlyPlusPieces));
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
			Assert.IsTrue(15M == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: 15M));

			// Override still applies
			Assert.IsTrue(10.5M == DefaultTest(payType: Payroll.Domain.Constants.PayType.SickLeave, payLineHourlyRate: 15M, hourlyRateOverride: 10.5M));
		}

		[TestMethod]
		public void PayTypeCovid19_Before20210607_ReturnsGreaterOfPayLineHourlyRateAndCulturalRate()
		{
			var endDate = new DateTime(2021, 6, 6);
			// Pay line is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: endDate));

		}

		[TestMethod]
		public void PayTypeCovid19_Crew_OnOrAfter20210607_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRateAndCrewRate()
		{
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Pay line is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19_Cultural_OnOrAfter20210607_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRateAndCulturalRate()
		{
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Pay line is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate + 2M, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate + 2M, employeeHourlyRate: _culturalLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_culturalLaborRate == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, minimumWage: _culturalLaborRate + 2, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, minimumWage: _culturalLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_culturalLaborRate - 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate, hourlyRateOverride: _culturalLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate - 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate, hourlyRateOverride: _culturalLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19WageContinuation_Before20210607_ReturnsGreaterOfPayLineHourlyRateAndCulturalRate()
		{
			var endDate = new DateTime(2021, 6, 6);

			// Pay line is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19WageContinuation_Crew_OnOrAfter20210607_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRateAndCrewRate()
		{
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Pay line is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19WageContinuation_Cultural_OnOrAfter20210607_ReturnsGreaterOfPayLineHourlyRateAndEmployeeRateAndCulturalRate()
		{
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Pay line is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate + 2M, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate + 2M, employeeHourlyRate: _culturalLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_culturalLaborRate == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, minimumWage: _culturalLaborRate + 2, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, minimumWage: _culturalLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_culturalLaborRate - 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate, hourlyRateOverride: _culturalLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate - 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19WageContinuation, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate, hourlyRateOverride: _culturalLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19W_Before20210607_ReturnsGreaterOfPayLineHourlyRateAndCulturalRate()
		{
			var endDate = new DateTime(2021, 6, 6);

			// Pay line is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19W_Crew_OnOrAfter20210607_ReturnsGreaterOfPayLineHourlyRateEmployeeRateAndCrewRate()
		{
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Pay line is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate + 2M, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate + 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate - 2M, employeeHourlyRate: _crewLaborRate - 2M, minimumWage: _crewLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_crewLaborRate - 2M == DefaultTest(payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _crewLaborRate, employeeHourlyRate: _crewLaborRate, hourlyRateOverride: _crewLaborRate - 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void PayTypeCovid19W_Cultural_OnOrAfter20210607_ReturnsGreaterOfPayLineHourlyRateEmployeeRateAndCulturalRate()
		{
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Pay line is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate + 2M, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate + 2M, employeeHourlyRate: _culturalLaborRate, shiftDate: endDate));

			// Employee hourly rate is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate + 2M, shiftDate: endDate));

			// Crew labor rate is greater
			Assert.IsTrue(_culturalLaborRate == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, shiftDate: endDate));

			// Minimum wage is greater
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, minimumWage: _culturalLaborRate + 2, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate + 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate - 2M, employeeHourlyRate: _culturalLaborRate - 2M, minimumWage: _culturalLaborRate + 2, shiftDate: endDate));

			// Override still applies
			Assert.IsTrue(_culturalLaborRate - 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate, hourlyRateOverride: _culturalLaborRate - 2M, shiftDate: effectiveDate));
			Assert.IsTrue(_culturalLaborRate - 2M == DefaultTest(crew: 99, payType: Payroll.Domain.Constants.PayType.Covid19W, payLineHourlyRate: _culturalLaborRate, employeeHourlyRate: _culturalLaborRate, hourlyRateOverride: _culturalLaborRate - 2M, shiftDate: endDate));
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
		public void CBHeatRelatedSupplement_ReturnsZero()
		{
			Assert.IsTrue(0 == DefaultTest(payType: Payroll.Domain.Constants.PayType.CBHeatRelatedSupplement));
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
		public void LaborCode_103_AlmondHarvestEquipmentOperatorDay_Before20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay;
			// [Labor Code]=103 => If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],14.25)
			Assert.AreEqual(14.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: new DateTime(2021, 6, 6)));
			Assert.AreEqual(14.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: new DateTime(2021, 6, 6)));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: new DateTime(2021, 6, 6)));

			// Minimum Wage
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: 15M, shiftDate: new DateTime(2021, 6, 6)));
			Assert.AreEqual(_crewLaborRate + 2M , DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2, shiftDate: new DateTime(2021, 6, 6)));
		}

		[TestMethod]
		public void LaborCode_103_AlmondHarvestEquipmentOperatorDay_Cultural_OnOrAfter20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: endDate));

			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, minimumWage: _culturalLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, minimumWage: _culturalLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_103_AlmondHarvestEquipmentOperatorDay_Crew_OnOrAfter20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: _crewLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: _crewLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_104_AlmondHarvestEquipmentOperatorNight_Before20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight;
			var endDate = new DateTime(2021, 6, 6);

			// [LC104Rate] = If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate]+1,15.25)
			Assert.AreEqual(15.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(15.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(16M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: 16M, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 4, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 4, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_104_AlmondHarvestEquipmentOperatorNight_Cultural_OnOrAfter20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: endDate));

			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, minimumWage: _culturalLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, minimumWage: _culturalLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_104_AlmondHarvestEquipmentOperatorNight_Crew_OnOrAfter20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: _crewLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: _crewLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_105_AlmondHarvestGeneral_Before20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestGeneral;
			var endDate = new DateTime(2021, 6, 6);

			// [LC105Rate] = If([Crew]=65,If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],14),If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],[Crew Labor Rate]))
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate, minimumWage: 15M, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, crew: (int)Crew.AlmondHarvest_Nights, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, minimumWage: _crewLaborRate + 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_105_AlmondHarvestGeneral_Cultural_OnOrAfter20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestGeneral;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: endDate));

			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, minimumWage: _culturalLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, minimumWage: _culturalLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_105_AlmondHarvestGeneral_Crew_OnOrAfter20210607()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestGeneral;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));

			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: _crewLaborRate + 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, minimumWage: _crewLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_116_CrewHelper_Before20201207_IsGrapeHavest()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper;
			var effectiveDate = new DateTime(2020, 12, 6);

			// [LC116Rate] = 12
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate.AddYears(-5)));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate.AddYears(-5)));
			Assert.AreEqual(12M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate.AddYears(-5)));

			// Minimum Wage
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: 15M, shiftDate: effectiveDate));
			Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: 15M, shiftDate: effectiveDate.AddYears(-5)));
		}

		[TestMethod]
		public void LaborCode_116_CrewHelper_OnOrAfter20201207_Before20210607_IsCrewLaborRate()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper;
			var effectiveDate = new DateTime(2020, 12, 7);
			var endDate = new DateTime(2021, 6, 6);

			// [LC116Rate] = 12
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_116_CrewHelper_OnOrAfter20210607_Cultural_IsCulturalLaborRate()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// [LC116Rate] = 12
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, shiftDate: endDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_culturalLaborRate + 2M, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 2M, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_116_CrewHelper_OnOrAfter20210607_Crew_IsCrewLaborRate()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// [LC116Rate] = 12
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_117_CrewHelper_BonusRate_Before20201207_IsGrapeHavest()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper_BonusRate;
			var effectiveDate = new DateTime(2020, 12, 6);

			// [LC117Rate] = NULL
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate.AddYears(-5)));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate.AddYears(-5)));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate.AddYears(-5)));
		}

		[TestMethod]
		public void LaborCode_117_CrewHelper_BonusRate_OnOrAfter20201207_Before20210607_IsCrewLaborRate()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper_BonusRate;
			var effectiveDate = new DateTime(2020, 12, 7);
			var endDate = new DateTime(2021, 6, 6);

			// [LC116Rate] = 12
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_117_CrewHelper_BonusRate_OnOrAfter20210607_Crew_IsCrewLaborRate()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper_BonusRate;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// [LC116Rate] = 12
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_117_CrewHelper_BonusRate_OnOrAfter20210607_Cultural_IsCulturalLaborRate()
		{
			var laborCode = (int)RanchLaborCode.CrewHelper_BonusRate;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// [LC116Rate] = 12
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, shiftDate: endDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_culturalLaborRate + 2M, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 2M, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_120_GrapeHarvestSupport_Before20210607()
		{
			var laborCode = (int)RanchLaborCode.GrapeHarvestSupport;
			var endDate = new DateTime(2021, 6, 6);

			// [Labor Code] = 120 => [CulturalRate]
			// [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_120_GrapeHarvestSupport_OnOrAfter20210607_Crew_IsCrewLaborRate()
		{
			var laborCode = (int)RanchLaborCode.GrapeHarvestSupport;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// [Labor Code] = 120 => [CulturalRate]
			// [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_120_GrapeHarvestSupport_OnOrAfter20210607_Cultural_IsCulturalLaborRate()
		{
			var laborCode = (int)RanchLaborCode.GrapeHarvestSupport;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// [Labor Code] = 120 => [CulturalRate]
			// [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));

			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate, shiftDate: endDate));
			Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: endDate));

			// Minimum Wage
			Assert.AreEqual(_culturalLaborRate + 2, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 2, DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: _culturalLaborRate - 1, minimumWage: _culturalLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_215_Girdling_Before20200321_IsIgnored()
		{
			var laborCode = (int)RanchLaborCode.Girdling;
			var endDate = new DateTime(2020, 3, 20);
			// Crew 27 returns _crewLaborRate + .50 (15.50) in the first two instances and _crewLaborRate + 2 (17) in the last
			// since labor code 215 does not take precedence before 3/21/2020
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate, shiftDate: endDate));
			Assert.AreEqual(_crewLaborRate + 1.5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_215_Girdling_OnOrAfter20200321_Before20210208_Returns15_50()
		{
			var laborCode = (int)RanchLaborCode.Girdling;
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: new DateTime(2020, 3, 21)));
			Assert.AreEqual(15.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: new DateTime(2021, 2, 7)));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 1M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: new DateTime(2020, 3, 21), minimumWage: _crewLaborRate + 1M));
			Assert.AreEqual(_crewLaborRate + 1M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: new DateTime(2021, 2, 7), minimumWage: _crewLaborRate + 1M));
		}

		[TestMethod]
		public void LaborCode_215_Girdling_OnOrAfter20210208_ReturnsCrewLaborRatePlus1()
		{
			var laborCode = (int)RanchLaborCode.Girdling;
			var effectiveDate = new DateTime(2021, 2, 8);

			// Return crew labor rate + 1
			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + 3M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));

			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate.AddYears(10)));
			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: effectiveDate.AddYears(10)));
			Assert.AreEqual((_crewLaborRate + 3M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate.AddYears(10)));
		}

		[TestMethod]
		public void LaborCode_217_GraftingBuddingExpertCrew_Before20210111_Crew223_Returns15()
		{
			var laborCode = (int)RanchLaborCode.Grafting_BuddingExpertCrew;
			var crew = (int)Crew.JoseLuisRodriguez;
			var shiftDate = new DateTime(2021, 1, 10);

			// Crew 223 gets the grafting rate of 15 when using labor code 217, otherwise they get the crew labor rate.
			Assert.AreEqual(15, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: shiftDate));
			Assert.AreEqual(15, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: shiftDate));
			Assert.AreEqual(15, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: shiftDate));

			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate - 1, shiftDate: shiftDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate, shiftDate: shiftDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate + 1, shiftDate: shiftDate));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: shiftDate));
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(crew: crew, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: shiftDate));
		}

		[TestMethod]
		public void LaborCode_217_GraftingBuddingExpertCrew_Before20210111_NonCrew223()
		{
			var laborCode = (int)RanchLaborCode.Grafting_BuddingExpertCrew;
			var crew = 120;
			var shiftDate = new DateTime(2021, 1, 10);
			
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1, shiftDate: shiftDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate, shiftDate: shiftDate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: crew, laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1, shiftDate: shiftDate));
		}

		[TestMethod]
		public void LaborCode_217_GraftingBuddingExpertCrew_OnOrAfter20210111_Before20220314_ReturnsCrewLaborRatePlus1()
		{
			var laborCode = (int) RanchLaborCode.Grafting_BuddingExpertCrew;
			var effectiveDate = new DateTime(2021, 1, 11);
			var endDate = new DateTime(2022, 3, 13);

			// Return crew labor rate + 1
			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + 3M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));

			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: endDate));
			Assert.AreEqual((_crewLaborRate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 2M, shiftDate: endDate));
			Assert.AreEqual((_crewLaborRate + 3M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: _crewLaborRate + 2M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_217_GraftingBuddingExpertCrew_OnOrAfter20220314_Before20230101_Returns_16_25()
		{
			var laborCode = (int)RanchLaborCode.Grafting_BuddingExpertCrew;
			var effectiveDate = new DateTime(2022, 3, 14);
			var endDate = new DateTime(2022, 12, 31);

			// Return 16.25 unless minimum wage is higher
			Assert.AreEqual((16.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((16.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 18.25M, shiftDate: effectiveDate));
			Assert.AreEqual((17.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: 17.25M, shiftDate: effectiveDate));
			

			Assert.AreEqual((16.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: endDate));
			Assert.AreEqual((16.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 18.25M, shiftDate: endDate));
			Assert.AreEqual((17.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: 17.25M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_217_GraftingBuddingExpertCrew_OnOrAfter20230101_Returns_Min_Plus_1()
		{
			var laborCode = (int)RanchLaborCode.Grafting_BuddingExpertCrew;
			var effectiveDate = new DateTime(2023, 1, 1);
			var endDate = effectiveDate.AddYears(10);

			// Return minimum wage + 1 regardless of employee hourly rate or crew labor rate
			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: 10M, shiftDate: effectiveDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 18.25M, minimumWage: 15.5M, shiftDate: effectiveDate));
			Assert.AreEqual(18.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: 17.25M, shiftDate: effectiveDate));


			Assert.AreEqual(11M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: 10M, shiftDate: endDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 18.25M, minimumWage: 15.5M, shiftDate: endDate));
			Assert.AreEqual(18.25M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, minimumWage: 17.25M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_226_Chainsaw_OnOrAfter20221001()
		{
			var laborCode = (int)RanchLaborCode.Chainsaw;
			var effectiveDate = new DateTime(2022, 10, 1);

			// Returns the default rate before 10/1/2022
			Assert.AreEqual(_crewLaborRate, DefaultTest(laborCode: laborCode, shiftDate: effectiveDate.AddDays(-1)));

			// Returns the greater of employee hourly rate and crew labor rate plus 1 on or after 10/1/2022
			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, shiftDate: effectiveDate, employeeHourlyRate: _crewLaborRate + 1));

			Assert.AreEqual(_crewLaborRate + 1, DefaultTest(laborCode: laborCode, shiftDate: effectiveDate.AddYears(10)));
			Assert.AreEqual(_crewLaborRate + 2, DefaultTest(laborCode: laborCode, shiftDate: effectiveDate.AddYears(10), employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_234_SeasonalEquipmentOperator_OnOrAfter20220307_ReturnsGreaterOfEmployeeRateAndCulturalLaborPlusHalfDollar()
		{
			var laborCode = (int)RanchLaborCode.SeasonalEquipmentOperator;
			var effectiveDate = new DateTime(2022, 3, 7);
			var endDate = effectiveDate.AddYears(5);

			// Return cultural labor rate + .50 when it is greater than employee hourly rate
			Assert.AreEqual((_culturalLaborRate + .5M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_culturalLaborRate + .5M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: endDate));

			// Return the employee hourly rate when it is greater than cultural labor rate + .50
			Assert.AreEqual((_culturalLaborRate + .75M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .25M), shiftDate: effectiveDate));
			Assert.AreEqual((_culturalLaborRate + .75M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .25M), shiftDate: endDate));

			// Returns minimum wage + .50 when minimum wage is greater than employee and cultural labor rates
			Assert.AreEqual((_culturalLaborRate + 1.5M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .5M), shiftDate: effectiveDate, minimumWage: _culturalLaborRate + 1M));
			Assert.AreEqual((_culturalLaborRate + 1.5M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .5M), shiftDate: endDate, minimumWage: _culturalLaborRate + 1M));
		}

		[TestMethod]
		public void LaborCode_367_TransportationForklift_OnOrAfter20220523_Before20230522_CrewLabor_Returns1650()
		{
			var laborCode = (int)RanchLaborCode.TransportationForklift;
			var culturalCrew = (int)Crew.Trucking;
			var shiftDate = new DateTime(2022, 5, 23);
			var endDate = new DateTime(2023, 5, 21);

			// CREW gets 16.50 when operating forklifts during harvest

			// Override is less than 16.50 return 16.50
			Assert.AreEqual(15, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 15, shiftDate: shiftDate));
			Assert.AreEqual(15, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 15, shiftDate: endDate));

			// Employee Hourly Rate is less than 16.50 return 16.50
			Assert.AreEqual(16.5M , DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: shiftDate));
			Assert.AreEqual(16.5M , DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: endDate));

			// Employee Hourly Rate is greater than 16.50 return 16.50
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: shiftDate));
			Assert.AreEqual(16.5M, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: endDate));

			// Minimum wage is greater than 16.50, return minimum wage
			Assert.AreEqual(30M, DefaultTest(laborCode: laborCode, minimumWage: 30, hourlyRateOverride: 0, employeeHourlyRate: 8M, shiftDate: shiftDate));
			Assert.AreEqual(30M, DefaultTest(laborCode: laborCode, minimumWage: 30, hourlyRateOverride: 0, employeeHourlyRate: 8M, shiftDate: endDate));


			// CULTURAL unaffected by labor code
			// Employee Hourly Rate is less than 16.50, return max (EHR, cultural rate)
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: shiftDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: endDate));

			// Employee Hourly Rate is greater than 16.50, return max (EHR, cultural rate)
			Assert.AreEqual(30, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: shiftDate));
			Assert.AreEqual(30, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_367_TransportationForklift_OnOrAfter20230522_CrewLabor_Returns1675()
		{
			var laborCode = (int)RanchLaborCode.TransportationForklift;
			var culturalCrew = (int)Crew.Trucking;
			var shiftDate = new DateTime(2023, 5, 22);
			var endDate = shiftDate.AddYears(5);

			// CREW gets 16.75 when operating forklifts during harvest

			// Override is less than 16.75 return 16.75
			Assert.AreEqual(15, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 15, shiftDate: shiftDate));
			Assert.AreEqual(15, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 15, shiftDate: endDate));

			// Employee Hourly Rate is less than 16.75 return 16.75
			Assert.AreEqual(16.75M, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: shiftDate));
			Assert.AreEqual(16.75M, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: endDate));

			// Employee Hourly Rate is greater than 16.75 return 16.75
			Assert.AreEqual(16.75M, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: shiftDate));
			Assert.AreEqual(16.75M, DefaultTest(laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: endDate));

			// Minimum wage is greater than 16.75, return minimum wage
			Assert.AreEqual(30M, DefaultTest(laborCode: laborCode, minimumWage: 30, hourlyRateOverride: 0, employeeHourlyRate: 8M, shiftDate: shiftDate));
			Assert.AreEqual(30M, DefaultTest(laborCode: laborCode, minimumWage: 30, hourlyRateOverride: 0, employeeHourlyRate: 8M, shiftDate: endDate));


			// CULTURAL unaffected by labor code
			// Employee Hourly Rate is less than 16.75, return max (EHR, cultural rate)
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: shiftDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 10M, shiftDate: endDate));

			// Employee Hourly Rate is greater than 16.75, return max (EHR, cultural rate)
			Assert.AreEqual(30, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: shiftDate));
			Assert.AreEqual(30, DefaultTest(crew: culturalCrew, laborCode: laborCode, minimumWage: 8, hourlyRateOverride: 0, employeeHourlyRate: 30M, shiftDate: endDate));
		}

		[TestMethod]
		public void LaborCode_380_RecoveryTime_ReturnsZero()
		{
			var laborCode = (int)RanchLaborCode.RecoveryTime;
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_381_NonProductiveTime_ReturnsZero()
		{
			var laborCode = (int)RanchLaborCode.NonProductiveTime;
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(0M, DefaultTest(laborCode: laborCode, employeeHourlyRate: _crewLaborRate + 1));
		}

		[TestMethod]
		public void LaborCode_551_QualityControl_Before20200511_IsIgnored()
		{
			var laborCode = (int)RanchLaborCode.QualityControl;
			
			// Crew 27 returns _crewLaborRate + .50 (15.50) in the first two instances and _crewLaborRate + 2 (17) in the last
			// since labor code 551 does not take precedence before 5/11/2020
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1, shiftDate: new DateTime(2020, 5, 10)));
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate, shiftDate: new DateTime(2020, 5, 10)));
			Assert.AreEqual(_crewLaborRate + 1.5M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1, shiftDate: new DateTime(2020, 5, 10)));
		}

		[TestMethod]
		public void LaborCode_551_QualityControl_OnOrAfter20200511_Before20210607_ReturnsGreaterOfEmployeeRateAndCrewLaborPlusQuarterDollar()
		{
			var laborCode = (int)RanchLaborCode.QualityControl;
			var effectiveDate = new DateTime(2020, 5, 11);
			var endDate = new DateTime(2021, 6, 6);

			// Return crew labor rate + .25 when it is greater than employee hourly rate
			Assert.AreEqual((_crewLaborRate + .25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + .25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: endDate));

			// Return the employee hourly rate when it is greater than crew labor rate + .25
			Assert.AreEqual((_crewLaborRate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: endDate));

			// Returns minimum wage + .25 when minimum wage is greater than employee and crew labor rates
			Assert.AreEqual((_crewLaborRate + 1.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: effectiveDate, minimumWage: _crewLaborRate + 1M));
			Assert.AreEqual((_crewLaborRate + 1.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: endDate, minimumWage: _crewLaborRate + 1M));
		}

		[TestMethod]
		public void LaborCode_551_QualityControl_OnOrAfter20210607_Crew_ReturnsGreaterOfEmployeeRateAndCrewLaborPlusQuarterDollar()
		{
			var laborCode = (int)RanchLaborCode.QualityControl;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Return crew labor rate + .25 when it is greater than employee hourly rate
			Assert.AreEqual((_crewLaborRate + .25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + .25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: endDate));

			// Return the employee hourly rate when it is greater than crew labor rate + .25
			Assert.AreEqual((_crewLaborRate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: effectiveDate));
			Assert.AreEqual((_crewLaborRate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: endDate));

			// Returns minimum wage + .25 when minimum wage is greater than employee and crew labor rates
			Assert.AreEqual((_crewLaborRate + 1.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: effectiveDate, minimumWage: _crewLaborRate + 1M));
			Assert.AreEqual((_crewLaborRate + 1.25M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_crewLaborRate + .5M), shiftDate: endDate, minimumWage: _crewLaborRate + 1M));
		}

		[TestMethod]
		public void LaborCode_551_QualityControl_OnOrAfter20210607_Cultural_ReturnsGreaterOfEmployeeRateAndCulturalLaborPlusQuarterDollar()
		{
			var laborCode = (int)RanchLaborCode.QualityControl;
			var effectiveDate = new DateTime(2021, 6, 7);
			var endDate = effectiveDate.AddYears(5);

			// Return cultural labor rate + .25 when it is greater than employee hourly rate
			Assert.AreEqual((_culturalLaborRate + .25M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: effectiveDate));
			Assert.AreEqual((_culturalLaborRate + .25M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: 8M, shiftDate: endDate));

			// Return the employee hourly rate when it is greater than cultural labor rate + .25
			Assert.AreEqual((_culturalLaborRate + .5M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .5M), shiftDate: effectiveDate));
			Assert.AreEqual((_culturalLaborRate + .5M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .5M), shiftDate: endDate));

			// Returns minimum wage + .25 when minimum wage is greater than employee and cultural labor rates
			Assert.AreEqual((_culturalLaborRate + 1.25M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .5M), shiftDate: effectiveDate, minimumWage: _culturalLaborRate + 1M));
			Assert.AreEqual((_culturalLaborRate + 1.25M), DefaultTest(crew: 99, laborCode: laborCode, employeeHourlyRate: (_culturalLaborRate + .5M), shiftDate: endDate, minimumWage: _culturalLaborRate + 1M));
		}

		[TestMethod]
		public void LaborCode_SupercedesCrew()
		{
			var laborCode = (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight;
			var shiftDate = new DateTime(2021, 6, 7);

			// Crew 27 would return _culturalLaborRate + .50 in the first two instances and _crewLaborRate + 1.5 (17.50) in the last
			// but labor code 104 takes precedence
			Assert.AreEqual(_culturalLaborRate, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: shiftDate));
			Assert.AreEqual(_culturalLaborRate, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate, shiftDate: shiftDate));
			Assert.AreEqual(_culturalLaborRate + 1M, DefaultTest(laborCode: laborCode, crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: shiftDate));
		}

		#endregion

		#region Crew Tests

		[TestMethod]
		public void CrewLabor()
		{
			// Crews over 100 always get the crew labor rate unless minimum wage is higher.
			for (int i = 101; i < 1000; i++)
			{
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate - 1));
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate));
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate + 1));
				Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M));
			}
		}

		[TestMethod]
		public void CulturalLabor_Before20210607()
		{
			// Crews that are less than 100 and don't otherwise have exceptions (27, 75, and 76) receive
			// the cultural rate.
			// [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
			var effectiveDate = new DateTime(2021, 6, 6);
			List<int> exceptionCrews = new List<int> { (int)Crew.WestTractor_Night, (int)Crew.SouthTractor_Night, (int)Crew.EastTractor_Night, (int)Crew.LightDuty_East, (int)Crew.LightDuty_West };
			for (int i = 1; i < 100; i++)
			{
				if (exceptionCrews.Contains(i)) continue;
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
				Assert.AreEqual(_crewLaborRate, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
				Assert.AreEqual(_crewLaborRate + 1, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));
				Assert.AreEqual(_crewLaborRate + 2, DefaultTest(crew: i, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
			}
		}

		[TestMethod]
		public void CulturalLabor_OnOrAfter20210607()
		{
			// Crews that are less than 100 and don't otherwise have exceptions (27, 75, and 76) receive
			// the cultural rate.
			// [CulturalRate] = If([Employee Hourly Rate]<[Cultural Labor Rate],[Cultural Labor Rate],[Employee Hourly Rate])
			var effectiveDate = new DateTime(2021, 6, 7);
			List<int> exceptionCrews = new List<int> { (int)Crew.WestTractor_Night, (int)Crew.SouthTractor_Night, (int)Crew.EastTractor_Night, (int)Crew.LightDuty_East, (int)Crew.LightDuty_West };
			for (int i = 1; i < 100; i++)
			{
				if (exceptionCrews.Contains(i)) continue;
				Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: i, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
				Assert.AreEqual(_culturalLaborRate, DefaultTest(crew: i, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
				Assert.AreEqual(_culturalLaborRate + 1, DefaultTest(crew: i, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));
				Assert.AreEqual(_culturalLaborRate + 2, DefaultTest(crew: i, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
			}
		}

		[TestMethod]
		public void Crew_27_WestTractor_Night_Before20210607()
		{
			var effectiveDate = new DateTime(2021, 6, 6);

			// [CulturalRate] + .5
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + .5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_crewLaborRate + 1.5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1, shiftDate: effectiveDate));

			// Minimum Wage + .5
			Assert.AreEqual(_crewLaborRate + 2.5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M, shiftDate: effectiveDate));
		}

		[TestMethod]
		public void Crew_27_WestTractor_Night_OnOrAfter20210607()
		{
			var effectiveDate = new DateTime(2021, 6, 7);

			// [CulturalRate] + .5
			Assert.AreEqual(_culturalLaborRate + .5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + .5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1.5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));

			// Minimum Wage + .5
			Assert.AreEqual(_culturalLaborRate + 2.5M, DefaultTest(crew: (int)Crew.WestTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
		}

		[TestMethod]
		public void Crew_24_SouthEquipmentOperator_Night_OnOrAfter20220404()
		{
			var effectiveDate = new DateTime(2022, 4, 4);

			// [CulturalRate] + .5
			Assert.AreEqual(_culturalLaborRate + .5M, DefaultTest(crew: (int)Crew.SouthTractor_Night, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + .5M, DefaultTest(crew: (int)Crew.SouthTractor_Night, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1.5M, DefaultTest(crew: (int)Crew.SouthTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));

			// Minimum Wage + .5
			Assert.AreEqual(_culturalLaborRate + 2.5M, DefaultTest(crew: (int)Crew.SouthTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
		}

		[TestMethod]
		public void Crew_21_EastTractor_Night_OnOrAfter20220404()
		{
			var effectiveDate = new DateTime(2022, 4, 4);

			// [CulturalRate] + .5
			Assert.AreEqual(_culturalLaborRate + .5M, DefaultTest(crew: (int)Crew.EastTractor_Night, employeeHourlyRate: _culturalLaborRate - 1, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + .5M, DefaultTest(crew: (int)Crew.EastTractor_Night, employeeHourlyRate: _culturalLaborRate, shiftDate: effectiveDate));
			Assert.AreEqual(_culturalLaborRate + 1.5M, DefaultTest(crew: (int)Crew.EastTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, shiftDate: effectiveDate));

			// Minimum Wage + .5
			Assert.AreEqual(_culturalLaborRate + 2.5M, DefaultTest(crew: (int)Crew.EastTractor_Night, employeeHourlyRate: _culturalLaborRate + 1, minimumWage: _culturalLaborRate + 2M, shiftDate: effectiveDate));
		}

		[TestMethod]
		public void Crew_75_LightDuty_East()
		{
			// Crew 75 always get the crew labor rate.
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate + 1));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(crew: (int)Crew.LightDuty_East, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M));
		}

		[TestMethod]
		public void Crew_76_LightDutyWest()
		{
			// Crew 76 always get the crew labor rate.
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate - 1));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate));
			Assert.AreEqual(_crewLaborRate, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate + 1));

			// Minimum Wage
			Assert.AreEqual(_crewLaborRate + 2M, DefaultTest(crew: (int)Crew.LightDuty_West, employeeHourlyRate: _crewLaborRate + 1, minimumWage: _crewLaborRate + 2M));
		}

		#endregion
	}
}
