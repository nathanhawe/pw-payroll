using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
    [TestClass]
    public class MinimumWageServiceTests
    {
        [TestMethod]
        public void GetMinimumWageOnDate_SelectsCorrectWageForDate()
        {
            var dbName = "GetMinimumWageOnDate_SelectsCorrectWageForDate";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            var minimumWageService = new MinimumWageService(context);

            Assert.AreEqual(0, minimumWageService.GetMinimumWageOnDate(new DateTime(2009, 12, 31)));
            Assert.AreEqual(8, minimumWageService.GetMinimumWageOnDate(new DateTime(2010, 1, 1)));
            Assert.AreEqual(8, minimumWageService.GetMinimumWageOnDate(new DateTime(2014, 6, 30)));
            Assert.AreEqual(9, minimumWageService.GetMinimumWageOnDate(new DateTime(2014, 7, 1)));
            Assert.AreEqual(9, minimumWageService.GetMinimumWageOnDate(new DateTime(2015, 12, 31)));
            Assert.AreEqual(10, minimumWageService.GetMinimumWageOnDate(new DateTime(2016, 1, 1)));
            Assert.AreEqual(10, minimumWageService.GetMinimumWageOnDate(new DateTime(2016, 12, 31)));
            Assert.AreEqual(10.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2017, 1, 1)));
            Assert.AreEqual(10.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2017, 12, 31)));
            Assert.AreEqual(11, minimumWageService.GetMinimumWageOnDate(new DateTime(2018, 1, 1)));
            Assert.AreEqual(11, minimumWageService.GetMinimumWageOnDate(new DateTime(2018, 12, 31)));
            Assert.AreEqual(12, minimumWageService.GetMinimumWageOnDate(new DateTime(2019, 1, 1)));
            Assert.AreEqual(12, minimumWageService.GetMinimumWageOnDate(new DateTime(2019, 12, 31)));
            Assert.AreEqual(13, minimumWageService.GetMinimumWageOnDate(new DateTime(2020, 1, 1)));
            Assert.AreEqual(13, minimumWageService.GetMinimumWageOnDate(new DateTime(2020, 12, 31)));
            Assert.AreEqual(14, minimumWageService.GetMinimumWageOnDate(new DateTime(2021, 1, 1)));
            Assert.AreEqual(14, minimumWageService.GetMinimumWageOnDate(new DateTime(2021, 12, 31)));
            Assert.AreEqual(15, minimumWageService.GetMinimumWageOnDate(new DateTime(2022, 1, 1)));
            Assert.AreEqual(15, minimumWageService.GetMinimumWageOnDate(new DateTime(2099, 12, 31)));
        }

        [TestMethod]
        public void GetMinimumWageOnDate_SelectsNonDeletedWage()
        {
            var dbName = "GetMinimumWageOnDate_SelectsNonDeletedWage";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            context.MinimumWages.RemoveRange(context.MinimumWages.ToList());
            context.Add(Helper.MockMinimumWage(effectiveDate: new DateTime(2000, 1, 1), wage: 8.5M));
            context.Add(Helper.MockMinimumWage(effectiveDate: new DateTime(2001, 1, 1), wage: 9M, isDeleted: true));
            context.Add(Helper.MockMinimumWage(effectiveDate: new DateTime(2002, 1, 1), wage: 9.5M, isDeleted: true));
            context.Add(Helper.MockMinimumWage(effectiveDate: new DateTime(2003, 1, 1), wage: 10M, isDeleted: true));
            context.Add(Helper.MockMinimumWage(effectiveDate: new DateTime(2004, 1, 1), wage: 10.5M, isDeleted: true));
            context.SaveChanges();

            if (context.MinimumWages.Count() != 5) Assert.Inconclusive("Unexpected count of Minimum Wage records.");

            var minimumWageService = new MinimumWageService(context);

            Assert.AreEqual(8.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2000, 1, 1)));
            Assert.AreEqual(8.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2001, 1, 1)));
            Assert.AreEqual(8.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2002, 1, 1)));
            Assert.AreEqual(8.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2003, 1, 1)));
            Assert.AreEqual(8.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2004, 1, 1)));
            Assert.AreEqual(8.5M, minimumWageService.GetMinimumWageOnDate(new DateTime(2005, 1, 1)));
        }
    }
}

