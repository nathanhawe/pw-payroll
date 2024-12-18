﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Domain.Constants;
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
		public void LaborCode_555_TallyTagWriter_Incentive_ReturnsIncentive()
		{
			TallyTagWriter_Incentive_ReturnsIncentive((int)PlantLaborCode.TallyTagWriter);
		}

		[TestMethod]
		public void LaborCode_7555_LightDutyTallyTagWriter_Incentive_ReturnsIncentive()
		{
			TallyTagWriter_Incentive_ReturnsIncentive((int)PlantLaborCode.LightDuty_TallyTagWriter);
		}

		[TestMethod]
		public void LaborCode_555_TallyTagWriter_IncentiveDisqualified_ReturnsZero()
		{
			TallyTagWriter_IncentiveDisqualified_ReturnsZero((int)PlantLaborCode.TallyTagWriter);
			
		}

		[TestMethod]
		public void LaborCode_7555_LightDutyTallyTagWriter_IncentiveDisqualified_ReturnsZero()
		{
			TallyTagWriter_IncentiveDisqualified_ReturnsZero((int)PlantLaborCode.TallyTagWriter);

		}

		[TestMethod]
		public void LaborCode_555_TallyTagWriter_Incentive_IgnoresNonPrima()
		{
			TallyTagWriter_Incentive_IgnoresNonPrima((int)PlantLaborCode.TallyTagWriter);
		}

		[TestMethod]
		public void LaborCode_7555_LightDutyTallyTagWriter_Incentive_IgnoresNonPrima()
		{
			TallyTagWriter_Incentive_IgnoresNonPrima((int)PlantLaborCode.LightDuty_TallyTagWriter);
		}

		[TestMethod]
		public void LaborCode_558_TagWriterLead_Incentive_Before20220418_ReturnsIncentive()
		{
			var laborCode = (int)PlantLaborCode.TagWriterLead;

			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 8M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .25M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 20M},
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
				laborCode: x.LaborCode,
				shiftDate: new DateTime(2022, 4, 17))).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void LaborCode_558_TagWriterLead_IncentiveDisqualified_ReturnsZero()
		{
			var laborCode = (int)PlantLaborCode.TagWriterLead;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
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
		public void LaborCode_558_TagWriterLead_Incentive_Before20220418_IgnoresNonPrima()
		{
			var laborCode = (int)PlantLaborCode.TagWriterLead;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 8M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .25M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 20M},
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
				laborCode: x.LaborCode,
				shiftDate: new DateTime(2022, 4, 17))).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void LaborCode_558_TagWriterLead_OnOrAfter20220418_ReturnsNoIncentive()
		{
			var laborCode = (int)PlantLaborCode.TagWriterLead;

			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
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
				laborCode: x.LaborCode,
				shiftDate: new DateTime(2022, 4, 18))).ToList();

			testLines.AddRange(tests.Select(x => EntityMocker.MockPlantPayLine(
				id: x.Id,
				hoursWorked: x.HoursWorked,
				pieces: x.Pieces,
				increasedRate: x.IncreasedRate,
				primaRate: x.PrimaRate,
				nonPrimaRate: x.NonPrimaRate,
				isIncentiveDisqualified: false,
				hasNonPrimaViolation: false,
				laborCode: x.LaborCode,
				shiftDate: new DateTime(2032, 4, 18))).ToList());

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(2, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}


		[TestMethod]
		public void UseIncreasedRate_ReturnsIncreasedRateIncentive()
		{
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 15M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .3M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 11.25M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 15M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .3M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 11.25M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .14M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 3.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 5.25M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, HoursWorked = 8, Pieces = 100, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 7M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, HoursWorked = 0, Pieces = 2, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .14M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, HoursWorked = .25M, Pieces = 50, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 3.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, HoursWorked = 20M, Pieces = 75, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 5.25M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, Pieces = 100, PrimaRate = .11115M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, Pieces = 100, PrimaRate = .11116M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, Pieces = 100, PrimaRate = .11117M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, Pieces = 100, PrimaRate = .11118M, ExpectedGross = 11.12M},
				new GrossFromIncentiveTestCase { Id = 5, LaborCode = laborCode, Pieces = 100, PrimaRate = .11119M, ExpectedGross = 11.12M},
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
			var laborCode = (int)PlantLaborCode.Packing;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode = laborCode, Pieces = 100, PrimaRate = .11114M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode = laborCode, Pieces = 100, PrimaRate = .11113M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode = laborCode, Pieces = 100, PrimaRate = .11112M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode = laborCode, Pieces = 100, PrimaRate = .11111M, ExpectedGross = 11.11M},
				new GrossFromIncentiveTestCase { Id = 5, LaborCode = laborCode, Pieces = 100, PrimaRate = .11110M, ExpectedGross = 11.11M},
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
		public void NonDiscretionaryBonus()
		{
			var laborCode = (int)PlantLaborCode.Unknown;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, NonDiscretionaryBonusRate = 2, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, NonDiscretionaryBonusRate = 2, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = 10.25M, NonDiscretionaryBonusRate = 2.25M, ExpectedGross = 23.06M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 8M, NonDiscretionaryBonusRate = 0, ExpectedGross = 0M},
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
				laborCode: x.LaborCode,
				nonDiscretionaryBonusRate: x.NonDiscretionaryBonusRate)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void NonDiscretionaryBonusRate_IgnoresIncentiveDisqualified()
		{
			var laborCode = (int)PlantLaborCode.Unknown;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, NonDiscretionaryBonusRate = 2, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, NonDiscretionaryBonusRate = 2, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = 10.25M, NonDiscretionaryBonusRate = 2.25M, ExpectedGross = 23.06M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 8M, NonDiscretionaryBonusRate = 0, ExpectedGross = 0M},
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
				laborCode: x.LaborCode,
				nonDiscretionaryBonusRate: x.NonDiscretionaryBonusRate)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void NonDiscretionaryBonusRate_IsNotAddedToTallyTagWriting()
		{
			var laborCode = (int)PlantLaborCode.TallyTagWriter;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, NonDiscretionaryBonusRate = 2, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, NonDiscretionaryBonusRate = 2, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = 10.25M, NonDiscretionaryBonusRate = 2.25M, ExpectedGross = 20.5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 8M, NonDiscretionaryBonusRate = 0, ExpectedGross = 16M},
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
				laborCode: x.LaborCode,
				nonDiscretionaryBonusRate: x.NonDiscretionaryBonusRate)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		[TestMethod]
		public void NonDiscretionaryBonusRate_IsNotAddedToTagWriterLead()
		{
			var laborCode = (int)PlantLaborCode.TagWriterLead;
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, NonDiscretionaryBonusRate = 2, ExpectedGross = 8M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, NonDiscretionaryBonusRate = 2, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = 10.25M, NonDiscretionaryBonusRate = 2.25M, ExpectedGross = 10.25M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 8M, NonDiscretionaryBonusRate = 0, ExpectedGross = 8M},
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
				laborCode: x.LaborCode,
				nonDiscretionaryBonusRate: x.NonDiscretionaryBonusRate)).ToList();

			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(testLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testLines.Where(x => x.Id == test.Id && x.GrossFromIncentive == test.ExpectedGross).Count());
			}
		}

		#region Helpers
		private void TallyTagWriter_Incentive_IgnoresNonPrima(int laborCode)
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 40M},
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

		private void TallyTagWriter_Incentive_ReturnsIncentive(int laborCode)
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 16M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = .5M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 40M},
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

		private void TallyTagWriter_IncentiveDisqualified_ReturnsZero(int laborCode)
		{
			var tests = new List<GrossFromIncentiveTestCase>
			{
				new GrossFromIncentiveTestCase { Id = 1, LaborCode =  laborCode, HoursWorked = 8, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 2, LaborCode =  laborCode, HoursWorked = 0, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 3, LaborCode =  laborCode, HoursWorked = .25M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
				new GrossFromIncentiveTestCase { Id = 4, LaborCode =  laborCode, HoursWorked = 20M, Pieces = 0, IncreasedRate = .83M, PrimaRate = .75M, NonPrimaRate = .68M, ExpectedGross = 0M},
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

		#endregion
	}
}
