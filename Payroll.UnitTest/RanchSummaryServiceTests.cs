using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
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
	public class RanchSummaryServiceTests
	{
		private PayrollContext _context;
		private RanchSummaryService _ranchSummaryService;

		[TestInitialize]
		public void Setup()
		{
			var dbName = "RanchSummaryServiceTests";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			_context = new PayrollContext(options);
			_context.Database.EnsureCreated();

			if (_context.RanchPayLines.Count() == 0)
			{
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee4", crew: (int)Crew.WestTractor_Night, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 172, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 172, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee4", crew: (int)Crew.WestTractor_Night, lastCrew: 100, hoursWorked: 10, totalGross: 172));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 6), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 7), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 8), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 9), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 10), employeeId: "Employee1", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 6), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 7), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 8), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 9), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 10), employeeId: "Employee2", crew: 100, lastCrew: 142, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 6), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 7), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 8), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 9), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 10), employeeId: "Employee3", crew: 100, lastCrew: 60, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 6), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 7), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 8), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 9), employeeId: "Employee4", crew: 100, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), shiftDate: new DateTime(2020, 1, 10), employeeId: "Employee4", crew: (int)Crew.WestTractor_Night, lastCrew: 100, hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockRanchPayLine(batchId: 3, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 30), employeeId: "Employee1", crew: 60, lastCrew: 100, hoursWorked: 10, totalGross: 150, laborCode: (int)RanchLaborCode.Covid19));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 3, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2019, 12, 31), employeeId: "Employee1", crew: 60, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 3, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 1), employeeId: "Employee1", crew: 60, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 3, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 2), employeeId: "Employee1", crew: 60, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockRanchPayLine(batchId: 3, weekEndDate: new DateTime(2020, 1, 5), shiftDate: new DateTime(2020, 1, 3), employeeId: "Employee1", crew: 60, lastCrew: 100, hoursWorked: 10, totalGross: 150));
				
				_context.SaveChanges();
			}

			_ranchSummaryService = new RanchSummaryService(_context);
		}
		
		[TestMethod]
		public void FromBatch_SumOfHoursWorked()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalHours == 50).Count());
		}

		[TestMethod]
		public void FromBatch_SumOfTotalGross()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 750).Count());
		}

		[TestMethod]
		public void FromBatch_SumOfCulturalHours_AreHoursForCrewsBelow61()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.CulturalHours == 10).Count());

			ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(3);
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CulturalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Count());

		}

		[TestMethod]
		public void FromBatch_SumOfCovidHours_AreHoursForLaborCode600()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.CovidHours == 20).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.CovidHours == 30).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.CovidHours == 50).Count());

			ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(2);
			
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 30).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 20).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
		}

		[TestMethod]
		public void FromBatch_GroupsByWeekEndDate()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(2);

			Assert.AreEqual(8, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 142 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 60 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 142 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 60 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 10).Count());
		}

		[TestMethod]
		public void FromBatch_GroupsByEmployeeId()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2").Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3").Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4").Count());
		}

		[TestMethod]
		public void FromBatch_GroupsByLastCrew()
		{
			var ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.LastCrew == 100).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.LastCrew == 142).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.LastCrew == 60).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.LastCrew == 100).Count());
		}

		[TestMethod]
		public void FromList_SumOfHoursWorked()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 1).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalHours == 50).Count());
		}

		[TestMethod]
		public void FromList_SumOfTotalGross()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 1).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 750).Count());
		}

		[TestMethod]
		public void FromList_SumOfCulturalHours_AreHoursForCrewsBelow61()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 1).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.CulturalHours == 10).Count());
			
			ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 3).ToList();
			ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CulturalHours == 50).Count());
			Assert.AreEqual(1, ranchSummaries.Count());

		}

		[TestMethod]
		public void FromList_SumOfCovidHours_AreHoursForLaborCode600()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 1).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.CovidHours == 20).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.CovidHours == 30).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.CovidHours == 50).Count());

			ranchSummaries = _ranchSummaryService.CreateSummariesForBatch(2);

			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 30).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 20).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
		}

		[TestMethod]
		public void FromList_GroupsByWeekEndDate()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 2).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(8, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 142 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 60 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 860 && x.CulturalHours == 10).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 142 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 60 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 0).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.LastCrew == 100 && x.TotalHours == 50 && x.TotalGross == 750 && x.CulturalHours == 10).Count());
		}

		[TestMethod]
		public void FromList_GroupsByEmployeeId()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 1).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2").Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3").Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4").Count());
		}

		[TestMethod]
		public void FromList_GroupsByLastCrew()
		{
			var ranchPayLines = _context.RanchPayLines.Where(x => x.BatchId == 1).ToList();
			var ranchSummaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);

			Assert.AreEqual(4, ranchSummaries.Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee1" && x.LastCrew == 100).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee2" && x.LastCrew == 142).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee3" && x.LastCrew == 60).Count());
			Assert.AreEqual(1, ranchSummaries.Where(x => x.EmployeeId == "Employee4" && x.LastCrew == 100).Count());
		}
	}
}
