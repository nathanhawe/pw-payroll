using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Payroll.UnitTest
{
    [TestClass]
    public class GrossFromPiecesCalculatorTests
    {
        private class GrossFromPiecesTest
        {
            public int Id { get; set; }
            public decimal Pieces { get; set; }
            public decimal PieceRate { get; set; }
            public decimal ExpectedGrossFromPieces { get; set; }
        }

        [TestMethod]
        public void GrossFromPiecesIsProduct()
        {
            var tests = new List<GrossFromPiecesTest>
            {
                new GrossFromPiecesTest { Id = 1, Pieces = 0, PieceRate = 0, ExpectedGrossFromPieces = 0 },
                new GrossFromPiecesTest { Id = 2, Pieces = 100, PieceRate = 0, ExpectedGrossFromPieces = 0 },
                new GrossFromPiecesTest { Id = 3, Pieces = 0, PieceRate = .31M, ExpectedGrossFromPieces = 0 },
                new GrossFromPiecesTest { Id = 4, Pieces = 100, PieceRate = .31M, ExpectedGrossFromPieces = 31 },
                new GrossFromPiecesTest { Id = 5, Pieces = 500, PieceRate = .31111M, ExpectedGrossFromPieces = 155.56M },
                new GrossFromPiecesTest { Id = 6, Pieces = 100, PieceRate = .31114M, ExpectedGrossFromPieces = 31.11M },
                new GrossFromPiecesTest { Id = 7, Pieces = 100, PieceRate = .31116M, ExpectedGrossFromPieces = 31.12M },
                new GrossFromPiecesTest { Id = 8, Pieces = 100, PieceRate = .31115M, ExpectedGrossFromPieces = 31.12M }

            };

            var grossFromPiecesCalculator = new GrossFromPiecesCalculator();

            var testPayLines = tests.Select(x => Helper.MockRanchPayLine(id: x.Id, pieces: x.Pieces, pieceRate: x.PieceRate)).ToList();

            grossFromPiecesCalculator.CalculateGrossFromPieces(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromPieces == test.ExpectedGrossFromPieces).Count());
            }
        }
    }
}
