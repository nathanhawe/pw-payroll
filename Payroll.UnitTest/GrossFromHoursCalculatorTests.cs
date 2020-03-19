using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Payroll.UnitTest.Helpers;

namespace Payroll.UnitTest
{
	[TestClass]
	public class GrossFromHoursCalculatorTests
	{
		private readonly Mocks.MockRanchHourlyRateSelector _mockRanchHourlyRateSelector = new Mocks.MockRanchHourlyRateSelector(10);
		private readonly Mocks.MockPlantHourlyRateSelector _mockPlantHourlyRateSelector = new Mocks.MockPlantHourlyRateSelector(10);
		private readonly RoundingService _roundingService = new RoundingService();

		#region Ranch Pay Line Tests

		[TestMethod]
		public void RanchPayLines_GrossFromHoursIsProduct()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 9.99M, ExpectedGrossFromHours = 99.9M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchPayLines_RoundDownBelowMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1111M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1112M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1113M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1114M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11144M, ExpectedGrossFromHours = 101.11M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchPayLines_RoundUpAboveMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1119M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1118M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1117M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1116M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11156M, ExpectedGrossFromHours = 101.12M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchPayLines_RoundUpAtMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1115M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.11151M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.11145M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.111449M, ExpectedGrossFromHours = 101.12M },
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		#endregion

		#region Ranch Adjustment Line Tests

		[TestMethod]
		public void RanchAdjustmentLines_GrossFromHoursIsProduct()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 9.99M, ExpectedGrossFromHours = 99.90M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLines_RoundDownBelowMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1111M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1112M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1113M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1114M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11144M, ExpectedGrossFromHours = 101.11M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLines_RoundUpAboveMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1119M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1118M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1117M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1116M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11156M, ExpectedGrossFromHours = 101.12M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLines_RoundUpAtMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1115M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.11151M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.11145M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.111449M, ExpectedGrossFromHours = 101.12M },
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLines_UseOldHourlyRate_UsesOldHourlyRate()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 2, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 10, ExpectedGrossFromHours = 200 },
				new GrossFromHoursTestCase { Id = 3, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 4, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 10, ExpectedGrossFromHours = 200 },
				new GrossFromHoursTestCase { Id = 5, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 9.999M, ExpectedGrossFromHours = 199.98M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked, useOldHourlyRate: x.UseOldHourlyRate, oldHourlyRate: x.OldHourlyRate)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		#endregion

		#region Plant Pay Line Tests

		[TestMethod]
		public void PlantPayLines_GrossFromHoursIsProduct()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 9.99M, ExpectedGrossFromHours = 99.90M }


			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void PlantPayLines_RoundDownBelowMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1111M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1112M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1113M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1114M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11144M, ExpectedGrossFromHours = 101.11M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void PlantPayLines_RoundUpAboveMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1119M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1118M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1117M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1116M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11156M, ExpectedGrossFromHours = 101.12M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void PlantPayLines_RoundUpAtMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1115M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.11151M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.11145M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.111449M, ExpectedGrossFromHours = 101.12M },
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		#endregion

		#region Plant Adjustment Line Tests

		[TestMethod]
		public void PlantAdjustmentLines_GrossFromHoursIsProduct()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10, ExpectedGrossFromHours = 100 },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 9.99M, ExpectedGrossFromHours = 99.9M }


			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLines_RoundDownBelowMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1111M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1112M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1113M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1114M, ExpectedGrossFromHours = 101.11M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11144M, ExpectedGrossFromHours = 101.11M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLines_RoundUpAboveMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1119M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.1118M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.1117M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.1116M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 5, HoursWorked = 10.11156M, ExpectedGrossFromHours = 101.12M }
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLines_RoundUpAtMidPoint()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, HoursWorked = 10.1115M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 2, HoursWorked = 10.11151M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 3, HoursWorked = 10.11145M, ExpectedGrossFromHours = 101.12M },
				new GrossFromHoursTestCase { Id = 4, HoursWorked = 10.111449M, ExpectedGrossFromHours = 101.12M },
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		public void PlantAdjustmentLines_UseOldHourlyRate_UsesOldHourlyRate()
		{
			var tests = new List<GrossFromHoursTestCase>
			{
				new GrossFromHoursTestCase { Id = 1, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 2, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 10, ExpectedGrossFromHours = 200 },
				new GrossFromHoursTestCase { Id = 3, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 0, ExpectedGrossFromHours = 0 },
				new GrossFromHoursTestCase { Id = 4, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 10, ExpectedGrossFromHours = 200 },
				new GrossFromHoursTestCase { Id = 5, UseOldHourlyRate = true, OldHourlyRate = 20, HoursWorked = 9.999M, ExpectedGrossFromHours = 199.98M}
			};

			var grossFromHoursCalculator = new GrossFromHoursCalculator(_mockRanchHourlyRateSelector, _mockPlantHourlyRateSelector, _roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, hoursWorked: x.HoursWorked, useOldHourlyRate: x.UseOldHourlyRate, oldHourlyRate: x.OldHourlyRate)).ToList();

			grossFromHoursCalculator.CalculateGrossFromHours(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
			}
		}

		#endregion
	}
}
