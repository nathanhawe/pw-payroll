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
            Plant plant = Domain.Constants.Plant.Sanger)
        {
            var wageService = new MockMinimumWageService();
            wageService.Test_AddMinimumWage(new DateTime(2000, 1, 1), minimumWage);
            var rateSelector = new PlantHourlyRateSelector(wageService);

            return _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, plant);
        }

        [TestMethod]
        public void H2A_ReturnsH2ARate()
        {
            var payType = Payroll.Domain.Constants.PayType.Regular;
            var laborCode = -1;
            var employeeHourlyRate = 0M;
            var hourlyRateOverride = 0M;
            var isH2A = true;

            var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger);
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
                var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger);
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

            var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger);
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

            var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger);
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
            var hourlyRate = hourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourlyRate, hourlyRateOverride, isH2A, Plant.Sanger);

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

            var hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payType, laborCode, employeeHourly, hourlyOverride, isH2A, Plant.Sanger);
            Assert.AreEqual(hourlyOverride, hourlyRate);
        }

        [TestMethod]
        public void NonH2A_NoSpecialLaborCode_NoHourlyRateOverride_ReturnsMaxOf_EmployeeHourlyRate_MinimumWage()
        {
            
            // Employee Hourly Rate is greater
            Assert.AreEqual(17.5M, DefaultTest(employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

            // Minimum Wage is greater
            Assert.AreEqual(17.75M, DefaultTest(employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
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
            /* 
                [125 Rate] = 
			        [Shift Date]<ToDate("5-27-2019") => If([EmployeeHourlyRateCalc]<12.5,12.5,[EmployeeHourlyRateCalc])
			        [H-2A]=true =>(If([Plant]=3,[H-2A Rate],If([EmployeeHourlyRateCalc]<13,13,[EmployeeHourlyRateCalc])))
			        [EmployeeHourlyRateCalc] < 13 => 13
			        ELSE [EmployeeHourlyRateCalc]

                    Because the way the Quick Base formula is written, [H-2A] will always be false when this is reached meaning that
                    the actual logic should be:

                        [Shift Date] < #5/27/2019# => MAX([EmployeeHourlyRateCalc], 12.5)
                        ELSE MAX([EmployeeHourlyRateCalc], 13)
                    
                    The formula for [EmployeeHourlyRateCalc] is:
                        
                        [Hourly Rate Override] > 0 => [Hourly Rate Override]
                        ELSE MAX([Employee Hourly Rate], [Minimum Wage])
            
            */
            var laborCode = 125;

            // Minimum Wage and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0));

            // Minimum Wage is above 13
            Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0));

            // Employee Hourly Rate is above 13
            Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0));

            // Override, Minimum Wage, and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10));

            // Override is above 13
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M));

            // Minimum Wage is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M));

            // Employee Hourly Rate is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M));
        }

        [TestMethod]
        public void LaborCode151()
        {
            /*
                Always returns 2 + [EmployeeHourlyRateCalc].  The formula for [EmployeeHourlyRateCalc] is:
                        
                        [Hourly Rate Override] > 0 => [Hourly Rate Override]
                        ELSE MAX([Employee Hourly Rate], [Minimum Wage])
            */
            var laborCode = 151;

            // Override
            Assert.AreEqual(17M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M));

            // Employee Hourly Rate
            Assert.AreEqual(17.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

            // Minimum Wage
            Assert.AreEqual(17.75M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
        }

        [TestMethod]
        public void LaborCode312()
        {
            // Returns the 125 rate (see LaborCode125()).
            var laborCode = 312;

            // Minimum Wage and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0));

            // Minimum Wage is above 13
            Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0));

            // Employee Hourly Rate is above 13
            Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0));

            // Override, Minimum Wage, and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10));

            // Override is above 13
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M));

            // Minimum Wage is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M));

            // Employee Hourly Rate is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M));
        }

        [TestMethod]
        public void LaborCode535_Cutler_IgnoreH2A()
        {
            /*
                [535 Rate] = 
			        [Plant]=11 => [EmployeeHourlyRateCalc]
			        [EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
			        ELSE [EmployeeHourlyRateCalc]
            */
            var laborCode = 535;
            var plant = Plant.Cutler;

            // Override
            Assert.AreEqual(15M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 15M));

            // Employee Hourly Rate
            Assert.AreEqual(15M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 10M, hourlyRateOverride: 0M));

            // Minimum Wage
            Assert.AreEqual(15M, DefaultTest(plant: plant, laborCode: laborCode, employeeHourlyRate: 15.5M, minimumWage: 15.75M, hourlyRateOverride: 0M));
        }

        [TestMethod]
        public void LaborCode535_NonCutler_EnsuresMinimumH2ARate()
        {
            /*
                [535 Rate] = 
			        [Plant]=11 => [EmployeeHourlyRateCalc]
			        [EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
			        ELSE [EmployeeHourlyRateCalc]
            */
            var laborCode = 535;

            // Override above H2A return Override
            Assert.AreEqual((_h2ARate + .5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: (_h2ARate + .5M)));

            // Override below H2A returns H2A
            Assert.AreEqual((_h2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: (_h2ARate - 1M)));

            // No override and Employee Hourly Rate is max
            Assert.AreEqual((_h2ARate + 1M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: 0, hourlyRateOverride: 0));

            // No override and Minimum Wage is max
            Assert.AreEqual((_h2ARate + 1.5M), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate + 1), minimumWage: (_h2ARate + 1.5M), hourlyRateOverride: 0));

            // No override and H2A Rate is max
            Assert.AreEqual((_h2ARate), DefaultTest(laborCode: laborCode, employeeHourlyRate: (_h2ARate - 1), minimumWage: (_h2ARate - .5M), hourlyRateOverride: 0));
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
            /* 
                This is effectively the same as 125 rate after 5/27/2019.
                [503 Rate] = 
			        [Shift Date]<ToDate("5-27-2019") => If([EmployeeHourlyRateCalc]<12,12,[EmployeeHourlyRateCalc])
                    If([EmployeeHourlyRateCalc]<13,13,[EmployeeHourlyRateCalc]))
		
                    The formula for [EmployeeHourlyRateCalc] is:
                        
                        [Hourly Rate Override] > 0 => [Hourly Rate Override]
                        ELSE MAX([Employee Hourly Rate], [Minimum Wage])
            
            */
            var laborCode = 9503;

            // Minimum Wage and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0));

            // Minimum Wage is above 13
            Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0));

            // Employee Hourly Rate is above 13
            Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0));

            // Override, Minimum Wage, and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10));

            // Override is above 13
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M));

            // Minimum Wage is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M));

            // Employee Hourly Rate is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M));
        }

        [TestMethod]
        public void LaborCode503()
        {
            /* 
                This is effectively the same as 125 rate after 5/27/2019.
                [503 Rate] = 
			        [Shift Date]<ToDate("5-27-2019") => If([EmployeeHourlyRateCalc]<12,12,[EmployeeHourlyRateCalc])
                    If([EmployeeHourlyRateCalc]<13,13,[EmployeeHourlyRateCalc]))
		
                    The formula for [EmployeeHourlyRateCalc] is:
                        
                        [Hourly Rate Override] > 0 => [Hourly Rate Override]
                        ELSE MAX([Employee Hourly Rate], [Minimum Wage])
            
            */
            var laborCode = 503;

            // Minimum Wage and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 0));

            // Minimum Wage is above 13
            Assert.AreEqual(14M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 0));

            // Employee Hourly Rate is above 13
            Assert.AreEqual(15M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 0));

            // Override, Minimum Wage, and Employee Hourly Rate are below 13
            Assert.AreEqual(13M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 10));

            // Override is above 13
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 10, hourlyRateOverride: 13.5M));

            // Minimum Wage is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 10, minimumWage: 14M, hourlyRateOverride: 13.5M));

            // Employee Hourly Rate is above Override
            Assert.AreEqual(13.5M, DefaultTest(laborCode: laborCode, employeeHourlyRate: 15M, minimumWage: 14M, hourlyRateOverride: 13.5M));
        }      

        #endregion
    }
}
