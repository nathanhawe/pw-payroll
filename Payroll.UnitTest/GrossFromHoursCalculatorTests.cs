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
    public class GrossFromHoursCalculatorTests
    {
        private class GrossFromHoursTest
        {
            public int Id { get; set; }
            public decimal HoursWorked { get; set; }
            public decimal HourlyRate { get; set; }
            public decimal ExpectedGrossFromHours { get; set; }
        }

        [TestMethod]
        public void GrossFromHoursIsProduct()
        {
            var tests = new List<GrossFromHoursTest>
            {
                new GrossFromHoursTest { Id = 1, HoursWorked = 0, HourlyRate = 0, ExpectedGrossFromHours = 0 },
                new GrossFromHoursTest { Id = 2, HoursWorked = 10, HourlyRate = 0, ExpectedGrossFromHours = 0 },
                new GrossFromHoursTest { Id = 3, HoursWorked = 0, HourlyRate = 15, ExpectedGrossFromHours = 0 },
                new GrossFromHoursTest { Id = 4, HoursWorked = 10, HourlyRate = 15, ExpectedGrossFromHours = 150 },
                new GrossFromHoursTest { Id = 5, HoursWorked = 9.99M, HourlyRate = 15, ExpectedGrossFromHours = 419.85M }


            };

            var mockHourlyRateSelector = new Mocks.MockRanchHourlyRateSelector(15M);
            var grossFromHoursCalculator = new GrossFromHoursCalculator(mockHourlyRateSelector);

            var testPayLines = tests.Select(x => Helper.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

            grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
            }
        }

        [TestMethod]
        public void RoundDownBelowMidPoint()
        {
            var tests = new List<GrossFromHoursTest>
            {
                new GrossFromHoursTest { Id = 1, HoursWorked = 10, HourlyRate = 10.1111M, ExpectedGrossFromHours = 101.11M },
                new GrossFromHoursTest { Id = 2, HoursWorked = 10, HourlyRate = 10.1112M, ExpectedGrossFromHours = 101.11M },
                new GrossFromHoursTest { Id = 3, HoursWorked = 10, HourlyRate = 10.1113M, ExpectedGrossFromHours = 101.11M },
                new GrossFromHoursTest { Id = 4, HoursWorked = 10, HourlyRate = 10.1114M, ExpectedGrossFromHours = 101.11M },
                new GrossFromHoursTest { Id = 5, HoursWorked = 10, HourlyRate = 10.11144M, ExpectedGrossFromHours = 101.11M }
            };

            var mockHourlyRateSelector = new Mocks.MockRanchHourlyRateSelector(10M);
            var grossFromHoursCalculator = new GrossFromHoursCalculator(mockHourlyRateSelector);

            var testPayLines = tests.Select(x => Helper.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

            grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
            }
        }

        [TestMethod]
        public void RoundUpAboveMidPoint()
        {
            var tests = new List<GrossFromHoursTest>
            {
                new GrossFromHoursTest { Id = 1, HoursWorked = 10, HourlyRate = 10.1119M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 2, HoursWorked = 10, HourlyRate = 10.1118M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 3, HoursWorked = 10, HourlyRate = 10.1117M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 4, HoursWorked = 10, HourlyRate = 10.1116M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 5, HoursWorked = 10, HourlyRate = 10.11156M, ExpectedGrossFromHours = 101.12M }
            };

            var mockHourlyRateSelector = new Mocks.MockRanchHourlyRateSelector(10M);
            var grossFromHoursCalculator = new GrossFromHoursCalculator(mockHourlyRateSelector);

            var testPayLines = tests.Select(x => Helper.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

            grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
            }
        }

        [TestMethod]
        public void RoundUpAtMidPoint()
        {
            var tests = new List<GrossFromHoursTest>
            {
                new GrossFromHoursTest { Id = 1, HoursWorked = 10, HourlyRate = 10.1115M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 2, HoursWorked = 10, HourlyRate = 10.11151M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 3, HoursWorked = 10, HourlyRate = 10.11145M, ExpectedGrossFromHours = 101.12M },
                new GrossFromHoursTest { Id = 3, HoursWorked = 10, HourlyRate = 10.111449M, ExpectedGrossFromHours = 101.12M },
            };

            var mockHourlyRateSelector = new Mocks.MockRanchHourlyRateSelector(10M);
            var grossFromHoursCalculator = new GrossFromHoursCalculator(mockHourlyRateSelector);

            var testPayLines = tests.Select(x => Helper.MockRanchPayLine(id: x.Id, hoursWorked: x.HoursWorked)).ToList();

            grossFromHoursCalculator.CalculateGrossFromHours(testPayLines);

            foreach (var test in tests)
            {
                Assert.AreEqual(1, testPayLines.Where(x => x.Id == test.Id && x.GrossFromHours == test.ExpectedGrossFromHours).Count());
            }
        }
    }
}
