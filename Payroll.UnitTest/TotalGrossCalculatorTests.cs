using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Payroll.Data;
using System.Linq;
using Payroll.UnitTest.Helpers;

namespace Payroll.UnitTest
{
	[TestClass]
	public class TotalGrossCalculatorTests
	{
		RoundingService _roundingService = new RoundingService();

		[TestMethod]
		public void RanchPayLine_TotalGrossIsSum()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 0 },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 100.99M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, ExpectedTotalGross = 302.42M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, ExpectedTotalGross = 999.87M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, ExpectedTotalGross = 100 }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void RanchPayLine_TotalGross_RoundsUpAtMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1191M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1181M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1171M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1161M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1156M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 6, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1150M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 7, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1145M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 8, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.11445M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 9, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114445M, ExpectedTotalGross = 100.12M },
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void RanchPayLine_TotalGross_RoundsDownBelowMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114444M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.113M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.112M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.111M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.110M, ExpectedTotalGross = 100.11M }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLine_TotalGrossIsSum()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 0 },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 100.99M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, ExpectedTotalGross = 302.42M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, ExpectedTotalGross = 999.87M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, ExpectedTotalGross = 100 }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLine_TotalGross_RoundsUpAtMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1191M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1181M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1171M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1161M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1156M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 6, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1150M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 7, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1145M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 8, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.11445M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 9, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114445M, ExpectedTotalGross = 100.12M },
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentLine_TotalGross_RoundsDownBelowMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114444M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.113M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.112M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.111M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.110M, ExpectedTotalGross = 100.11M }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testAdjustmentLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testAdjustmentLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testAdjustmentLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void PlantPayLine_TotalGrossIsSum()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 0 },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 100.99M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 302.42M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, GrossFromIncentive = 0, ExpectedTotalGross = 999.87M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 37.42M },
				new TotalGrossTestCase { Id = 6, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 137.42M }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross, grossFromIncentive: x.GrossFromIncentive)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void PlantPayLine_TotalGross_RoundsUpAtMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1191M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1181M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1171M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1161M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1156M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 6, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1150M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 7, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1145M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 8, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.11445M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 9, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114445M, ExpectedTotalGross = 100.12M },
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void PlantPayLine_TotalGross_RoundsDownBelowMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114444M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.113M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.112M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.111M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.110M, ExpectedTotalGross = 100.11M }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLine_TotalGrossIsSum()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 0 },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 100.99M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 302.42M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, GrossFromIncentive = 0, ExpectedTotalGross = 999.87M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 37.42M },
				new TotalGrossTestCase { Id = 6, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 137.42M }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross, grossFromIncentive: x.GrossFromIncentive)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLine_TotalGross_RoundsUpAtMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1191M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1181M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1171M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1161M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1156M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 6, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1150M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 7, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.1145M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 8, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.11445M, ExpectedTotalGross = 100.12M },
				new TotalGrossTestCase { Id = 9, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114445M, ExpectedTotalGross = 100.12M },
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLine_TotalGross_RoundsDownBelowMidPoint()
		{
			var tests = new List<TotalGrossTestCase>
			{
				new TotalGrossTestCase { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.114444M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 2, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.113M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 3, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.112M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.111M, ExpectedTotalGross = 100.11M },
				new TotalGrossTestCase { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 100.110M, ExpectedTotalGross = 100.11M }
			};

			var totalGrossCalculator = new TotalGrossCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
			totalGrossCalculator.CalculateTotalGross(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
			}
		}
	}
}
