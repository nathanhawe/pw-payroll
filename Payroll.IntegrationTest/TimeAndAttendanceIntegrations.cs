using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using Payroll.Service;
using System;

namespace Payroll.IntegrationTest
{
	[TestClass]
	public class TimeAndAttendanceIntegrations
	{
		[TestMethod]
		[Ignore("Only Run To Change State")]
		public void RunPlantsProcess()
		{
			var weekEndingDate = new DateTime(2020, 6, 28);

			// Database context
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = PayrollData")
				.Options;

			using var context = new PayrollContext(options);
			
			// Loggers
			var apiLogger = new MockLogger<QuickBase.Api.QuickBaseConnection>();
			var serviceLogger = new MockLogger<TimeAndAttendanceService>();


			// Quick Base Connection
			var configuration = ConfigurationHelper.GetIConfigurationRoot();
			var realm = configuration["QuickBase:Realm"];
			var userToken = configuration["QuickBase:UserToken"];
			var quickBaseConnection = new QuickBase.Api.QuickBaseConnection(realm, userToken, apiLogger);

			// Repositories
			var pslTrackingDailyRepo = new PslTrackingDailyRepo(quickBaseConnection);
			var crewBossPayRepo = new CrewBossPayRepo(quickBaseConnection);
			var ranchPayrollRepo = new RanchPayrollRepo(quickBaseConnection);
			var ranchPayrollAdjustmentRepo = new RanchPayrollAdjustmentRepo(quickBaseConnection);
			var ranchSummariesRepo = new RanchSummariesRepo(quickBaseConnection);
			var plantPayrollRepo = new PlantPayrollRepo(quickBaseConnection);
			var plantPayrollAdjustmentRepo = new PlantPayrollAdjustmentRepo(quickBaseConnection);
			var plantSummariesRepo = new PlantSummariesRepo(quickBaseConnection);

			// Services
			var minimumWageService = new MinimumWageService(context);
			var crewLaborWageService = new CrewLaborWageService(context);
			var roundingService = new RoundingService();
			var grossFromHoursCalculator = new GrossFromHoursCalculator(new RanchHourlyRateSelector(new CrewLaborWageService(context)), new PlantHourlyRateSelector(minimumWageService), roundingService);
			var grossFromPiecesCalculator = new GrossFromPiecesCalculator(roundingService);
			var grossFromIncentiveCalculator = new GrossFromIncentiveCalculator(roundingService);
			var totalGrossCalculator = new TotalGrossCalculator(roundingService);
			var dailySummaryCalculator = new DailySummaryCalculator(context, minimumWageService, roundingService, crewLaborWageService);
			

			var paidSickLeaveService = new PaidSickLeaveService(context, roundingService);
			var crewBossPayService = new CrewBossPayService(context, new CrewBossWageService(context), roundingService);
			var ranchDailyOTDTHoursCalculator = new RanchDailyOTDTHoursCalculator();
			var ranchWeeklySummaryCalculator = new RanchWeeklySummaryCalculator(roundingService);
			var ranchWeeklyOverTimeHoursCalculator = new RanchWeeklyOTHoursCalculator(roundingService);
			var ranchMinimumMakeUpCalculator = new RanchMinimumMakeUpCalculator(roundingService);
			var ranchSummaryService = new RanchSummaryService(context);

			var plantDailyOTDTHoursCalculator = new PlantDailyOTDTHoursCalculator();
			var plantWeeklySummaryCalculator = new PlantWeeklySummaryCalculator(roundingService);
			var plantWeeklyOverTimeHoursCalculator = new PlantWeeklyOTHoursCalculator(roundingService);
			var plantMinimumMakeUpCalculator = new PlantMinimumMakeUpCalculator(roundingService);
			var plantSummaryService = new PlantSummaryService(context);

			var service = new TimeAndAttendanceService(
				serviceLogger,
				context,
				pslTrackingDailyRepo,
				crewBossPayRepo,
				ranchPayrollRepo,
				ranchPayrollAdjustmentRepo,
				ranchSummariesRepo,
				plantPayrollRepo,
				plantPayrollAdjustmentRepo,
				plantSummariesRepo,
				grossFromHoursCalculator,
				grossFromPiecesCalculator,
				grossFromIncentiveCalculator,
				totalGrossCalculator,
				dailySummaryCalculator,
				roundingService,
				paidSickLeaveService,
				crewBossPayService,
				ranchDailyOTDTHoursCalculator,
				ranchWeeklySummaryCalculator,
				ranchWeeklyOverTimeHoursCalculator,
				ranchMinimumMakeUpCalculator,
				ranchSummaryService,
				plantDailyOTDTHoursCalculator,
				plantWeeklySummaryCalculator,
				plantWeeklyOverTimeHoursCalculator,
				plantMinimumMakeUpCalculator,
				plantSummaryService);

			// Create a new batch
			var batch = new Batch
			{
				WeekEndDate = weekEndingDate,
				LayoffId = null,
				Company = Payroll.Domain.Constants.Company.Plants
			};
			context.Add(batch);
			context.SaveChanges();

			// Batch should now have an Id and the process can be executed
			service.PerformCalculations(batch.Id);
		}

