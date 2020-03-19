using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class GrossFromIncentiveCalculatorTests
	{
		private readonly RoundingService _roundingService = new RoundingService();
		private GrossFromIncentiveCalculator _grossFromIncentiveCalculator;

		[TestInitialize]
		public void Setup()
		{
			_grossFromIncentiveCalculator = new GrossFromIncentiveCalculator(_roundingService);
		}


		[TestMethod]
		public void LaborCode555_Incentive_ReturnsIncentive()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  555, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  555, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  555, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  555, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 40M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void LaborCode555_IncentiveDisqualified_ReturnsZero()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  555, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  555, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  555, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  555, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				isIncentiveDisqualified: true,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void LaborCode555_Incentive_IgnoresNonPrima()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  555, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  555, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  555, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  555, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 40M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: true,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}


		[TestMethod]
		public void UseIncreasedRate_ReturnsIncreasedRateIncentive()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 15M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .3M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 11.25M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: true,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void UseIncreasedRate_NonPrima_ReturnsZero()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: true,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: true,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void UseIncreasedRate_IgnoresIsIncentiveDisqualified()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 15M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .3M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 11.25M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: true,
				isIncentiveDisqualified: true,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}



		[TestMethod]
		public void NonIncreasedRate_ReturnsPrimaRateIncentive()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .14M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 3.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 5.25M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: false,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void NonIncreasedRate_NonPrima_ReturnsZero()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: false,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: true,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void NonIncreasedRate_IgnoresIsIncentiveDisqualified()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .14M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 3.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 5.25M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: false,
				isIncentiveDisqualified: true,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void BonusPieces_RoundUpAtMidPoint()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, Pieces = 100, PrimaRate = .11115M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, Pieces = 100, PrimaRate = .11116M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, Pieces = 100, PrimaRate = .11117M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, Pieces = 100, PrimaRate = .11118M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 5, LaborCode = 123, Pieces = 100, PrimaRate = .11119M, ExpectedGross = 11.12M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: false,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void BonusPieces_RoundDownBelowMidPoint()
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = 123, Pieces = 100, PrimaRate = .11114M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = 123, Pieces = 100, PrimaRate = .11113M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = 123, Pieces = 100, PrimaRate = .11112M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = 123, Pieces = 100, PrimaRate = .11111M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 5, LaborCode = 123, Pieces = 100, PrimaRate = .11110M, ExpectedGross = 11.11M},
			};

			var testLines = tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				useIncreasedRate: false,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}
	}
}
