using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Service;
using Payroll.UnitTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest
{
	[TestClass]
	public class CulturalLaborWageServiceTests
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
			context.RemoveRange(context.CulturalLaborWages.ToList());
			context.SaveChanges();

			context.Add(EntityMocker.MockCulturalLaborWage(id: 1, effectiveDate: new DateTime(2000, 1, 1), wage: 6.75M));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 2, effectiveDate: new DateTime(2020, 2, 17), wage: 13));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 3, effectiveDate: new DateTime(2020, 2, 18), wage: 13.5M));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 4, effectiveDate: new DateTime(2020, 2, 19), wage: 14));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 5, effectiveDate: new DateTime(2020, 2, 20), wage: 14.75M));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 6, effectiveDate: new DateTime(2020, 2, 21), wage: 15));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 7, effectiveDate: new DateTime(2020, 2, 22), wage: 15.67M));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 8, effectiveDate: new DateTime(2020, 2, 23), wage: 16));
			context.SaveChanges();

			if (context.CulturalLaborWages.Count() != 8) Assert.Inconclusive("Unexpected number of CulturalLaborWages.");

			var CulturalLaborWageService = new CulturalLaborWageService(context);

			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 16)));
			Assert.AreEqual(13, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 17)));
			Assert.AreEqual(13.5M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 18)));
			Assert.AreEqual(14M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 19)));
			Assert.AreEqual(14.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 20)));
			Assert.AreEqual(15M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 21)));
			Assert.AreEqual(15.67M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 22)));
			Assert.AreEqual(16M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 23)));
			Assert.AreEqual(16M, CulturalLaborWageService.GetWage(new DateTime(2025, 1, 1)));
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
			context.RemoveRange(context.CulturalLaborWages.ToList());
			context.SaveChanges();

			context.Add(EntityMocker.MockCulturalLaborWage(id: 1, effectiveDate: new DateTime(2000, 1, 1), wage: 6.75M));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 2, effectiveDate: new DateTime(2020, 2, 17), wage: 13, isDeleted: true));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 3, effectiveDate: new DateTime(2020, 2, 18), wage: 13.5M, isDeleted: true));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 4, effectiveDate: new DateTime(2020, 2, 19), wage: 14, isDeleted: true));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 5, effectiveDate: new DateTime(2020, 2, 20), wage: 14.75M, isDeleted: true));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 6, effectiveDate: new DateTime(2020, 2, 21), wage: 15, isDeleted: true));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 7, effectiveDate: new DateTime(2020, 2, 22), wage: 15.67M, isDeleted: true));
			context.Add(EntityMocker.MockCulturalLaborWage(id: 8, effectiveDate: new DateTime(2020, 2, 23), wage: 16, isDeleted: true));
			context.SaveChanges();

			if (context.CulturalLaborWages.Count() != 8) Assert.Inconclusive("Unexpected number of CulturalLaborWages.");

			var CulturalLaborWageService = new CulturalLaborWageService(context);

			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 16)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 17)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 18)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 19)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 20)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 21)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 22)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2020, 2, 23)));
			Assert.AreEqual(6.75M, CulturalLaborWageService.GetWage(new DateTime(2025, 1, 1)));
		}

	}
}
