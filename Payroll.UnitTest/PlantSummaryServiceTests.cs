﻿using Microsoft.EntityFrameworkCore;
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
	public class PlantSummaryServiceTests
	{
		private PayrollContext _context;
		private PlantSummaryService _plantSummaryService;

		[TestInitialize]
		public void Setup()
		{
			var dbName = "PlantSummaryServiceTests";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;

			_context = new PayrollContext(options);
			_context.Database.EnsureCreated();

			if (_context.PlantPayLines.Count() == 0)
			{
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 1, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee1", hoursWorked: 10, totalGross: 172, laborCode: (int)PlantLaborCode.Covid19));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee2", hoursWorked: 10, totalGross: 172, laborCode: (int)PlantLaborCode.Covid19));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee3", hoursWorked: 10, totalGross: 172, laborCode: (int)PlantLaborCode.Covid19));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 172));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 5), employeeId: "Employee4", hoursWorked: 10, totalGross: 172));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee1", hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee2", hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee3", hoursWorked: 10, totalGross: 150));

				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee4", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee4", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee4", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee4", hoursWorked: 10, totalGross: 150));
				_context.Add(EntityMocker.MockPlantPayLine(batchId: 2, weekEndDate: new DateTime(2020, 1, 12), employeeId: "Employee4", hoursWorked: 10, totalGross: 150, laborCode: (int)PlantLaborCode.Covid19));

				_context.SaveChanges();
			}

			_plantSummaryService = new PlantSummaryService(_context);
		}
		
		[TestMethod]
		public void FromBatch_SumOfHoursWorked()
		{
			var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalHours == 50).Count());
		}

		[TestMethod]
		public void FromBatch_SumOfTotalGross()
		{
			var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 750).Count());
		}

		[TestMethod]
		public void FromBatch_SumOfCovidHours_AreHoursForLaborCode600()
		{
			var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.CovidHours == 20).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.CovidHours == 30).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.CovidHours == 50).Count());

			plantSummaries = _plantSummaryService.CreateSummariesForBatch(2);

			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 30).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 20).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 0).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
		}

		[TestMethod]
		public void FromBatch_GroupsByWeekEndDate()
		{
			var plantSummaries = _plantSummaryService.CreateSummariesForBatch(2);

			Assert.AreEqual(8, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
		}

		[TestMethod]
		public void FromBatch_GroupsByEmployeeId()
		{
			var plantSummaries = _plantSummaryService.CreateSummariesForBatch(1);

			Assert.AreEqual(4, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2").Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3").Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4").Count());
		}

		[TestMethod]
		public void FromList_SumOfHoursWorked()
		{
			var plantPayLines = _context.PlantPayLines.Where(x => x.BatchId == 1).ToList();
			var plantSummaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			Assert.AreEqual(4, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalHours == 50).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalHours == 50).Count());
		}

		[TestMethod]
		public void FromList_SumOfTotalGross()
		{
			var plantPayLines = _context.PlantPayLines.Where(x => x.BatchId == 1).ToList();
			var plantSummaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			Assert.AreEqual(4, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.TotalGross == 750).Count());
		}

		[TestMethod]
		public void FromList_SumOfCovidHours_AreHoursForLaborCode600()
		{
			var plantPayLines = _context.PlantPayLines.Where(x => x.BatchId == 1).ToList();
			var plantSummaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.CovidHours == 20).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.CovidHours == 30).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.CovidHours == 50).Count());

			plantPayLines = _context.PlantPayLines.Where(x => x.BatchId == 2).ToList();
			plantSummaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 30).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 20).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.CovidHours == 0).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.CovidHours == 10).Count());
		}

		[TestMethod]
		public void FromList_GroupsByWeekEndDate()
		{
			var plantPayLines = _context.PlantPayLines.Where(x => x.BatchId == 2).ToList();
			var plantSummaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			Assert.AreEqual(8, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 5) && x.TotalHours == 50 && x.TotalGross == 860).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4" && x.WeekEndDate == new DateTime(2020, 1, 12) && x.TotalHours == 50 && x.TotalGross == 750).Count());
		}

		[TestMethod]
		public void FromList_GroupsByEmployeeId()
		{
			var plantPayLines = _context.PlantPayLines.Where(x => x.BatchId == 1).ToList();
			var plantSummaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			Assert.AreEqual(4, plantSummaries.Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee1").Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee2").Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee3").Count());
			Assert.AreEqual(1, plantSummaries.Where(x => x.EmployeeId == "Employee4").Count());
		}
	}
}