		[TestMethod]
		[Ignore("Only Run To Change State")]
		public void RunRanchesProcess()
		{
			var weekEndingDate = new DateTime(2020, 6, 28);

			// Database context
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = PayrollData")
				.Options;

			using var context = new PayrollContext(options);

			// Loggers
			var apiLogger = new MockLogger<QuickBase.Api.QuickBaseConnection>();
			var serviceLogger = new MockLogger<TimeAndAttendanceService>();

			// Quick Base Connection
			var configuration = ConfigurationHelper.GetIConfigurationRoot();
			var realm = configuration["QuickBase:Realm"];
			var userToken = configuration["QuickBase:UserToken"];
			var quickBaseConnection = new QuickBase.Api.QuickBaseConnection(realm, userToken, apiLogger);

			// Repositories
			var pslTrackingDailyRepo = new PslTrackingDailyRepo(quickBaseConnection);
			var crewBossPayRepo = new CrewBossPayRepo(quickBaseConnection);
			var ranchPayrollRepo = new RanchPayrollRepo(quickBaseConnection);
			var ranchPayrollAdjustmentRepo = new RanchPayrollAdjustmentRepo(quickBaseConnection);
			var ranchSummariesRepo = new RanchSummariesRepo(quickBaseConnection);
			var plantPayrollRepo = new PlantPayrollRepo(quickBaseConnection);
			var plantPayrollAdjustmentRepo = new PlantPayrollAdjustmentRepo(quickBaseConnection);
			var plantSummariesRepo = new PlantSummariesRepo(quickBaseConnection);

			// Services
			var minimumWageService = new MinimumWageService(context);
			var crewLaborWageService = new CrewLaborWageService(context);
			var roundingService = new RoundingService();
			var grossFromHoursCalculator = new GrossFromHoursCalculator(new RanchHourlyRateSelector(new CrewLaborWageService(context)), new PlantHourlyRateSelector(minimumWageService), roundingService);
			var grossFromPiecesCalculator = new GrossFromPiecesCalculator(roundingService);
			var grossFromIncentiveCalculator = new GrossFromIncentiveCalculator(roundingService);
			var totalGrossCalculator = new TotalGrossCalculator(roundingService);
			var dailySummaryCalculator = new DailySummaryCalculator(context, minimumWageService, roundingService, crewLaborWageService);


			var paidSickLeaveService = new PaidSickLeaveService(context, roundingService);
			var crewBossPayService = new CrewBossPayService(context, new CrewBossWageService(context), roundingService);
			var ranchDailyOTDTHoursCalculator = new RanchDailyOTDTHoursCalculator();
			var ranchWeeklySummaryCalculator = new RanchWeeklySummaryCalculator(roundingService);
			var ranchWeeklyOverTimeHoursCalculator = new RanchWeeklyOTHoursCalculator(roundingService);
			var ranchMinimumMakeUpCalculator = new RanchMinimumMakeUpCalculator(roundingService);
			var ranchSummaryService = new RanchSummaryService(context);

			var plantDailyOTDTHoursCalculator = new PlantDailyOTDTHoursCalculator();
			var plantWeeklySummaryCalculator = new PlantWeeklySummaryCalculator(roundingService);
			var plantWeeklyOverTimeHoursCalculator = new PlantWeeklyOTHoursCalculator(roundingService);
			var plantMinimumMakeUpCalculator = new PlantMinimumMakeUpCalculator(roundingService);
			var plantSummaryService = new PlantSummaryService(context);

			var service = new TimeAndAttendanceService(
				serviceLogger,
				context,
				pslTrackingDailyRepo,
				crewBossPayRepo,
				ranchPayrollRepo,
				ranchPayrollAdjustmentRepo,
				ranchSummariesRepo,
				plantPayrollRepo,
				plantPayrollAdjustmentRepo,
				plantSummariesRepo,
				grossFromHoursCalculator,
				grossFromPiecesCalculator,
				grossFromIncentiveCalculator,
				totalGrossCalculator,
				dailySummaryCalculator,
				roundingService,
				paidSickLeaveService,
				crewBossPayService,
				ranchDailyOTDTHoursCalculator,
				ranchWeeklySummaryCalculator,
				ranchWeeklyOverTimeHoursCalculator,
				ranchMinimumMakeUpCalculator,
				ranchSummaryService,
				plantDailyOTDTHoursCalculator,
				plantWeeklySummaryCalculator,
				plantWeeklyOverTimeHoursCalculator,
				plantMinimumMakeUpCalculator,
				plantSummaryService);

			// Create a new batch
			var batch = new Batch
			{
				WeekEndDate = weekEndingDate,
				LayoffId = null,
				Company = Payroll.Domain.Constants.Company.Ranches
			};
			context.Add(batch);
			context.SaveChanges();

			// Batch should now have an Id and the process can be executed
			service.PerformCalculations(batch.Id);
		}

