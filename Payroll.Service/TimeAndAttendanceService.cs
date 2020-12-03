using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Manages the calculation workflow
	/// </summary>
	public class TimeAndAttendanceService : ITimeAndAttendanceService
	{
		private readonly ILogger<TimeAndAttendanceService> _logger;

		// Database context
		private readonly Data.PayrollContext _context;

		// Repositories
		private readonly IPslTrackingDailyRepo _pslTrackingDailyRepo;
		private readonly ICrewBossPayRepo _crewBossPayRepo;
		private readonly IRanchPayrollRepo _ranchPayrollRepo;
		private readonly IRanchPayrollAdjustmentRepo _ranchPayrollAdjustmentRepo;
		private readonly IRanchSummariesRepo _ranchSummariesRepo;
		private readonly IPlantPayrollRepo _plantPayrollRepo;
		private readonly IPlantPayrollAdjustmentRepo _plantPayrollAdjustmentRepo;
		private readonly IPlantSummariesRepo _plantSummariesRepo;
		private readonly IRanchPayrollOutRepo _ranchPayrollOutRepo;
		private readonly IRanchPayrollAdjustmentOutRepo _ranchPayrollAdjustmentOutRepo;
		private readonly IPlantPayrollOutRepo _plantPayrollOutRepo;
		private readonly IPlantPayrollAdjustmentOutRepo _plantPayrollAdjustmentOutRepo;

		// Services
		private readonly IGrossFromHoursCalculator _grossFromHoursCalculator;
		private readonly IGrossFromPiecesCalculator _grossFromPiecesCalculator;
		private readonly IGrossFromIncentiveCalculator _grossFromIncentiveCalculator;
		private readonly ITotalGrossCalculator _totalGrossCalculator;
		private readonly IDailySummaryCalculator _dailySummaryCalculator;
		private readonly IRoundingService _roundingService;

		private readonly IPaidSickLeaveService _paidSickLeaveService;
		private readonly ICrewBossPayService _crewBossPayService;
		private readonly IRanchDailyOTDTHoursCalculator _ranchDailyOTDTHoursCalculator;
		private readonly IRanchWeeklySummaryCalculator _ranchWeeklySummaryCalculator;
		private readonly IRanchWeeklyOTHoursCalculator _ranchWeeklyOverTimeHoursCalculator;
		private readonly IRanchMinimumMakeUpCalculator _ranchMinimumMakeUpCalculator;
		private readonly IRanchSummaryService _ranchSummaryService;

		private readonly IPlantDailyOTDTHoursCalculator _plantDailyOTDTHoursCalculator;
		private readonly IPlantWeeklySummaryCalculator _plantWeeklySummaryCalculator;
		private readonly IPlantWeeklyOTHoursCalculator _plantWeeklyOverTimeHoursCalculator;
		private readonly IPlantMinimumMakeUpCalculator _plantMinimumMakeUpCalculator;
		private readonly IPlantSummaryService _plantSummaryService;

		public TimeAndAttendanceService(
			ILogger<TimeAndAttendanceService> logger,
			Data.PayrollContext payrollContext,
			IPslTrackingDailyRepo pslTrackingDailyRepo,
			ICrewBossPayRepo crewBossPayRepo,
			IRanchPayrollRepo ranchPayrollRepo,
			IRanchPayrollAdjustmentRepo ranchPayrollAdjustmentRepo,
			IRanchSummariesRepo ranchSummariesRepo,
			IPlantPayrollRepo plantPayrollRepo,
			IPlantPayrollAdjustmentRepo plantPayrollAdjustmentRepo,
			IPlantSummariesRepo plantSummariesRepo,
			IGrossFromHoursCalculator grossFromHoursCalculator,
			IGrossFromPiecesCalculator grossFromPiecesCalculator,
			IGrossFromIncentiveCalculator grossFromIncentiveCalculator,
			ITotalGrossCalculator totalGrossCalculator,
			IDailySummaryCalculator dailySummaryCalculator,
			IRoundingService roundingService,
			IPaidSickLeaveService paidSickLeaveService,
			ICrewBossPayService crewBossPayService,
			IRanchDailyOTDTHoursCalculator ranchDailyOTDTHoursCalculator,
			IRanchWeeklySummaryCalculator ranchWeeklySummaryCalculator,
			IRanchWeeklyOTHoursCalculator ranchWeeklyOTHoursCalculator,
			IRanchMinimumMakeUpCalculator ranchMinimumMakeUpCalculator,
			IRanchSummaryService ranchSummaryService,
			IPlantDailyOTDTHoursCalculator plantDailyOTDTHoursCalculator,
			IPlantWeeklySummaryCalculator plantWeeklySummaryCalculator,
			IPlantWeeklyOTHoursCalculator plantWeeklyOTHoursCalculator,
			IPlantMinimumMakeUpCalculator plantMinimumMakeUpCalculator,
			IPlantSummaryService plantSummaryService,
			IRanchPayrollOutRepo ranchPayrollOutRepo,
			IRanchPayrollAdjustmentOutRepo ranchPayrollAdjustmentOutRepo,
			IPlantPayrollOutRepo plantPayrollOutRepo,
			IPlantPayrollAdjustmentOutRepo plantPayrollAdjustmentOutRepo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_context = payrollContext ?? throw new ArgumentNullException(nameof(payrollContext));
			_pslTrackingDailyRepo = pslTrackingDailyRepo ?? throw new ArgumentNullException(nameof(pslTrackingDailyRepo));
			_crewBossPayRepo = crewBossPayRepo ?? throw new ArgumentNullException(nameof(crewBossPayRepo));
			_ranchPayrollRepo = ranchPayrollRepo ?? throw new ArgumentNullException(nameof(ranchPayrollRepo));
			_ranchPayrollAdjustmentRepo = ranchPayrollAdjustmentRepo ?? throw new ArgumentNullException(nameof(ranchPayrollAdjustmentRepo));
			_ranchSummariesRepo = ranchSummariesRepo ?? throw new ArgumentNullException(nameof(ranchSummariesRepo));
			_plantPayrollRepo = plantPayrollRepo ?? throw new ArgumentNullException(nameof(plantPayrollRepo));
			_plantPayrollAdjustmentRepo = plantPayrollAdjustmentRepo ?? throw new ArgumentNullException(nameof(plantPayrollAdjustmentRepo));
			_plantSummariesRepo = plantSummariesRepo ?? throw new ArgumentNullException(nameof(plantSummariesRepo));
			_grossFromHoursCalculator = grossFromHoursCalculator ?? throw new ArgumentNullException(nameof(grossFromHoursCalculator));
			_grossFromPiecesCalculator = grossFromPiecesCalculator ?? throw new ArgumentNullException(nameof(grossFromPiecesCalculator));
			_grossFromIncentiveCalculator = grossFromIncentiveCalculator ?? throw new ArgumentNullException(nameof(grossFromIncentiveCalculator));
			_totalGrossCalculator = totalGrossCalculator ?? throw new ArgumentNullException(nameof(totalGrossCalculator));
			_dailySummaryCalculator = dailySummaryCalculator ?? throw new ArgumentNullException(nameof(dailySummaryCalculator));
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
			_paidSickLeaveService = paidSickLeaveService ?? throw new ArgumentNullException(nameof(paidSickLeaveService));
			_crewBossPayService = crewBossPayService ?? throw new ArgumentNullException(nameof(crewBossPayService));
			_ranchDailyOTDTHoursCalculator = ranchDailyOTDTHoursCalculator ?? throw new ArgumentNullException(nameof(ranchDailyOTDTHoursCalculator));
			_ranchWeeklySummaryCalculator = ranchWeeklySummaryCalculator ?? throw new ArgumentNullException(nameof(ranchWeeklySummaryCalculator));
			_ranchWeeklyOverTimeHoursCalculator = ranchWeeklyOTHoursCalculator ?? throw new ArgumentNullException(nameof(ranchWeeklyOTHoursCalculator));
			_ranchMinimumMakeUpCalculator = ranchMinimumMakeUpCalculator ?? throw new ArgumentNullException(nameof(ranchMinimumMakeUpCalculator));
			_ranchSummaryService = ranchSummaryService ?? throw new ArgumentNullException(nameof(ranchSummaryService));
			_plantDailyOTDTHoursCalculator = plantDailyOTDTHoursCalculator ?? throw new ArgumentNullException(nameof(plantDailyOTDTHoursCalculator));
			_plantWeeklySummaryCalculator = plantWeeklySummaryCalculator ?? throw new ArgumentNullException(nameof(plantWeeklySummaryCalculator));
			_plantWeeklyOverTimeHoursCalculator = plantWeeklyOTHoursCalculator ?? throw new ArgumentNullException(nameof(plantWeeklyOTHoursCalculator));
			_plantMinimumMakeUpCalculator = plantMinimumMakeUpCalculator ?? throw new ArgumentNullException(nameof(plantMinimumMakeUpCalculator));
			_plantSummaryService = plantSummaryService ?? throw new ArgumentNullException(nameof(plantSummaryService));
			_ranchPayrollOutRepo = ranchPayrollOutRepo ?? throw new ArgumentNullException(nameof(ranchPayrollOutRepo));
			_ranchPayrollAdjustmentOutRepo = ranchPayrollAdjustmentOutRepo ?? throw new ArgumentNullException(nameof(ranchPayrollAdjustmentOutRepo));
			_plantPayrollOutRepo = plantPayrollOutRepo ?? throw new ArgumentNullException(nameof(plantPayrollOutRepo));
			_plantPayrollAdjustmentOutRepo = plantPayrollAdjustmentOutRepo ?? throw new ArgumentNullException(nameof(plantPayrollAdjustmentOutRepo));
		}

		public void PerformCalculations(int batchId)
		{
			try
			{
				_logger.Log(LogLevel.Information, "Starting calculations for batch {batchId}", batchId);
				var batch = _context.Batches.Where(x => x.Id == batchId).FirstOrDefault();
				if (batch == null) throw new Exception($"The provided batch ID of '{batchId}' was not found in the database.");
				SetBatchStatus(batchId, BatchProcessingStatus.Starting);

				switch (batch.Company)
				{
					case Company.Plants: PerformPlantCalculations(batch); break;
					case Company.Ranches: PerformRanchCalculations(batch); break;
					default: throw new Exception($"Unknown company value '{batch.Company}'.");
				}
				_logger.Log(LogLevel.Information, "Completed calculations for batch {batchId}", batchId);
				SetBatchStatus(batchId, BatchProcessingStatus.Success);
			}
			catch(Exception ex)
			{
				SetBatchStatus(batchId, BatchProcessingStatus.Failed);
				throw ex;
			}
		}

		private void PerformPlantCalculations(Batch batch)
		{
			var company = Company.Plants;

			/* Download Quick Base data */
			SetBatchStatus(batch.Id, BatchProcessingStatus.Downloading);
			CopyPlantDataFromQuickBase(batch);

			_logger.Log(LogLevel.Information, "Calculating PSL for batch {batchId}", batch.Id);
			/* PSL Requires hours and gross for regular, piece; also requires hours on Sick Leave pay types*/
			SetBatchStatus(batch.Id, BatchProcessingStatus.PaidSickLeaveCalculations);
	
			// Perform PSL calculations
			DateTime? startDate = _context.PlantPayLines.Where(x => x.BatchId == batch.Id).OrderBy(s => s.ShiftDate).FirstOrDefault()?.ShiftDate;
			DateTime? endDate = _context.PlantPayLines.Where(x => x.BatchId == batch.Id).OrderByDescending(s => s.ShiftDate).FirstOrDefault()?.ShiftDate;

			if(startDate != null && endDate != null)
			{
				_paidSickLeaveService.UpdateTracking(batch.Id, company);
				_paidSickLeaveService.CalculateNinetyDay(batch.Id, company, startDate.Value, endDate.Value);

				// Update PSL usage
				_paidSickLeaveService.UpdateUsage(batch.Id, company);
			}
			

			// Set PSL/Covid19 rate and recalculate gross.
			var paidSickLeaves = _context.PlantPayLines
				.Where(x =>
					x.BatchId == batch.Id
					&&
					(
						x.PayType == PayType.SickLeave
						|| x.PayType == PayType.Covid19)
					)
				.ToList();
			paidSickLeaves.ForEach(x => x.HourlyRate = _paidSickLeaveService.GetNinetyDayRate(batch.Id, Company.Plants, x.EmployeeId, x.ShiftDate));
			_grossFromHoursCalculator.CalculateGrossFromHours(paidSickLeaves);
			_totalGrossCalculator.CalculateTotalGross(paidSickLeaves);
			_context.SaveChanges();

			_logger.Log(LogLevel.Information, "Calculating OT/DT/WOT/Seventh Day for batch {batchId}", batch.Id);
			/* OT/DT/Seventh Day Hours */
			SetBatchStatus(batch.Id, BatchProcessingStatus.AdditionalCalculations);
			// Daily summaries group all of the plant pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of plant sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummaries(batch.Id, company);

			// Calculate OT/DT/7th Day Hours
			// This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
			_plantDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			// Minimum Make Up is made by comparing the effective rate against minimum wage.  If minimum wage is greater than the effective rate, the difference
			// should be used to create a minimum make up line and the higher of the two rates is used for OT, DT, etc.  Plants calculates minimum makeup on a daily
			// basis in contrast to Ranches which performs the calculation using weekly effective rates.
			var minimumMakeUps = _plantMinimumMakeUpCalculator.GetMinimumMakeUps(dailySummaries);
			var minimumMakeUpRecords = minimumMakeUps.Select(x => new PlantPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross,
				BatchId = batch.Id
			}).ToList();
			_totalGrossCalculator.CalculateTotalGross(minimumMakeUpRecords);
			BulkInsert(minimumMakeUpRecords);


			// Create Weekly Summaries groups all of the daily summaries by Employee, Week End Date, and Minimum Wage and summarizes the
			// different types of hours for the week.  This information is used to figure out the effective hourly rate and create minimum
			// assurance lines.
			var weeklySummaries = _plantWeeklySummaryCalculator.GetWeeklySummary(dailySummaries, minimumMakeUps);


			/* WOT Hours */
			var weeklyOt = _plantWeeklyOverTimeHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			/* OT/DT Gross (Requires effective weekly rate) */
			var overTimeRecords = dailySummaries.Where(x => x.OverTimeHours > 0).Select(x => new PlantPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.OverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batch.Id
			}).ToList();
			overTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			BulkInsert(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new PlantPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.DoubleTime,
				OtDtWotHours = x.DoubleTimeHours,
				BatchId = batch.Id
			}).ToList();
			doubleTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.OtDtWotRate = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			BulkInsert(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new PlantPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				Plant = x.Crew,
				PayType = PayType.WeeklyOverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batch.Id
			}).ToList();
			weeklyOverTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			BulkInsert(weeklyOverTimeRecords);

			_logger.Log(LogLevel.Information, "Updating Reporting/Comp/NPT hourly rates for batch {batchId}", batch.Id);
			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			//var reportingPayRecords = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			var reportingPayRecords = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && x.PayType == PayType.ReportingPay).ToList();
			reportingPayRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && (x.LaborCode == (int)PlantLaborCode.RecoveryTime || x.LaborCode == (int)PlantLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();

			/* Calculate Adjustments */
			SetBatchStatus(batch.Id, BatchProcessingStatus.Adjustments);
			CalculatePlantAdjustments(batch.Id, company);

			/* Update records to Quick Base */
			SetBatchStatus(batch.Id, BatchProcessingStatus.Uploading);
			_logger.Log(LogLevel.Information, "Updating Quick Base for batch {batchId}", batch.Id);

			// Purge output table records for same weekending date and layoff ID before uploading
			var purgeResponse = _plantPayrollOutRepo.Delete(batch.WeekEndDate, batch.LayoffId ?? 0);
			var adjPurgeResponse = _plantPayrollAdjustmentOutRepo.Delete(batch.WeekEndDate, batch.LayoffId ?? 0);

			// Plant Payroll Records
			var toPlantPayroll = _context.PlantPayLines.Where(x => x.BatchId == batch.Id).ToList();
			if (batch.LayoffId != null) toPlantPayroll.ForEach(x => x.LayoffId = batch.LayoffId.Value);
			var ppResponse = _plantPayrollRepo.Save(toPlantPayroll);
			var ppOutResponse = _plantPayrollOutRepo.Save(toPlantPayroll);

			// Plant Adjustment Records
			var toPlantAdjustments = _context.PlantAdjustmentLines.Where(x => x.BatchId == batch.Id).ToList();
			if (batch.LayoffId != null) toPlantAdjustments.ForEach(x => x.LayoffId = batch.LayoffId.Value);
			var ppaResponse = _plantPayrollAdjustmentRepo.Save(toPlantAdjustments);
			var ppaOutResponse = _plantPayrollAdjustmentOutRepo.Save(toPlantAdjustments);

			// PSL Updates
			var pslResponse = _pslTrackingDailyRepo.Save(_context.PaidSickLeaves.Where(x =>
				x.BatchId == batch.Id
				&& x.ShiftDate >= batch.WeekEndDate.AddDays(-6)
				&& x.ShiftDate <= batch.WeekEndDate).ToList());
		}

		private void CopyPlantDataFromQuickBase(Batch batch)
		{
			// Plant Payroll
			_logger.Log(LogLevel.Information, "Downloading plant payroll data from Quick Base for {batchId}", batch.Id);
			var plantPayLines = _plantPayrollRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			plantPayLines = plantPayLines.Where(x => x.PayType != PayType.SpecialAdjustment || x.SpecialAdjustmentApproved).ToList();
			plantPayLines.ForEach(x => {
				x.BatchId = batch.Id;
				x.HoursWorked = _roundingService.Round(x.HoursWorked, 2);
			});

			// Plant Adjustment
			_logger.Log(LogLevel.Information, "Downloading plant adjustment data from Quick Base for {batchId}", batch.Id);
			var plantAdjustmentLines = _plantPayrollAdjustmentRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			plantAdjustmentLines.ForEach(x => x.BatchId = batch.Id);

			// Plant PSL Tracking
			_logger.Log(LogLevel.Information, "Downloading plant paid sick leave data from Quick Base for {batchId}", batch.Id);
			List<PaidSickLeave> paidSickLeaves = GetPaidSickLeaveTracking(batch.WeekEndDate, batch.Company);
			paidSickLeaves.ForEach(x => x.BatchId = batch.Id);

			/* Gross Calculations */
			_logger.Log(LogLevel.Information, "Calculating initial gross for batch {batchId}", batch.Id);
			SetBatchStatus(batch.Id, BatchProcessingStatus.GrossCalculations);

			// Hourly
			var hourlyLines = plantPayLines.Where(x => x.BatchId == batch.Id &&
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.Bereavement))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);

			// Pieces
			var pieceLines = plantPayLines.Where(x => x.BatchId == batch.Id && x.PayType == PayType.Pieces).ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);

			// Incentives!!!
			var incentiveLines = plantPayLines.Where(x => x.BatchId == batch.Id && (
				x.LaborCode == (int)PlantLaborCode.TallyTagWriter
				|| (x.PayType == PayType.Pieces && !x.IsIncentiveDisqualified)
				|| x.NonDiscretionaryBonusRate > 0)).ToList();
			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(incentiveLines);

			// Total
			var payLines = plantPayLines.Where(x => x.BatchId == batch.Id).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);

			_logger.Log(LogLevel.Information, "Saving plant data from Quick Base for {batchId}", batch.Id);
			BulkInsert(plantPayLines);
			BulkInsert(plantAdjustmentLines);
			BulkInsert(paidSickLeaves);
			_logger.Log(LogLevel.Information, "Completed save of plant data from Quick Base for {batchId}", batch.Id);
		}
				

		private List<PaidSickLeave> GetPaidSickLeaveTracking(DateTime weekEndDate, string company)
		{
			return _pslTrackingDailyRepo.Get(weekEndDate.AddDays(-91), weekEndDate.AddDays(-1), company).ToList();
		}

		private void CalculatePlantAdjustments(int batchId, string company)
		{
			_logger.Log(LogLevel.Information, "Calculating plant adjustments for {batchId}", batchId);

			DateTime weekendOfAdjustmentPaid = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId).OrderByDescending(x => x.WeekEndOfAdjustmentPaid).FirstOrDefault()?.WeekEndOfAdjustmentPaid ?? new DateTime(2000, 1, 1);

			/* Gross Calculations */
			// Hourly
			// Include sick leave AND COVID19 to be calculated as we expect to be provided an "Old Hourly Rate" instead of figuring
			// out the rate ourselves.
			var hourlyLines = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId &&
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.SickLeave
					|| x.PayType == PayType.Bereavement
					|| x.PayType == PayType.Covid19))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && x.PayType == PayType.Pieces).ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// There is no incentive calculation in adjustments at this time!
			// The previously calculated gross from incentive is loaded into a data entry field and the value must be manually adjusted in the
			// "New" record.

			// Total
			var payLines = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.UpdateRange(payLines);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
			// Daily summaries group all of the plant adjustment lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of plant sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummariesFromAdjustments(batchId, company);

			// Calculate OT/DT/7th Day Hours
			// This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
			_plantDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			// Minimum Make Up is made by comparing the effective rate against minimum wage.  If minimum wage is greater than the effective rate, the difference
			// should be used to create a minimum make up line and the higher of the two rates is used for OT, DT, etc.  Plants calculates minimum makeup on a daily
			// basis in contrast to Ranches which performs the calculation using weekly effective rates.
			var minimumMakeUps = _plantMinimumMakeUpCalculator.GetMinimumMakeUps(dailySummaries);
			var minimumMakeUpRecords = minimumMakeUps.Select(x => new PlantAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			_totalGrossCalculator.CalculateTotalGross(minimumMakeUpRecords);
			_context.AddRange(minimumMakeUpRecords);


			// Create Weekly Summaries groups all of the daily summaries by Employee, Week End Date, and Minimum Wage and summarizes the
			// different types of hours for the week.  This information is used to figure out the effective hourly rate and create minimum
			// assurance lines.
			var weeklySummaries = _plantWeeklySummaryCalculator.GetWeeklySummary(dailySummaries, minimumMakeUps);


			/* WOT Hours */
			var weeklyOt = _plantWeeklyOverTimeHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			/* OT/DT Gross (Requires effective weekly rate) */
			var overTimeRecords = dailySummaries.Where(x => x.OverTimeHours > 0).Select(x => new PlantAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.OverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			overTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			_context.AddRange(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new PlantAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.DoubleTime,
				OtDtWotHours = x.DoubleTimeHours,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid,
				BatchId = batchId
			}).ToList();
			doubleTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round((weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			_context.AddRange(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new PlantAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				Plant = x.Crew,
				PayType = PayType.WeeklyOverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			weeklyOverTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			_context.AddRange(weeklyOverTimeRecords);


			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			//var reportingPayRecords = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			var reportingPayRecords = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && x.PayType == PayType.ReportingPay).ToList();
			reportingPayRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && (x.LaborCode == (int)PlantLaborCode.RecoveryTime || x.LaborCode == (int)PlantLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();

			/* Figure out total Adjustments */
			/* Create new Plant Payroll Lines for all adjustments */
			var adjustmentLines = _context.PlantAdjustmentLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(x => new { x.EmployeeId, x.IsOriginal }, (key, group) => new
				{
					key.EmployeeId,
					key.IsOriginal,
					Gross = group.Sum(s => s.TotalGross)
				})
				.ToList()
				.GroupBy(x => new { x.EmployeeId }, (key, group) => new PlantPayLine
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = weekendOfAdjustmentPaid,
					ShiftDate = weekendOfAdjustmentPaid,
					PayType = PayType.Adjustment,
					OtherGross = group.Sum(s => s.Gross * (s.IsOriginal ? -1 : 1)),
					TotalGross = group.Sum(s => s.Gross * (s.IsOriginal ? -1 : 1)),
					BatchId = batchId
				})
				.ToList();
			_context.AddRange(adjustmentLines);
			_context.SaveChanges();
		}


		private void PerformRanchCalculations(Batch batch)
		{
			var company = Company.Ranches;

			/* Download Quick Base data */
			SetBatchStatus(batch.Id, BatchProcessingStatus.Downloading);
			CopyRanchDataFromQuickBase(batch);

			/* Crew Boss Calculations */
			_logger.Log(LogLevel.Information, "Calculating crew boss pay for batch {batchId}", batch.Id);
			SetBatchStatus(batch.Id, BatchProcessingStatus.CrewBossCalculations);
			var crewBossPayLines = _crewBossPayService.CalculateCrewBossPay(batch.Id);

			// Add crew boss pay lines to database.
			_context.AddRange(crewBossPayLines);
			_context.SaveChanges();

			/* PSL Requires hours and gross for regular, piece, and CB pay types; also requires hours on Sick Leave pay types*/
			SetBatchStatus(batch.Id, BatchProcessingStatus.PaidSickLeaveCalculations);
			_logger.Log(LogLevel.Information, "Calculating PSL for batch {batchId}", batch.Id);

			// Perform PSL calculations
			DateTime? startDate = _context.RanchPayLines.Where(x => x.BatchId == batch.Id).OrderBy(s => s.ShiftDate).FirstOrDefault()?.ShiftDate;
			DateTime? endDate = _context.RanchPayLines.Where(x => x.BatchId == batch.Id).OrderByDescending(s => s.ShiftDate).FirstOrDefault()?.ShiftDate;

			if(startDate != null && endDate != null)
			{
				_paidSickLeaveService.UpdateTracking(batch.Id, company);
				_paidSickLeaveService.CalculateNinetyDay(batch.Id, company, startDate.Value, endDate.Value);

				// Update PSL usage
				_paidSickLeaveService.UpdateUsage(batch.Id, company);
			}
			

			// Set PSL/COVID19 rate and recalculate gross.
			var paidSickLeaves = _context.RanchPayLines
				.Where(x =>
					x.BatchId == batch.Id
					&& (
						x.PayType == PayType.SickLeave
						|| x.PayType == PayType.Covid19
						)
					)
				.ToList();
			paidSickLeaves.ForEach(x => x.HourlyRate = _paidSickLeaveService.GetNinetyDayRate(batch.Id, Company.Ranches, x.EmployeeId, x.ShiftDate));
			_grossFromHoursCalculator.CalculateGrossFromHours(paidSickLeaves);
			_totalGrossCalculator.CalculateTotalGross(paidSickLeaves);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
			SetBatchStatus(batch.Id, BatchProcessingStatus.AdditionalCalculations);
			_logger.Log(LogLevel.Information, "Calculating OT/DT/WOT/Seventh Day for batch {batchId}", batch.Id);
			// Daily summaries group all of the ranch pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of Crew and last of FiveEight sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummaries(batch.Id, company);

			// Calculate OT/DT/7th Day Hours
			// This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			// Create Weekly Summaries groups all of the daily summaries by Employee, Week End Date, and Minimum Wage and summarizes the
			// different types of hours for the week.  This information is used to figure out the effective hourly rate and create minimum
			// assurance lines.
			var weeklySummaries = _ranchWeeklySummaryCalculator.GetWeeklySummary(dailySummaries);

			// Minimum Make Up is made by comparing the effective rate against minimum wage.  If minimum wage is greater than the effective rate, the difference
			// should be used to create a minimum make up line and the higher of the two rates is used for OT, DT, etc.  When a week has multiple minimum wages, the
			// the effective rate calculated against the higher minimum wage should be used as the weekly effective rate.
			var minimumMakeUpRecords = _ranchMinimumMakeUpCalculator.GetMinimumMakeUps(weeklySummaries).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross,
				BatchId = batch.Id
			}).ToList();
			_totalGrossCalculator.CalculateTotalGross(minimumMakeUpRecords);
			BulkInsert(minimumMakeUpRecords);

			/* WOT Hours */
			var weeklyOt = _ranchWeeklyOverTimeHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			/* OT/DT Gross (Requires effective weekly rate) */
			var overTimeRecords = dailySummaries.Where(x => x.OverTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.OverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batch.Id
			}).ToList();
			overTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w => 
						w.WeekEndDate == x.WeekEndDate 
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			BulkInsert(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.DoubleTime,
				OtDtWotHours = x.DoubleTimeHours,
				BatchId = batch.Id
			}).ToList();
			doubleTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			BulkInsert(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				Crew = x.Crew,
				PayType = PayType.WeeklyOverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batch.Id
			}).ToList();
			weeklyOverTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			BulkInsert(weeklyOverTimeRecords);

			_logger.Log(LogLevel.Information, "Updating Reporting/Comp/NPT hourly rates for batch {batchId}", batch.Id);
			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			var reportingPayRecords = _context.RanchPayLines.Where(x => x.BatchId == batch.Id && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			reportingPayRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.RanchPayLines.Where(x => x.BatchId == batch.Id && (x.LaborCode == (int)RanchLaborCode.RecoveryTime || x.LaborCode == (int)RanchLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();


			/* Calculate Adjustments */
			SetBatchStatus(batch.Id, BatchProcessingStatus.Adjustments);
			CalculateRanchAdjustments(batch.Id, company);

			
			/* Update records to Quick Base */
			SetBatchStatus(batch.Id, BatchProcessingStatus.Uploading);
			_logger.Log(LogLevel.Information, "Updating Quick Base for batch {batchId}", batch.Id);

			// Purge records from earlier calculations of the same week ending date and layoff ID before
			// writing new calculations.
			var purgeResponse = _ranchPayrollOutRepo.Delete(batch.WeekEndDate, batch.LayoffId ?? 0);
			var adjPurgeResponse = _ranchPayrollAdjustmentOutRepo.Delete(batch.WeekEndDate, batch.LayoffId ?? 0);


			// Crew Boss Pay Records
			var toCrewBossPay = _context.CrewBossPayLines.Where(x => x.BatchId == batch.Id).ToList();
			if (batch.LayoffId != null) toCrewBossPay.ForEach(x => x.LayoffId = batch.LayoffId.Value);
			var cbReponse = _crewBossPayRepo.Save(toCrewBossPay);

			// Ranch Payroll Records
			// Crew boss lines need to have the hours worked included as they have been newly created.
			var cbLinesToRanchPayroll = _context.RanchPayLines
				.Where(x =>
					x.BatchId == batch.Id
					&& x.QuickBaseRecordId == 0
					&& (
						x.PayType == PayType.CBCommission
						|| x.PayType == PayType.CBDaily
						|| x.PayType == PayType.CBHourlyTrees
						|| x.PayType == PayType.CBHourlyVines
						|| x.PayType == PayType.CBPerWorker
						|| x.PayType == PayType.CBSouthDaily
						|| x.PayType == PayType.CBSouthHourly))
				.ToList();

			var toRanchPayroll = _context.RanchPayLines
				.Where(x =>
					x.BatchId == batch.Id
					&& x.PayType != PayType.CBCommission
					&& x.PayType != PayType.CBDaily
					&& x.PayType != PayType.CBHourlyTrees
					&& x.PayType != PayType.CBHourlyVines
					&& x.PayType != PayType.CBPerWorker
					&& x.PayType != PayType.CBSouthDaily
					&& x.PayType != PayType.CBSouthHourly).ToList();
			if (batch.LayoffId != null)
			{
				cbLinesToRanchPayroll.ForEach(x => x.LayoffId = batch.LayoffId.Value);
				toRanchPayroll.ForEach(x => x.LayoffId = batch.LayoffId.Value);
			}
			// Write to both Ranch Payroll and Ranch Payroll Out until output tables are 
			// fully adopted.
			var cbrpResponse = _ranchPayrollRepo.SaveWithHoursWorked(cbLinesToRanchPayroll);
			var rpResponse = _ranchPayrollRepo.Save(toRanchPayroll);
			var cbrpOutResponse = _ranchPayrollOutRepo.Save(cbLinesToRanchPayroll);
			var rpOutResponse = _ranchPayrollOutRepo.Save(toRanchPayroll);

			// Ranch Adjustment Records
			var toRanchAdjustments = _context.RanchAdjustmentLines.Where(x => x.BatchId == batch.Id).ToList();
			if (batch.LayoffId != null) toRanchAdjustments.ForEach(x => x.LayoffId = batch.LayoffId.Value);
			// Write to both Ranch Payroll Adjustment and Ranch Payroll Adjustment Out until output
			// tables are fully adopted.
			var rpaResponse = _ranchPayrollAdjustmentRepo.Save(toRanchAdjustments);
			var rpaOutResponse = _ranchPayrollAdjustmentOutRepo.Save(toRanchAdjustments);

			// PSL Updates
			var pslResponse = _pslTrackingDailyRepo.Save(_context.PaidSickLeaves.Where(x =>
				x.BatchId == batch.Id
				&& x.ShiftDate >= batch.WeekEndDate.AddDays(-6)
				&& x.ShiftDate <= batch.WeekEndDate).ToList());

		}

		private void CopyRanchDataFromQuickBase(Batch batch)
		{
			// Crew Boss Pay
			_logger.Log(LogLevel.Information, "Downloading crew boss payroll data from Quick Base for {batchId}", batch.Id);
			var crewBossPayLines = _crewBossPayRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			crewBossPayLines.ForEach(x => {
				x.BatchId = batch.Id;
				x.HoursWorked = _roundingService.Round(x.HoursWorked, 2);
			});

			// Ranch Payroll
			_logger.Log(LogLevel.Information, "Downloading ranch payroll data from Quick Base for {batchId}", batch.Id);
			var ranchPayLines = _ranchPayrollRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			ranchPayLines = ranchPayLines.Where(x => x.PayType != PayType.SpecialAdjustment || x.SpecialAdjustmentApproved).ToList();
			ranchPayLines.ForEach(x => { 
				x.BatchId = batch.Id;
				x.HoursWorked = _roundingService.Round(x.HoursWorked, 2);
			});

			// Ranch Adjustment
			_logger.Log(LogLevel.Information, "Downloading ranch adjustment data from Quick Base for {batchId}", batch.Id);
			var ranchAdjustmentLines = _ranchPayrollAdjustmentRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			ranchAdjustmentLines.ForEach(x => x.BatchId = batch.Id);

			// Ranch PSL Tracking
			_logger.Log(LogLevel.Information, "Downloading ranch paid sick leave data from Quick Base for {batchId}", batch.Id);
			var paidSickLeaves = GetPaidSickLeaveTracking(batch.WeekEndDate, batch.Company);
			paidSickLeaves.ForEach(x => x.BatchId = batch.Id);

			/* Gross Calculations */
			_logger.Log(LogLevel.Information, "Calculating initial gross for batch {batchId}", batch.Id);
			SetBatchStatus(batch.Id, BatchProcessingStatus.GrossCalculations);

			// Hourly
			_grossFromHoursCalculator.CalculateGrossFromHours(ranchPayLines.Where(x => 
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.HourlyPlusPieces
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.Bereavement))
				.ToList());

			// Pieces
			_grossFromPiecesCalculator.CalculateGrossFromPieces(ranchPayLines.Where(x => 
				(
					x.PayType == PayType.Pieces
					|| x.PayType == PayType.HourlyPlusPieces))
				.ToList());
			
			// Total
			_totalGrossCalculator.CalculateTotalGross(ranchPayLines.Where(x => x.BatchId == batch.Id).ToList());

			_logger.Log(LogLevel.Information, "Starting save of ranch data from Quick Base for {batchId}", batch.Id);
			BulkInsert(crewBossPayLines);
			BulkInsert(ranchPayLines);
			BulkInsert(ranchAdjustmentLines);
			BulkInsert(paidSickLeaves);
			
			_logger.Log(LogLevel.Information, "Completed saving ranch payroll data from Quick Base for {batchId}", batch.Id);
		}

		private void CalculateRanchAdjustments(int batchId, string company)
		{
			_logger.Log(LogLevel.Information, "Calculating ranch adjustments for {batchId}", batchId);
			DateTime weekendOfAdjustmentPaid = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId).OrderByDescending(x => x.WeekEndOfAdjustmentPaid).FirstOrDefault()?.WeekEndOfAdjustmentPaid ?? new DateTime(2000, 1, 1);

			/* Gross Calculations */
			// Hourly
			// Include sick leave to be calculated as we expect to be provided an "Old Hourly Rate" instead of figuring
			// out the rate ourselves.
			var hourlyLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId &&
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.HourlyPlusPieces
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.SickLeave
					|| x.PayType == PayType.Bereavement))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.RanchAdjustmentLines.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId &&
				(
					x.PayType == PayType.Pieces
					|| x.PayType == PayType.HourlyPlusPieces))
				.ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.RanchAdjustmentLines.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.RanchAdjustmentLines.UpdateRange(payLines);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
			// Daily summaries group all of the ranch pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of Crew and last of FiveEight sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummariesFromAdjustments(batchId, company);

			// Calculate OT/DT/7th Day Hours
			// This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
			_ranchDailyOTDTHoursCalculator.SetDailyOTDTHours(dailySummaries);

			// Create Weekly Summaries groups all of the daily summaries by Employee, Week End Date, and Minimum Wage and summarizes the
			// different types of hours for the week.  This information is used to figure out the effective hourly rate and create minimum
			// assurance lines.
			var weeklySummaries = _ranchWeeklySummaryCalculator.GetWeeklySummary(dailySummaries);

			// Minimum Make Up is made by comparing the effective rate against minimum wage.  If minimum wage is greater than the effective rate, the difference
			// should be used to create a minimum make up line and the higher of the two rates is used for OT, DT, etc.  When a week has multiple minimum wages, the
			// the effective rate calculated against the higher minimum wage should be used as the weekly effective rate.
			var minimumMakeUpRecords = _ranchMinimumMakeUpCalculator.GetMinimumMakeUps(weeklySummaries).Select(x => new RanchAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			_totalGrossCalculator.CalculateTotalGross(minimumMakeUpRecords);
			_context.RanchAdjustmentLines.AddRange(minimumMakeUpRecords);

			/* WOT Hours */
			var weeklyOt = _ranchWeeklyOverTimeHoursCalculator.GetWeeklyOTHours(weeklySummaries);

			/* OT/DT Gross (Requires effective weekly rate) */
			var overTimeRecords = dailySummaries.Where(x => x.OverTimeHours > 0).Select(x => new RanchAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.OverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			overTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			_context.RanchAdjustmentLines.AddRange(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new RanchAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.DoubleTime,
				OtDtWotHours = x.DoubleTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			doubleTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			_context.RanchAdjustmentLines.AddRange(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new RanchAdjustmentLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				Crew = x.Crew,
				PayType = PayType.WeeklyOverTime,
				OtDtWotHours = x.OverTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			weeklyOverTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.OtDtWotRate = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.OtDtWotRate * x.OtDtWotHours, 2);
				x.HourlyRate = x.OtDtWotRate;
			});
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			_context.RanchAdjustmentLines.AddRange(weeklyOverTimeRecords);


			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			var reportingPayRecords = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			reportingPayRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && (x.LaborCode == (int)RanchLaborCode.RecoveryTime || x.LaborCode == (int)RanchLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries
					.Where(w =>
						w.WeekEndDate == x.WeekEndDate
						&& w.EmployeeId == x.EmployeeId)
					.OrderByDescending(o => o.MinimumWage)
					.FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.HourlyRate = x.HourlyRateOverride;
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();


			/* Figure out total Adjustments */
			/* Create new Ranch Payroll Lines for all adjustments */
			var adjustmentLines = _context.RanchAdjustmentLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(x => new { x.EmployeeId, x.IsOriginal }, (key, group) => new
				{
					key.EmployeeId,
					key.IsOriginal,
					Gross = group.Sum(s => s.TotalGross)
				})
				.ToList()
				.GroupBy(x => new { x.EmployeeId }, (key, group) => new RanchPayLine
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = weekendOfAdjustmentPaid,
					ShiftDate = weekendOfAdjustmentPaid,
					PayType = PayType.Adjustment,
					OtherGross = group.Sum(s => s.Gross * (s.IsOriginal ? -1 : 1)),
					TotalGross = group.Sum(s => s.Gross * (s.IsOriginal ? -1 : 1)),
					BatchId = batchId
				})
				.ToList();
			_context.RanchPayLines.AddRange(adjustmentLines);
			_context.SaveChanges();
		}

		private void SetBatchStatus(int id, BatchProcessingStatus status)
		{
			var batch = _context.Batches.Find(id);
			if(batch != null)
			{
				if(status == BatchProcessingStatus.Failed)
				{
					batch.StatusMessage = $"Batch ID #{batch.Id} failed while at step '{batch.ProcessingStatus}'.";
					batch.IsComplete = true;
					batch.EndDate = DateTime.UtcNow;
				}
				else if(status == BatchProcessingStatus.Success)
				{
					batch.IsComplete = true;
					batch.EndDate = DateTime.UtcNow;
				}
				else if(status == BatchProcessingStatus.Starting)
				{
					batch.StartDate = DateTime.UtcNow;
				}
				
				batch.ProcessingStatus = status;
				_context.SaveChanges();
			}
		}

		private void BulkInsert(List<PaidSickLeave> paidSickLeaves)
		{
			var tableName = "PaidSickLeaves";
			var table = new DataTable(tableName);
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.Id), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.DateCreated), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.DateModified), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.IsDeleted), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.BatchId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.EmployeeId), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.ShiftDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.Company), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.Hours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.Gross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.NinetyDayHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.NinetyDayGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PaidSickLeave.HoursUsed), typeof(decimal)));

			var utcNow = DateTime.UtcNow;
			foreach (var psl in paidSickLeaves)
			{
				var row = table.NewRow();
				row[nameof(PaidSickLeave.Id)] = 0;
				row[nameof(PaidSickLeave.DateCreated)] = utcNow;
				row[nameof(PaidSickLeave.DateModified)] = utcNow;
				row[nameof(PaidSickLeave.IsDeleted)] = psl.IsDeleted;
				row[nameof(PaidSickLeave.BatchId)] = psl.BatchId;
				row[nameof(PaidSickLeave.EmployeeId)] = psl.EmployeeId;
				row[nameof(PaidSickLeave.ShiftDate)] = psl.ShiftDate;
				row[nameof(PaidSickLeave.Company)] = psl.Company;
				row[nameof(PaidSickLeave.Hours)] = psl.Hours;
				row[nameof(PaidSickLeave.Gross)] = psl.Gross;
				row[nameof(PaidSickLeave.NinetyDayHours)] = psl.NinetyDayHours;
				row[nameof(PaidSickLeave.NinetyDayGross)] = psl.NinetyDayGross;
				row[nameof(PaidSickLeave.HoursUsed)] = psl.HoursUsed;

				table.Rows.Add(row);
			}

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			using var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction();
			try
			{
				using var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
				sqlBulkCopy.BulkCopyTimeout = 0;
				sqlBulkCopy.BatchSize = 10000;
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.WriteToServer(table);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			finally
			{
				transaction.Dispose();
				connection.Close();
			}
		}

		private void BulkInsert(List<CrewBossPayLine> payLines)
		{
			var tableName = "CrewBossPayLines";
			var table = new DataTable(tableName);
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.Id), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.DateCreated), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.DateModified), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.IsDeleted), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.BatchId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.LayoffId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.QuickBaseRecordId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.WeekEndDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.ShiftDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.Crew), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.EmployeeId), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.PayMethod), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.WorkerCount), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.HoursWorked), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.HourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.Gross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(CrewBossPayLine.FiveEight), typeof(bool)));

			var utcNow = DateTime.UtcNow;
			foreach (var payLine in payLines)
			{
				var row = table.NewRow();
				row[nameof(CrewBossPayLine.Id)] = 0;
				row[nameof(CrewBossPayLine.DateCreated)] = utcNow;
				row[nameof(CrewBossPayLine.DateModified)] = utcNow;
				row[nameof(CrewBossPayLine.IsDeleted)] = payLine.IsDeleted;
				row[nameof(CrewBossPayLine.BatchId)] = payLine.BatchId;
				row[nameof(CrewBossPayLine.LayoffId)] = payLine.LayoffId;
				row[nameof(CrewBossPayLine.QuickBaseRecordId)] = payLine.QuickBaseRecordId;
				row[nameof(CrewBossPayLine.WeekEndDate)] = payLine.WeekEndDate;
				row[nameof(CrewBossPayLine.ShiftDate)] = payLine.ShiftDate;
				row[nameof(CrewBossPayLine.Crew)] = payLine.Crew;
				row[nameof(CrewBossPayLine.EmployeeId)] = payLine.EmployeeId;
				row[nameof(CrewBossPayLine.PayMethod)] = payLine.PayMethod;
				row[nameof(CrewBossPayLine.WorkerCount)] = payLine.WorkerCount;
				row[nameof(CrewBossPayLine.HoursWorked)] = payLine.HoursWorked;
				row[nameof(CrewBossPayLine.HourlyRate)] = payLine.HourlyRate;
				row[nameof(CrewBossPayLine.Gross)] = payLine.Gross;
				row[nameof(CrewBossPayLine.FiveEight)] = payLine.FiveEight;

				table.Rows.Add(row);
			}

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			using var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction();
			try
			{
				using var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
				sqlBulkCopy.BulkCopyTimeout = 0;
				sqlBulkCopy.BatchSize = 10000;
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.WriteToServer(table);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			finally
			{
				transaction.Dispose();
				connection.Close();
			}
		}

		private void BulkInsert(List<RanchPayLine> payLines)
		{
			var tableName = "RanchPayLines";
			var table = new DataTable(tableName);
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.Id), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.DateCreated), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.DateModified), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.IsDeleted), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.BatchId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.LayoffId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.QuickBaseRecordId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.WeekEndDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.ShiftDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.Crew), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.EmployeeId), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.LaborCode), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.BlockId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.HoursWorked), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.PayType), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.Pieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.PieceRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.HourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.OtDtWotRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.OtDtWotHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.GrossFromHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.GrossFromPieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.OtherGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.AlternativeWorkWeek), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.FiveEight), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.EmployeeHourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.HourlyRateOverride), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.TotalGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.LastCrew), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.EndTime), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(RanchPayLine.StartTime), typeof(string)));

			var utcNow = DateTime.UtcNow;
			foreach (var payLine in payLines)
			{
				var row = table.NewRow();
				row[nameof(RanchPayLine.Id)] = 0;
				row[nameof(RanchPayLine.DateCreated)] = utcNow;
				row[nameof(RanchPayLine.DateModified)] = utcNow;
				row[nameof(RanchPayLine.IsDeleted)] = payLine.IsDeleted;
				row[nameof(RanchPayLine.BatchId)] = payLine.BatchId;
				row[nameof(RanchPayLine.LayoffId)] = payLine.LayoffId;
				row[nameof(RanchPayLine.QuickBaseRecordId)] = payLine.QuickBaseRecordId;
				row[nameof(RanchPayLine.WeekEndDate)] = payLine.WeekEndDate;
				row[nameof(RanchPayLine.ShiftDate)] = payLine.ShiftDate;
				row[nameof(RanchPayLine.Crew)] = payLine.Crew;
				row[nameof(RanchPayLine.EmployeeId)] = payLine.EmployeeId;
				row[nameof(RanchPayLine.LaborCode)] = payLine.LaborCode;
				row[nameof(RanchPayLine.BlockId)] = payLine.BlockId;
				row[nameof(RanchPayLine.HoursWorked)] = payLine.HoursWorked;
				row[nameof(RanchPayLine.PayType)] = payLine.PayType;
				row[nameof(RanchPayLine.Pieces)] = payLine.Pieces;
				row[nameof(RanchPayLine.PieceRate)] = payLine.PieceRate;
				row[nameof(RanchPayLine.HourlyRate)] = payLine.HourlyRate;
				row[nameof(RanchPayLine.OtDtWotRate)] = payLine.OtDtWotRate;
				row[nameof(RanchPayLine.OtDtWotHours)] = payLine.OtDtWotHours;
				row[nameof(RanchPayLine.GrossFromHours)] = payLine.GrossFromHours;
				row[nameof(RanchPayLine.GrossFromPieces)] = payLine.GrossFromPieces;
				row[nameof(RanchPayLine.OtherGross)] = payLine.OtherGross;
				row[nameof(RanchPayLine.AlternativeWorkWeek)] = payLine.AlternativeWorkWeek;
				row[nameof(RanchPayLine.FiveEight)] = payLine.FiveEight;
				row[nameof(RanchPayLine.EmployeeHourlyRate)] = payLine.EmployeeHourlyRate;
				row[nameof(RanchPayLine.HourlyRateOverride)] = payLine.HourlyRateOverride;
				row[nameof(RanchPayLine.TotalGross)] = payLine.TotalGross;
				row[nameof(RanchPayLine.LastCrew)] = payLine.LastCrew;
				row[nameof(RanchPayLine.EndTime)] = payLine.EndTime;
				row[nameof(RanchPayLine.StartTime)] = payLine.StartTime;
				table.Rows.Add(row);
			}

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			using var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction();
			try
			{
				using var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
				sqlBulkCopy.BulkCopyTimeout = 0;
				sqlBulkCopy.BatchSize = 10000;
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.WriteToServer(table);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			finally
			{
				transaction.Dispose();
				connection.Close();
			}
		}

		private void BulkInsert(List<RanchAdjustmentLine> adjustmentLines)
		{
			var tableName = "RanchAdjustmentLines";
			var table = new DataTable(tableName);
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.Id), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.DateCreated), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.DateModified), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.IsDeleted), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.BatchId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.LayoffId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.QuickBaseRecordId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.WeekEndDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.ShiftDate), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.Crew), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.EmployeeId), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.LaborCode), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.BlockId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.HoursWorked), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.PayType), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.Pieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.PieceRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.HourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.OtDtWotRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.OtDtWotHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.GrossFromHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.GrossFromPieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.OtherGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.TotalGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.AlternativeWorkWeek), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.FiveEight), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.HourlyRateOverride), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.EmployeeHourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.WeekEndOfAdjustmentPaid), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.IsOriginal), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.OldHourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.UseOldHourlyRate), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.StartTime), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(RanchAdjustmentLine.EndTime), typeof(string)));

			var utcNow = DateTime.UtcNow;
			foreach (var adjustmentLine in adjustmentLines)
			{
				var row = table.NewRow();
				row[nameof(RanchAdjustmentLine.Id)] = 0;
				row[nameof(RanchAdjustmentLine.DateCreated)] = utcNow;
				row[nameof(RanchAdjustmentLine.DateModified)] = utcNow;
				row[nameof(RanchAdjustmentLine.IsDeleted)] = adjustmentLine.IsDeleted;
				row[nameof(RanchAdjustmentLine.BatchId)] = adjustmentLine.BatchId;
				row[nameof(RanchAdjustmentLine.LayoffId)] = adjustmentLine.LayoffId;
				row[nameof(RanchAdjustmentLine.QuickBaseRecordId)] = adjustmentLine.QuickBaseRecordId;
				row[nameof(RanchAdjustmentLine.WeekEndDate)] = adjustmentLine.WeekEndDate;
				row[nameof(RanchAdjustmentLine.ShiftDate)] = adjustmentLine.ShiftDate;
				row[nameof(RanchAdjustmentLine.Crew)] = adjustmentLine.Crew;
				row[nameof(RanchAdjustmentLine.EmployeeId)] = adjustmentLine.EmployeeId;
				row[nameof(RanchAdjustmentLine.LaborCode)] = adjustmentLine.LaborCode;
				row[nameof(RanchAdjustmentLine.BlockId)] = adjustmentLine.BlockId;
				row[nameof(RanchAdjustmentLine.HoursWorked)] = adjustmentLine.HoursWorked;
				row[nameof(RanchAdjustmentLine.PayType)] = adjustmentLine.PayType;
				row[nameof(RanchAdjustmentLine.Pieces)] = adjustmentLine.Pieces;
				row[nameof(RanchAdjustmentLine.PieceRate)] = adjustmentLine.PieceRate;
				row[nameof(RanchAdjustmentLine.HourlyRate)] = adjustmentLine.HourlyRate;
				row[nameof(RanchAdjustmentLine.OtDtWotRate)] = adjustmentLine.OtDtWotRate;
				row[nameof(RanchAdjustmentLine.OtDtWotHours)] = adjustmentLine.OtDtWotHours;
				row[nameof(RanchAdjustmentLine.GrossFromHours)] = adjustmentLine.GrossFromHours;
				row[nameof(RanchAdjustmentLine.GrossFromPieces)] = adjustmentLine.GrossFromPieces;
				row[nameof(RanchAdjustmentLine.OtherGross)] = adjustmentLine.OtherGross;
				row[nameof(RanchAdjustmentLine.TotalGross)] = adjustmentLine.TotalGross;
				row[nameof(RanchAdjustmentLine.AlternativeWorkWeek)] = adjustmentLine.AlternativeWorkWeek;
				row[nameof(RanchAdjustmentLine.FiveEight)] = adjustmentLine.FiveEight;
				row[nameof(RanchAdjustmentLine.HourlyRateOverride)] = adjustmentLine.HourlyRateOverride;
				row[nameof(RanchAdjustmentLine.EmployeeHourlyRate)] = adjustmentLine.EmployeeHourlyRate;
				row[nameof(RanchAdjustmentLine.WeekEndOfAdjustmentPaid)] = adjustmentLine.WeekEndOfAdjustmentPaid;
				row[nameof(RanchAdjustmentLine.IsOriginal)] = adjustmentLine.IsOriginal;
				row[nameof(RanchAdjustmentLine.OldHourlyRate)] = adjustmentLine.OldHourlyRate;
				row[nameof(RanchAdjustmentLine.UseOldHourlyRate)] = adjustmentLine.UseOldHourlyRate;
				row[nameof(RanchAdjustmentLine.StartTime)] = adjustmentLine.StartTime;
				row[nameof(RanchAdjustmentLine.EndTime)] = adjustmentLine.EndTime;
				table.Rows.Add(row);
			}

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			using var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction();
			try
			{
				using var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
				sqlBulkCopy.BulkCopyTimeout = 0;
				sqlBulkCopy.BatchSize = 10000;
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.WriteToServer(table);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			finally
			{
				transaction.Dispose();
				connection.Close();
			}
		}

		private void BulkInsert(List<PlantPayLine> payLines)
		{
			var tableName = "PlantPayLines";
			var table = new DataTable(tableName);
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.Id), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.DateCreated), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.DateModified), typeof(System.DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.IsDeleted), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.BatchId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.LayoffId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.QuickBaseRecordId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.WeekEndDate), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.ShiftDate), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.Plant), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.EmployeeId), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.LaborCode), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.HoursWorked), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.PayType), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.Pieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.HourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.OtDtWotRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.OtDtWotHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.GrossFromHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.GrossFromPieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.OtherGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.TotalGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.AlternativeWorkWeek), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.HourlyRateOverride), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.EmployeeHourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.GrossFromIncentive), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.HasNonPrimaViolation), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.IncreasedRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.IsH2A), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.IsIncentiveDisqualified), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.NonPrimaRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.PrimaRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.UseIncreasedRate), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.NonDiscretionaryBonusRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.UseCrewLaborRateForMinimumAssurance), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.BoxStyle), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.BoxStyleDescription), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.EndTime), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.H2AHoursOffered), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantPayLine.StartTime), typeof(DateTime)));

			var utcNow = DateTime.UtcNow;
			foreach (var payLine in payLines)
			{
				var row = table.NewRow();
				row[nameof(PlantPayLine.Id)] = 0;
				row[nameof(PlantPayLine.DateCreated)] = utcNow;
				row[nameof(PlantPayLine.DateModified)] = utcNow;
				row[nameof(PlantPayLine.IsDeleted)] = payLine.IsDeleted;
				row[nameof(PlantPayLine.BatchId)] = payLine.BatchId;
				row[nameof(PlantPayLine.LayoffId)] = payLine.LayoffId;
				row[nameof(PlantPayLine.QuickBaseRecordId)] = payLine.QuickBaseRecordId;
				row[nameof(PlantPayLine.WeekEndDate)] = payLine.WeekEndDate;
				row[nameof(PlantPayLine.ShiftDate)] = payLine.ShiftDate;
				row[nameof(PlantPayLine.Plant)] = payLine.Plant;
				row[nameof(PlantPayLine.EmployeeId)] = payLine.EmployeeId;
				row[nameof(PlantPayLine.LaborCode)] = payLine.LaborCode;
				row[nameof(PlantPayLine.HoursWorked)] = payLine.HoursWorked;
				row[nameof(PlantPayLine.PayType)] = payLine.PayType;
				row[nameof(PlantPayLine.Pieces)] = payLine.Pieces;
				row[nameof(PlantPayLine.HourlyRate)] = payLine.HourlyRate;
				row[nameof(PlantPayLine.OtDtWotRate)] = payLine.OtDtWotRate;
				row[nameof(PlantPayLine.OtDtWotHours)] = payLine.OtDtWotHours;
				row[nameof(PlantPayLine.GrossFromHours)] = payLine.GrossFromHours;
				row[nameof(PlantPayLine.GrossFromPieces)] = payLine.GrossFromPieces;
				row[nameof(PlantPayLine.OtherGross)] = payLine.OtherGross;
				row[nameof(PlantPayLine.TotalGross)] = payLine.TotalGross;
				row[nameof(PlantPayLine.AlternativeWorkWeek)] = payLine.AlternativeWorkWeek;
				row[nameof(PlantPayLine.HourlyRateOverride)] = payLine.HourlyRateOverride;
				row[nameof(PlantPayLine.EmployeeHourlyRate)] = payLine.EmployeeHourlyRate;
				row[nameof(PlantPayLine.GrossFromIncentive)] = payLine.GrossFromIncentive;
				row[nameof(PlantPayLine.HasNonPrimaViolation)] = payLine.HasNonPrimaViolation;
				row[nameof(PlantPayLine.IncreasedRate)] = payLine.IncreasedRate;
				row[nameof(PlantPayLine.IsH2A)] = payLine.IsH2A;
				row[nameof(PlantPayLine.IsIncentiveDisqualified)] = payLine.IsIncentiveDisqualified;
				row[nameof(PlantPayLine.NonPrimaRate)] = payLine.NonPrimaRate;
				row[nameof(PlantPayLine.PrimaRate)] = payLine.PrimaRate;
				row[nameof(PlantPayLine.UseIncreasedRate)] = payLine.UseIncreasedRate;
				row[nameof(PlantPayLine.NonDiscretionaryBonusRate)] = payLine.NonDiscretionaryBonusRate;
				row[nameof(PlantPayLine.UseCrewLaborRateForMinimumAssurance)] = payLine.UseCrewLaborRateForMinimumAssurance;
				row[nameof(PlantPayLine.BoxStyle)] = payLine.BoxStyle;
				row[nameof(PlantPayLine.BoxStyleDescription)] = payLine.BoxStyleDescription;
				row[nameof(PlantPayLine.EndTime)] = payLine.EndTime ?? (object)DBNull.Value;
				row[nameof(PlantPayLine.H2AHoursOffered)] = payLine.H2AHoursOffered;
				row[nameof(PlantPayLine.StartTime)] = payLine.StartTime ?? (object)DBNull.Value;
				table.Rows.Add(row);
			}

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			using var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction();
			try
			{
				using var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
				sqlBulkCopy.BulkCopyTimeout = 0;
				sqlBulkCopy.BatchSize = 10000;
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.WriteToServer(table);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			finally
			{
				transaction.Dispose();
				connection.Close();
			}
		}

		private void BulkInsert(List<PlantAdjustmentLine> adjustmentLines)
		{
			var tableName = "PlantAdjustmentLines";
			var table = new DataTable(tableName);
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.Id), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.DateCreated), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.DateModified), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.IsDeleted), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.BatchId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.LayoffId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.QuickBaseRecordId), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.WeekEndDate), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.ShiftDate), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.Plant), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.EmployeeId), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.LaborCode), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.HoursWorked), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.PayType), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.Pieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.PieceRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.HourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.OtDtWotRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.OtDtWotHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.GrossFromHours), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.GrossFromPieces), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.OtherGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.GrossFromIncentive), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.TotalGross), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.AlternativeWorkWeek), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.HourlyRateOverride), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.EmployeeHourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.IsH2A), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.WeekEndOfAdjustmentPaid), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.IsOriginal), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.OldHourlyRate), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.UseOldHourlyRate), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.UseCrewLaborRateForMinimumAssurance), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.BoxStyle), typeof(int)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.BoxStyleDescription), typeof(string)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.EndTime), typeof(DateTime)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.H2AHoursOffered), typeof(decimal)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.IsIncentiveDisqualified), typeof(bool)));
			table.Columns.Add(new DataColumn(nameof(PlantAdjustmentLine.StartTime), typeof(DateTime)));

			var utcNow = DateTime.UtcNow;
			foreach (var adjustmentLine in adjustmentLines)
			{
				var row = table.NewRow();
				row[nameof(PlantAdjustmentLine.Id)] = 0;
				row[nameof(PlantAdjustmentLine.DateCreated)] = utcNow;
				row[nameof(PlantAdjustmentLine.DateModified)] = utcNow;
				row[nameof(PlantAdjustmentLine.IsDeleted)] = adjustmentLine.IsDeleted;
				row[nameof(PlantAdjustmentLine.BatchId)] = adjustmentLine.BatchId;
				row[nameof(PlantAdjustmentLine.LayoffId)] = adjustmentLine.LayoffId;
				row[nameof(PlantAdjustmentLine.QuickBaseRecordId)] = adjustmentLine.QuickBaseRecordId;
				row[nameof(PlantAdjustmentLine.WeekEndDate)] = adjustmentLine.WeekEndDate;
				row[nameof(PlantAdjustmentLine.ShiftDate)] = adjustmentLine.ShiftDate;
				row[nameof(PlantAdjustmentLine.Plant)] = adjustmentLine.Plant;
				row[nameof(PlantAdjustmentLine.EmployeeId)] = adjustmentLine.EmployeeId;
				row[nameof(PlantAdjustmentLine.LaborCode)] = adjustmentLine.LaborCode;
				row[nameof(PlantAdjustmentLine.HoursWorked)] = adjustmentLine.HoursWorked;
				row[nameof(PlantAdjustmentLine.PayType)] = adjustmentLine.PayType;
				row[nameof(PlantAdjustmentLine.Pieces)] = adjustmentLine.Pieces;
				row[nameof(PlantAdjustmentLine.PieceRate)] = adjustmentLine.PieceRate;
				row[nameof(PlantAdjustmentLine.HourlyRate)] = adjustmentLine.HourlyRate;
				row[nameof(PlantAdjustmentLine.OtDtWotRate)] = adjustmentLine.OtDtWotRate;
				row[nameof(PlantAdjustmentLine.OtDtWotHours)] = adjustmentLine.OtDtWotHours;
				row[nameof(PlantAdjustmentLine.GrossFromHours)] = adjustmentLine.GrossFromHours;
				row[nameof(PlantAdjustmentLine.GrossFromPieces)] = adjustmentLine.GrossFromPieces;
				row[nameof(PlantAdjustmentLine.OtherGross)] = adjustmentLine.OtherGross;
				row[nameof(PlantAdjustmentLine.GrossFromIncentive)] = adjustmentLine.GrossFromIncentive;
				row[nameof(PlantAdjustmentLine.TotalGross)] = adjustmentLine.TotalGross;
				row[nameof(PlantAdjustmentLine.AlternativeWorkWeek)] = adjustmentLine.AlternativeWorkWeek;
				row[nameof(PlantAdjustmentLine.HourlyRateOverride)] = adjustmentLine.HourlyRateOverride;
				row[nameof(PlantAdjustmentLine.EmployeeHourlyRate)] = adjustmentLine.EmployeeHourlyRate;
				row[nameof(PlantAdjustmentLine.IsH2A)] = adjustmentLine.IsH2A;
				row[nameof(PlantAdjustmentLine.WeekEndOfAdjustmentPaid)] = adjustmentLine.WeekEndOfAdjustmentPaid;
				row[nameof(PlantAdjustmentLine.IsOriginal)] = adjustmentLine.IsOriginal;
				row[nameof(PlantAdjustmentLine.OldHourlyRate)] = adjustmentLine.OldHourlyRate;
				row[nameof(PlantAdjustmentLine.UseOldHourlyRate)] = adjustmentLine.UseOldHourlyRate;
				row[nameof(PlantAdjustmentLine.UseCrewLaborRateForMinimumAssurance)] = adjustmentLine.UseCrewLaborRateForMinimumAssurance;
				row[nameof(PlantAdjustmentLine.BoxStyle)] = adjustmentLine.BoxStyle;
				row[nameof(PlantAdjustmentLine.BoxStyleDescription)] = adjustmentLine.BoxStyleDescription;
				row[nameof(PlantAdjustmentLine.EndTime)] = adjustmentLine.EndTime ?? (object)DBNull.Value;
				row[nameof(PlantAdjustmentLine.H2AHoursOffered)] = adjustmentLine.H2AHoursOffered;
				row[nameof(PlantAdjustmentLine.IsIncentiveDisqualified)] = adjustmentLine.IsIncentiveDisqualified;
				row[nameof(PlantAdjustmentLine.StartTime)] = adjustmentLine.StartTime ?? (object)DBNull.Value;
				table.Rows.Add(row);
			}

			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			using var connection = new SqlConnection(connectionString);
			connection.Open();
			var transaction = connection.BeginTransaction();
			try
			{
				using var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
				sqlBulkCopy.BulkCopyTimeout = 0;
				sqlBulkCopy.BatchSize = 10000;
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.WriteToServer(table);
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			finally
			{
				transaction.Dispose();
				connection.Close();
			}
		}
	}
}
