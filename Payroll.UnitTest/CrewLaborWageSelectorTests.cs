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
    public class CrewLaborWageSelectorTests
    {
        [TestMethod]
        public void SelectsWageBasedOnEffectiveDate()
        {
            var dbName = "SelectsWageBasedOnEffectiveDate";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Remove any default seeded values and replace with known
            context.RemoveRange(context.CrewLaborWages.ToList());
            context.SaveChanges();

            context.Add(Helper.MockCrewLaborWage(id: 1, effectiveDate: new DateTime(2000, 1, 1), wage: 6.75M));
            context.Add(Helper.MockCrewLaborWage(id: 2, effectiveDate: new DateTime(2020, 2, 17), wage: 13));
            context.Add(Helper.MockCrewLaborWage(id: 3, effectiveDate: new DateTime(2020, 2, 18), wage: 13.5M));
            context.Add(Helper.MockCrewLaborWage(id: 4, effectiveDate: new DateTime(2020, 2, 19), wage: 14));
            context.Add(Helper.MockCrewLaborWage(id: 5, effectiveDate: new DateTime(2020, 2, 20), wage: 14.75M));
            context.Add(Helper.MockCrewLaborWage(id: 6, effectiveDate: new DateTime(2020, 2, 21), wage: 15));
            context.Add(Helper.MockCrewLaborWage(id: 7, effectiveDate: new DateTime(2020, 2, 22), wage: 15.67M));
            context.Add(Helper.MockCrewLaborWage(id: 8, effectiveDate: new DateTime(2020, 2, 23), wage: 16));
            context.SaveChanges();

            if (context.CrewLaborWages.Count() != 8) Assert.Inconclusive("Unexpected number of CrewLaborWages.");

            var wageSelector = new CrewLaborWageSelector(context);

            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 16)));
            Assert.AreEqual(13, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 17)));
            Assert.AreEqual(13.5M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 18)));
            Assert.AreEqual(14M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 19)));
            Assert.AreEqual(14.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 20)));
            Assert.AreEqual(15M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 21)));
            Assert.AreEqual(15.67M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 22)));
            Assert.AreEqual(16M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 23)));
            Assert.AreEqual(16M, wageSelector.GetCrewLaborWage(new DateTime(2025, 1, 1)));
        }

        [TestMethod]
        public void SelectsNonDeletedRecords()
        {
            var dbName = "SelectsNonDeletedRecords";
            var options = new DbContextOptionsBuilder<PayrollContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            using var context = new PayrollContext(options);
            context.Database.EnsureCreated();

            // Remove any default seeded values and replace with known
            context.RemoveRange(context.CrewLaborWages.ToList());
            context.SaveChanges();

            context.Add(Helper.MockCrewLaborWage(id: 1, effectiveDate: new DateTime(2000, 1, 1), wage: 6.75M));
            context.Add(Helper.MockCrewLaborWage(id: 2, effectiveDate: new DateTime(2020, 2, 17), wage: 13, isDeleted: true));
            context.Add(Helper.MockCrewLaborWage(id: 3, effectiveDate: new DateTime(2020, 2, 18), wage: 13.5M, isDeleted: true));
            context.Add(Helper.MockCrewLaborWage(id: 4, effectiveDate: new DateTime(2020, 2, 19), wage: 14, isDeleted: true));
            context.Add(Helper.MockCrewLaborWage(id: 5, effectiveDate: new DateTime(2020, 2, 20), wage: 14.75M, isDeleted: true));
            context.Add(Helper.MockCrewLaborWage(id: 6, effectiveDate: new DateTime(2020, 2, 21), wage: 15, isDeleted: true));
            context.Add(Helper.MockCrewLaborWage(id: 7, effectiveDate: new DateTime(2020, 2, 22), wage: 15.67M, isDeleted: true));
            context.Add(Helper.MockCrewLaborWage(id: 8, effectiveDate: new DateTime(2020, 2, 23), wage: 16, isDeleted: true));
            context.SaveChanges();

            if (context.CrewLaborWages.Count() != 8) Assert.Inconclusive("Unexpected number of CrewLaborWages.");

            var wageSelector = new CrewLaborWageSelector(context);

            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 16)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 17)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 18)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 19)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 20)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 21)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 22)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2020, 2, 23)));
            Assert.AreEqual(6.75M, wageSelector.GetCrewLaborWage(new DateTime(2025, 1, 1)));
        }

    }
}