		[TestMethod]
		[Ignore("Only Run To Change State")]
		public void CreatePlantSummaries()
		{
			var weekEndingDate = new DateTime(2020, 7, 19);
			var layoffId = 1129;

			// Database context
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = PayrollData")
				.Options;

			using var context = new PayrollContext(options);

			// Loggers
			var apiLogger = new MockLogger<QuickBase.Api.QuickBaseConnection>();
			var serviceLogger = new MockLogger<SummaryCreationService>();


			// Quick Base Connection
			var configuration = ConfigurationHelper.GetIConfigurationRoot();
			var realm = configuration["QuickBase:Realm"];
			var userToken = configuration["QuickBase:UserToken"];
			var quickBaseConnection = new QuickBase.Api.QuickBaseConnection(realm, userToken, apiLogger);

			// Repositories
			var ranchPayrollRepo = new RanchPayrollRepo(quickBaseConnection);
			var ranchSummariesRepo = new RanchSummariesRepo(quickBaseConnection);
			var plantPayrollRepo = new PlantPayrollRepo(quickBaseConnection);
			var plantSummariesRepo = new PlantSummariesRepo(quickBaseConnection);

			// Services
			var ranchSummaryService = new RanchSummaryService(context);
			var plantSummaryService = new PlantSummaryService(context);

			var service = new SummaryCreationService(
				serviceLogger,
				context,
				ranchPayrollRepo,
				ranchSummariesRepo,
				plantPayrollRepo,
				plantSummariesRepo,
				ranchSummaryService,
				plantSummaryService);

			// Create a new batch
			var summaryBatch = new SummaryBatch
			{
				WeekEndDate = weekEndingDate,
				LayoffId = layoffId,
				Company = Payroll.Domain.Constants.Company.Plants
			};
			context.Add(summaryBatch);
			context.SaveChanges();

			// Batch should now have an Id and the process can be executed
			service.CreateSummaries(summaryBatch.Id);
		}

