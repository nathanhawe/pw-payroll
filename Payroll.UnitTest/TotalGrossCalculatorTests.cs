using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Payroll.Data;
using System.Linq;

namespace Payroll.UnitTest
{
    [TestClass]
    public class TotalGrossCalculatorTests
    {
        private class TotalGrossTest
        {
            public int Id { get; set; }
            public decimal GrossFromHours { get; set; }
            public decimal GrossFromPieces { get; set; }
            public decimal OtherGross { get; set; }
            public decimal GrossFromIncentive { get; set; }
            public decimal ExpectedTotalGross { get; set; }
        }

        [TestMethod]
        public void RanchPayLine_TotalGrossIsSum()
        {
            var tests = new List<TotalGrossTest>
            {
                new TotalGrossTest { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 0 },
                new TotalGrossTest { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 100.99M },
                new TotalGrossTest { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, ExpectedTotalGross = 302.42M },
                new TotalGrossTest { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, ExpectedTotalGross = 999.87M },
                new TotalGrossTest { Id = 5, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, ExpectedTotalGross = 100 }
            };

            var totalGrossCalculator = new TotalGrossCalculator();

            var testPayLines = tests.Select(x => Helper.MockRanchPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
            totalGrossCalculator.CalculateTotalGross(testPayLines);

            foreach(var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
            }
        }

        [TestMethod]
        public void RanchAdjustmentLine_TotalGrossIsSum()
        {
            var tests = new List<TotalGrossTest>
            {
                new TotalGrossTest { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 0 },
                new TotalGrossTest { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, ExpectedTotalGross = 100.99M },
                new TotalGrossTest { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, ExpectedTotalGross = 302.42M },
                new TotalGrossTest { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, ExpectedTotalGross = 999.87M },
                new TotalGrossTest { Id = 5, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, ExpectedTotalGross = 100 }
            };

            var totalGrossCalculator = new TotalGrossCalculator();

            var testPayLines = tests.Select(x => Helper.MockRanchAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
            totalGrossCalculator.CalculateTotalGross(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
            }
        }

        [TestMethod]
        public void PlantPayLine_TotalGrossIsSum()
        {
            var tests = new List<TotalGrossTest>
            {
                new TotalGrossTest { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 0 },
                new TotalGrossTest { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 100.99M },
                new TotalGrossTest { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 302.42M },
                new TotalGrossTest { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, GrossFromIncentive = 0, ExpectedTotalGross = 999.87M },
                new TotalGrossTest { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 37.42M },
                new TotalGrossTest { Id = 6, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 137.42M }
            };

            var totalGrossCalculator = new TotalGrossCalculator();

            var testPayLines = tests.Select(x => Helper.MockPlantPayLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
            totalGrossCalculator.CalculateTotalGross(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
            }
        }

        [TestMethod]
        public void PlantAdjustmentLine_TotalGrossIsSum()
        {
            var tests = new List<TotalGrossTest>
            {
                new TotalGrossTest { Id = 1, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 0 },
                new TotalGrossTest { Id = 2, GrossFromHours = 100.99M, GrossFromPieces = 0, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 100.99M },
                new TotalGrossTest { Id = 3, GrossFromHours = 0, GrossFromPieces = 302.42M, OtherGross = 0, GrossFromIncentive = 0, ExpectedTotalGross = 302.42M },
                new TotalGrossTest { Id = 4, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 999.87M, GrossFromIncentive = 0, ExpectedTotalGross = 999.87M },
                new TotalGrossTest { Id = 5, GrossFromHours = 0, GrossFromPieces = 0, OtherGross = 0M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 37.42M },
                new TotalGrossTest { Id = 6, GrossFromHours = 33.33M, GrossFromPieces = 33.34M, OtherGross = 33.33M, GrossFromIncentive = 37.42M, ExpectedTotalGross = 137.42M }
            };

            var totalGrossCalculator = new TotalGrossCalculator();

            var testPayLines = tests.Select(x => Helper.MockPlantAdjustmentLine(id: x.Id, grossFromHours: x.GrossFromHours, grossFromPieces: x.GrossFromPieces, otherGross: x.OtherGross)).ToList();
            totalGrossCalculator.CalculateTotalGross(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.TotalGross == test.ExpectedTotalGross).Count());
            }
        }
    }
}
