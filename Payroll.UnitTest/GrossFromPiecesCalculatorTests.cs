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
	public class GrossFromPiecesCalculatorTests
	{
		private readonly RoundingService _roundingService = new RoundingService();

		[TestMethod]
		public void RanchPayLine_GrossFromPiecesIsProduct()
		{
			var tests = new List<GrossFromPiecesTestCase>
			{
				new GrossFromPiecesTestCase { Id = 1, Pieces = 0, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 2, Pieces = 100, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 3, Pieces = 0, PieceRate = .31M, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 4, Pieces = 100, PieceRate = .31M, ExpectedGrossFromPieces = 31 },
				new GrossFromPiecesTestCase { Id = 5, Pieces = 500, PieceRate = .31111M, ExpectedGrossFromPieces = 155.56M },
				new GrossFromPiecesTestCase { Id = 6, Pieces = 100, PieceRate = .31114M, ExpectedGrossFromPieces = 31.11M },
				new GrossFromPiecesTestCase { Id = 7, Pieces = 100, PieceRate = .31116M, ExpectedGrossFromPieces = 31.12M },
				new GrossFromPiecesTestCase { Id = 8, Pieces = 100, PieceRate = .31115M, ExpectedGrossFromPieces = 31.12M }

			};

			var grossFromPiecesCalculator = new GrossFromPiecesCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchPayLine(id: x.Id, pieces: x.Pieces, pieceRate: x.PieceRate)).ToList();

			grossFromPiecesCalculator.CalculateGrossFromPieces(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromPieces == test.ExpectedGrossFromPieces).Count());
			}
		}

		[TestMethod]
		public void RanchAdjustmentPayLine_GrossFromPiecesIsProduct()
		{
			var tests = new List<GrossFromPiecesTestCase>
			{
				new GrossFromPiecesTestCase { Id = 1, Pieces = 0, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 2, Pieces = 100, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 3, Pieces = 0, PieceRate = .31M, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 4, Pieces = 100, PieceRate = .31M, ExpectedGrossFromPieces = 31 },
				new GrossFromPiecesTestCase { Id = 5, Pieces = 500, PieceRate = .31111M, ExpectedGrossFromPieces = 155.56M },
				new GrossFromPiecesTestCase { Id = 6, Pieces = 100, PieceRate = .31114M, ExpectedGrossFromPieces = 31.11M },
				new GrossFromPiecesTestCase { Id = 7, Pieces = 100, PieceRate = .31116M, ExpectedGrossFromPieces = 31.12M },
				new GrossFromPiecesTestCase { Id = 8, Pieces = 100, PieceRate = .31115M, ExpectedGrossFromPieces = 31.12M }

			};

			var grossFromPiecesCalculator = new GrossFromPiecesCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockRanchAdjustmentLine(id: x.Id, pieces: x.Pieces, pieceRate: x.PieceRate)).ToList();

			grossFromPiecesCalculator.CalculateGrossFromPieces(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromPieces == test.ExpectedGrossFromPieces).Count());
			}
		}

		[TestMethod]
		public void PlantPayLine_GrossFromPiecesIsProductOfNonPrimaRate()
		{
			var tests = new List<GrossFromPiecesTestCase>
			{
				new GrossFromPiecesTestCase { Id = 1, Pieces = 0, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 2, Pieces = 100, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 3, Pieces = 0, PieceRate = .31M, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 4, Pieces = 100, PieceRate = .31M, ExpectedGrossFromPieces = 31 },
				new GrossFromPiecesTestCase { Id = 5, Pieces = 500, PieceRate = .31111M, ExpectedGrossFromPieces = 155.56M },
				new GrossFromPiecesTestCase { Id = 6, Pieces = 100, PieceRate = .31114M, ExpectedGrossFromPieces = 31.11M },
				new GrossFromPiecesTestCase { Id = 7, Pieces = 100, PieceRate = .31116M, ExpectedGrossFromPieces = 31.12M },
				new GrossFromPiecesTestCase { Id = 8, Pieces = 100, PieceRate = .31115M, ExpectedGrossFromPieces = 31.12M }

			};

			var grossFromPiecesCalculator = new GrossFromPiecesCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantPayLine(id: x.Id, pieces: x.Pieces, nonPrimaRate: x.PieceRate)).ToList();

			grossFromPiecesCalculator.CalculateGrossFromPieces(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromPieces == test.ExpectedGrossFromPieces).Count());
			}
		}

		[TestMethod]
		public void PlantAdjustmentLine_GrossFromPiecesIsProductOfPieceRate()
		{
			var tests = new List<GrossFromPiecesTestCase>
			{
				new GrossFromPiecesTestCase { Id = 1, Pieces = 0, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 2, Pieces = 100, PieceRate = 0, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 3, Pieces = 0, PieceRate = .31M, ExpectedGrossFromPieces = 0 },
				new GrossFromPiecesTestCase { Id = 4, Pieces = 100, PieceRate = .31M, ExpectedGrossFromPieces = 31 },
				new GrossFromPiecesTestCase { Id = 5, Pieces = 500, PieceRate = .31111M, ExpectedGrossFromPieces = 155.56M },
				new GrossFromPiecesTestCase { Id = 6, Pieces = 100, PieceRate = .31114M, ExpectedGrossFromPieces = 31.11M },
				new GrossFromPiecesTestCase { Id = 7, Pieces = 100, PieceRate = .31116M, ExpectedGrossFromPieces = 31.12M },
				new GrossFromPiecesTestCase { Id = 8, Pieces = 100, PieceRate = .31115M, ExpectedGrossFromPieces = 31.12M }

			};

			var grossFromPiecesCalculator = new GrossFromPiecesCalculator(_roundingService);

			var testPayLines = tests.Select(x => EntityMocker.MockPlantAdjustmentLine(id: x.Id, pieces: x.Pieces, pieceRate: x.PieceRate)).ToList();

			grossFromPiecesCalculator.CalculateGrossFromPieces(testPayLines);

			foreach (var test in tests)
			{
				Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromPieces == test.ExpectedGrossFromPieces).Count());
			}
		}
	}
}