		[TestMethod]
		[Ignore("Only Run To Change State")]
		public void CreateRanchSummaries()
		{
			var weekEndingDate = new DateTime(2020, 7, 19);
			var layoffId = 1143;

			// Database context
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = PayrollData")
				.Options;

			using var context = new PayrollContext(options);

			// Loggers
			var apiLogger = new MockLogger<QuickBase.Api.QuickBaseConnection>();
			var serviceLogger = new MockLogger<SummaryCreationService>();


			// Quick Base Connection
			var configuration = ConfigurationHelper.GetIConfigurationRoot();
			var realm = configuration["QuickBase:Realm"];
			var userToken = configuration["QuickBase:UserToken"];
			var quickBaseConnection = new QuickBase.Api.QuickBaseConnection(realm, userToken, apiLogger);

			// Repositories
			var ranchPayrollRepo = new RanchPayrollRepo(quickBaseConnection);
			var ranchSummariesRepo = new RanchSummariesRepo(quickBaseConnection);
			var plantPayrollRepo = new PlantPayrollRepo(quickBaseConnection);
			var plantSummariesRepo = new PlantSummariesRepo(quickBaseConnection);

			// Services
			var ranchSummaryService = new RanchSummaryService(context);
			var plantSummaryService = new PlantSummaryService(context);

			var service = new SummaryCreationService(
				serviceLogger,
				context,
				ranchPayrollRepo,
				ranchSummariesRepo,
				plantPayrollRepo,
				plantSummariesRepo,
				ranchSummaryService,
				plantSummaryService);

			// Create a new batch
			var summaryBatch = new SummaryBatch
			{
				WeekEndDate = weekEndingDate,
				LayoffId = layoffId,
				Company = Payroll.Domain.Constants.Company.Ranches
			};
			context.Add(summaryBatch);
			context.SaveChanges();

			// Batch should now have an Id and the process can be executed
			service.CreateSummaries(summaryBatch.Id);
		}


		[TestMethod]
		[Ignore("Only Run To Change State")]
		public void AuditLock_Plants()
		{
			var weekEndingDate = new DateTime(2020, 8, 16);
			var layoffId = 0;
			bool auditLock = true;

			// Database context
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = PayrollData")
				.Options;

			using var context = new PayrollContext(options);

			// Loggers
			var apiLogger = new MockLogger<QuickBase.Api.QuickBaseConnection>();
			var serviceLogger = new MockLogger<AuditLockService>();


			// Quick Base Connection
			var configuration = ConfigurationHelper.GetIConfigurationRoot();
			var realm = configuration["QuickBase:Realm"];
			var userToken = configuration["QuickBase:UserToken"];
			var quickBaseConnection = new QuickBase.Api.QuickBaseConnection(realm, userToken, apiLogger);

			// Repositories
			var ranchPayrollRepo = new RanchPayrollRepo(quickBaseConnection);
			var plantPayrollRepo = new PlantPayrollRepo(quickBaseConnection);

			var service = new AuditLockService(
				serviceLogger,
				context,
				ranchPayrollRepo,
				plantPayrollRepo);

			// Create a new batch
			var auditLockBatch = new AuditLockBatch
			{
				WeekEndDate = weekEndingDate,
				LayoffId = layoffId,
				Company = Payroll.Domain.Constants.Company.Plants,
				Lock = auditLock
			};
			context.Add(auditLockBatch);
			context.SaveChanges();

			// Batch should now have an Id and the process can be executed
			service.ProcessAuditLockBatch(auditLockBatch.Id);
		}

		[TestMethod]
		[Ignore("Only Run To Change State")]
		public void AuditLock_Ranches()
		{
			var weekEndingDate = new DateTime(2020, 8, 9);
			var layoffId = 0;
			bool auditLock = true;

			// Database context
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = PayrollData")
				.Options;

			using var context = new PayrollContext(options);

			// Loggers
			var apiLogger = new MockLogger<QuickBase.Api.QuickBaseConnection>();
			var serviceLogger = new MockLogger<AuditLockService>();


			// Quick Base Connection
			var configuration = ConfigurationHelper.GetIConfigurationRoot();
			var realm = configuration["QuickBase:Realm"];
			var userToken = configuration["QuickBase:UserToken"];
			var quickBaseConnection = new QuickBase.Api.QuickBaseConnection(realm, userToken, apiLogger);

			// Repositories
			var ranchPayrollRepo = new RanchPayrollRepo(quickBaseConnection);
			var plantPayrollRepo = new PlantPayrollRepo(quickBaseConnection);

			var service = new AuditLockService(
				serviceLogger,
				context,
				ranchPayrollRepo,
				plantPayrollRepo);

			// Create a new batch
			var auditLockBatch = new AuditLockBatch
			{
				WeekEndDate = weekEndingDate,
				LayoffId = layoffId,
				Company = Payroll.Domain.Constants.Company.Ranches,
				Lock = auditLock
			};
			context.Add(auditLockBatch);
			context.SaveChanges();

			// Batch should now have an Id and the process can be executed
			service.ProcessAuditLockBatch(auditLockBatch.Id);
		}
	}
}
