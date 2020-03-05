using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    [TestClass]
    public class GrossFromIncentiveCalculatorTests
    {
        private class GrossFromIncentiveTest
        {
            public int Id { get; set; }
            public int LaborCode { get; set; }
            public decimal HoursWorked { get; set; }
            public decimal Pieces { get; set; }
            public decimal IncreasedRate { get; set; }
            public decimal PrimaRate { get; set; }
            public decimal NonPrimaRate { get; set; }
            public decimal ExpectedGross { get; set; }
        }

        private readonly GrossFromIncentiveCalculator _grossFromIncentiveCalculator = new GrossFromIncentiveCalculator();
        
        [TestMethod]
        public void LaborCode555_Incentive_ReturnsIncentive()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 16M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .5M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 40M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach(var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void LaborCode555_IncentiveDisqualified_ReturnsZero()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                isIncentiveDisqualified: true,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void LaborCode555_Incentive_IgnoresNonPrima()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 16M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .5M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 40M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: true)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }
        

        [TestMethod]
        public void UseIncreasedRate_ReturnsIncreasedRateIncentive()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 86M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 1.66M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 41.5M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 62.25M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: true,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void UseIncreasedRate_NonPrima_ReturnsZero()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: true,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: true)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void UseIncreasedRate_IgnoresIsIncentiveDisqualified()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 86M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 1.66M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 41.5M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 62.25M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: true,
                isIncentiveDisqualified: true,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }



        [TestMethod]
        public void NonIncreasedRate_ReturnsPrimaRateIncentive()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 75M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 1.5M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 37.5M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 56.25M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: false,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void NonIncreasedRate_NonPrima_ReturnsZero()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: false,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: true)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void NonIncreasedRate_IgnoresIsIncentiveDisqualified()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 75M},
                new GrossFromIncentiveTest { Id = 2, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 1.5M},
                new GrossFromIncentiveTest { Id = 3, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 37.5M},
                new GrossFromIncentiveTest { Id = 4, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 56.25M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: false,
                isIncentiveDisqualified: true,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void BonusPieces_RoundUpAtMidPoint()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, Pieces = 100, PrimaRate = .11115M, ExpectedGross = 11.12M},
                new GrossFromIncentiveTest { Id = 2, Pieces = 100, PrimaRate = .11116M, ExpectedGross = 11.12M},
                new GrossFromIncentiveTest { Id = 3, Pieces = 100, PrimaRate = .11117M, ExpectedGross = 11.12M},
                new GrossFromIncentiveTest { Id = 4, Pieces = 100, PrimaRate = .11118M, ExpectedGross = 11.12M},
                new GrossFromIncentiveTest { Id = 5, Pieces = 100, PrimaRate = .11119M, ExpectedGross = 11.12M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: false,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }

        [TestMethod]
        public void BonusPieces_RoundDownBelowMidPoint()
        {
            var tests = new List<GrossFromIncentiveTest>
            {
                new GrossFromIncentiveTest { Id = 1, Pieces = 100, PrimaRate = .11114M, ExpectedGross = 11.11M},
                new GrossFromIncentiveTest { Id = 2, Pieces = 100, PrimaRate = .11113M, ExpectedGross = 11.11M},
                new GrossFromIncentiveTest { Id = 3, Pieces = 100, PrimaRate = .11112M, ExpectedGross = 11.11M},
                new GrossFromIncentiveTest { Id = 4, Pieces = 100, PrimaRate = .11111M, ExpectedGross = 11.11M},
                new GrossFromIncentiveTest { Id = 5, Pieces = 100, PrimaRate = .11110M, ExpectedGross = 11.11M},
            };

            var testLines = tests.Select(x => Helper.MockPlantPayLine(
                id: x.Id,
                hoursWorked: x.HoursWorked,
                pieces: x.Pieces,
                increasedRate: x.IncreasedRate,
                primaRate: x.PrimaRate,
                nonPrimaRate: x.NonPrimaRate,
                useIncreasedRate: false,
                isIncentiveDisqualified: false,
                hasNonPrimaViolation: false)).ToList();

            _grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
            }
        }
    }
}
