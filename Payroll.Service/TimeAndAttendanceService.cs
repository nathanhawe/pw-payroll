using Payroll.Data.QuickBase;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Manages the calculation workflow
	/// </summary>
	public class TimeAndAttendanceService : ITimeAndAttendanceService
	{
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
			IPlantSummaryService plantSummaryService)
		{
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

		}

		public void PerformCalculations(int batchId)
		{
			var batch = _context.Batches.Where(x => x.Id == batchId).FirstOrDefault();
			if (batch == null) throw new Exception($"The provided batch ID of '{batchId}' was not found in the database.");

			switch (batch.Company)
			{
				case Company.Plants: PerformPlantCalculations(batch); break;
				case Company.Ranches: PerformRanchCalculations(batch); break;
				default: throw new Exception($"Unknown company value '{batch.Company}'.");
			}
		}

		private void PerformPlantCalculations(Batch batch)
		{
			var company = Company.Plants;

			/* Download Quick Base data */
			CopyPlantDataFromQuickBase(batch);

			/* Gross Calculations */
			// Hourly
			var hourlyLines = _context.PlantPayLines.Where(x => x.BatchId == batch.Id &&
				(
					x.PayType == PayType.Regular
					//|| x.PayType == PayType.HourlyPlusPieces
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.Bereavement))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && x.PayType == PayType.Pieces).ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Incentives!!!
			var incentiveLines = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && (
				x.LaborCode == (int)PlantLaborCode.TallyTagWriter
				|| (x.PayType == PayType.Pieces && !x.IsIncentiveDisqualified))).ToList();
			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(incentiveLines);
			_context.UpdateRange(incentiveLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.PlantPayLines.Where(x => x.BatchId == batch.Id).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.UpdateRange(payLines);
			_context.SaveChanges();


			/* PSL Requires hours and gross for regular, piece; also requires hours on Sick Leave pay types*/
			// Perform PSL calculations
			var startDate = _context.PlantPayLines.Where(x => x.BatchId == batch.Id).OrderBy(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;
			var endDate = _context.PlantPayLines.Where(x => x.BatchId == batch.Id).OrderByDescending(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;

			_paidSickLeaveService.UpdateTracking(batch.Id, company);
			_paidSickLeaveService.CalculateNinetyDay(batch.Id, company, startDate, endDate);

			// Update PSL usage
			_paidSickLeaveService.UpdateUsage(batch.Id, company);

			// Set PSL rate and recalculate gross.
			var paidSickLeaves = _context.PlantPayLines
				.Where(x =>
					x.BatchId == batch.Id
					&& x.PayType == PayType.SickLeave)
				.ToList();
			paidSickLeaves.ForEach(x => x.HourlyRate = _paidSickLeaveService.GetNinetyDayRate(batch.Id, Company.Plants, x.EmployeeId, x.ShiftDate));
			_grossFromHoursCalculator.CalculateGrossFromHours(paidSickLeaves);
			_totalGrossCalculator.CalculateTotalGross(paidSickLeaves);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
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
			_context.AddRange(minimumMakeUpRecords);


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
				HoursWorked = x.OverTimeHours,
				BatchId = batch.Id
			}).ToList();
			overTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
			});
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			_context.AddRange(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new PlantPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Plant = x.Crew,
				PayType = PayType.DoubleTime,
				HoursWorked = x.DoubleTimeHours,
				BatchId = batch.Id
			}).ToList();
			doubleTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
			});
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			_context.AddRange(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new PlantPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				Plant = x.Crew,
				PayType = PayType.WeeklyOverTime,
				HoursWorked = x.OverTimeHours,
				BatchId = batch.Id
			}).ToList();
			weeklyOverTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
			});
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			_context.AddRange(weeklyOverTimeRecords);


			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			//var reportingPayRecords = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			var reportingPayRecords = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && x.PayType == PayType.ReportingPay).ToList();
			reportingPayRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.PlantPayLines.Where(x => x.BatchId == batch.Id && (x.LaborCode == (int)PlantLaborCode.RecoveryTime || x.LaborCode == (int)PlantLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();

			/* Calculate Adjustments */
			CalculatePlantAdjustments(batch.Id, company);

			/* Create Summaries */
			var summaries = _plantSummaryService.CreateSummariesForBatch(batch.Id);
			_context.AddRange(summaries);
			_context.SaveChanges();

			/* Update records to Quick Base */
			// Ranch Payroll Records
			// Ranch Adjustment Records
			// Ranch Summary Records
		}

		private void CopyPlantDataFromQuickBase(Batch batch)
		{
			// Plant Payroll
			var plantPayLines = _plantPayrollRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			plantPayLines = plantPayLines.Where(x => x.PayType != PayType.SpecialAdjustment || x.SpecialAdjustmentApproved).ToList();
			plantPayLines.ForEach(x => x.BatchId = batch.Id);

			// Plant Adjustment
			var plantAdjustmentLines = _plantPayrollAdjustmentRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			plantAdjustmentLines.ForEach(x => x.BatchId = batch.Id);

			// Plant PSL Tracking
			List<PaidSickLeave> paidSickLeaves = GetPaidSickLeaveTracking(batch.WeekEndDate, batch.Company);
			paidSickLeaves.ForEach(x => x.BatchId = batch.Id);

			
			_context.AddRange(plantPayLines);
			_context.AddRange(plantAdjustmentLines);
			_context.AddRange(paidSickLeaves);
			_context.SaveChanges();
		}

		private List<PaidSickLeave> GetPaidSickLeaveTracking(DateTime weekEndDate, string company)
		{
			return _pslTrackingDailyRepo.Get(weekEndDate.AddDays(-91), weekEndDate.AddDays(-1), company).ToList();
		}

		private void CalculatePlantAdjustments(int batchId, string company)
		{
			DateTime weekendOfAdjustmentPaid = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId).OrderByDescending(x => x.WeekEndOfAdjustmentPaid).FirstOrDefault()?.WeekEndOfAdjustmentPaid ?? new DateTime(2000, 1, 1);

			/* Gross Calculations */
			// Hourly
			// Include sick leave to be calculated as we expect to be provided an "Old Hourly Rate" instead of figuring
			// out the rate ourselves.
			var hourlyLines = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal &&
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.SickLeave
					|| x.PayType == PayType.Bereavement))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal && x.PayType == PayType.Pieces).ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// There is no incentive calculation in adjustments at this time!
			// The previously calculated gross from incentive is loaded into a data entry field and the value must be manually adjusted in the
			// "New" record.

			// Total
			var payLines = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal).ToList();
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
				HoursWorked = x.OverTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			overTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
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
				HoursWorked = x.DoubleTimeHours,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid,
				BatchId = batchId
			}).ToList();
			doubleTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
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
				HoursWorked = x.OverTimeHours,
				BatchId = batchId,
				WeekEndOfAdjustmentPaid = weekendOfAdjustmentPaid
			}).ToList();
			weeklyOverTimeRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
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
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.PlantAdjustmentLines.Where(x => x.BatchId == batchId && (x.LaborCode == (int)PlantLaborCode.RecoveryTime || x.LaborCode == (int)PlantLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();

			/* Figure out total Adjustments */
			/* Create new Plant Payroll Lines for all adjustments */
			var adjustmentLines = _context.PlantAdjustmentLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(x => new { x.EmployeeId, x.WeekEndDate, x.IsOriginal }, (key, group) => new
				{
					key.EmployeeId,
					key.WeekEndDate,
					key.IsOriginal,
					Gross = group.Sum(s => s.TotalGross)
				})
				.ToList()
				.GroupBy(x => new { x.EmployeeId, x.WeekEndDate }, (key, group) => new PlantPayLine
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					ShiftDate = key.WeekEndDate,
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
			CopyRanchDataFromQuickBase(batch);

			/* Crew Boss Calculations */
			var crewBossPayLines = _crewBossPayService.CalculateCrewBossPay(batch.Id);

			// Add crew boss pay lines to database.
			_context.AddRange(crewBossPayLines);
			_context.SaveChanges();


			/* Gross Calculations */
			// Hourly
			var hourlyLines = _context.RanchPayLines.Where(x => x.BatchId == batch.Id &&
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.HourlyPlusPieces
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday
					|| x.PayType == PayType.Bereavement))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.RanchPayLines.Where(x => x.BatchId == batch.Id &&
				(
					x.PayType == PayType.Pieces
					|| x.PayType == PayType.HourlyPlusPieces))
				.ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.RanchPayLines.Where(x => x.BatchId == batch.Id).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.UpdateRange(payLines);
			_context.SaveChanges();

			/* PSL Requires hours and gross for regular, piece, and CB pay types; also requires hours on Sick Leave pay types*/
			// Perform PSL calculations
			var startDate = _context.RanchPayLines.Where(x => x.BatchId == batch.Id).OrderBy(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;
			var endDate = _context.RanchPayLines.Where(x => x.BatchId == batch.Id).OrderByDescending(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;

			_paidSickLeaveService.UpdateTracking(batch.Id, company);
			_paidSickLeaveService.CalculateNinetyDay(batch.Id, company, startDate, endDate);

			// Update PSL usage
			_paidSickLeaveService.UpdateUsage(batch.Id, company);

			// Set PSL rate and recalculate gross.
			var paidSickLeaves = _context.RanchPayLines
				.Where(x =>
					x.BatchId == batch.Id
					&& x.PayType == PayType.SickLeave)
				.ToList();
			paidSickLeaves.ForEach(x => x.HourlyRate = _paidSickLeaveService.GetNinetyDayRate(batch.Id, Company.Ranches, x.EmployeeId, x.ShiftDate));
			_grossFromHoursCalculator.CalculateGrossFromHours(paidSickLeaves);
			_totalGrossCalculator.CalculateTotalGross(paidSickLeaves);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
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
			_context.AddRange(minimumMakeUpRecords);

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
				HoursWorked = x.OverTimeHours,
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
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
			});
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			_context.AddRange(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				Crew = x.Crew,
				PayType = PayType.DoubleTime,
				HoursWorked = x.DoubleTimeHours,
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
				x.HourlyRateOverride = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
			});
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			_context.AddRange(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				Crew = x.Crew,
				PayType = PayType.WeeklyOverTime,
				HoursWorked = x.OverTimeHours,
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
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
			});
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			_context.AddRange(weeklyOverTimeRecords);


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
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();


			/* Calculate Adjustments */
			CalculateRanchAdjustments(batch.Id, company);

			/* Create Summaries */
			var summaries = _ranchSummaryService.CreateSummariesForBatch(batch.Id);
			_context.AddRange(summaries);
			_context.SaveChanges();

			/* Update records to Quick Base */
			// Ranch Payroll Records
			// Ranch Adjustment Records
			// Ranch Summary Records
		}

		private void CopyRanchDataFromQuickBase(Batch batch)
		{
			// Crew Boss Pay
			var crewBossPayLines = _crewBossPayRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			crewBossPayLines.ForEach(x => x.BatchId = batch.Id);

			// Ranch Payroll
			var ranchPayLines = _ranchPayrollRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			ranchPayLines = ranchPayLines.Where(x => x.PayType != PayType.SpecialAdjustment || x.SpecialAdjustmentApproved).ToList();
			ranchPayLines.ForEach(x => x.BatchId = batch.Id);

			// Ranch Adjustment
			var ranchAdjustmentLines = _ranchPayrollAdjustmentRepo.Get(batch.WeekEndDate, batch.LayoffId ?? 0).ToList();
			ranchAdjustmentLines.ForEach(x => x.BatchId = batch.Id);

			// Ranch PSL Tracking
			var paidSickLeaves = GetPaidSickLeaveTracking(batch.WeekEndDate, batch.Company);
			paidSickLeaves.ForEach(x => x.BatchId = batch.Id);

			_context.AddRange(crewBossPayLines);
			_context.AddRange(ranchPayLines);
			_context.AddRange(ranchAdjustmentLines);
			_context.AddRange(paidSickLeaves);
			_context.SaveChanges();
		}

		private void CalculateRanchAdjustments(int batchId, string company)
		{
			DateTime weekendOfAdjustmentPaid = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId).OrderByDescending(x => x.WeekEndOfAdjustmentPaid).FirstOrDefault()?.WeekEndOfAdjustmentPaid ?? new DateTime(2000, 1, 1);

			/* Gross Calculations */
			// Hourly
			// Include sick leave to be calculated as we expect to be provided an "Old Hourly Rate" instead of figuring
			// out the rate ourselves.
			var hourlyLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal &&
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
			var pieceLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal &&
				(
					x.PayType == PayType.Pieces
					|| x.PayType == PayType.HourlyPlusPieces))
				.ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.RanchAdjustmentLines.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal).ToList();
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
				HoursWorked = x.OverTimeHours,
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
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
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
				HoursWorked = x.DoubleTimeHours,
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
				x.HourlyRateOverride = _roundingService.Round((weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
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
				HoursWorked = x.OverTimeHours,
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
				x.HourlyRateOverride = _roundingService.Round(.5M * (weeklySummary == null ? 0 : Math.Max(weeklySummary.EffectiveHourlyRate, weeklySummary.MinimumWage)), 2);
				x.OtherGross = _roundingService.Round(x.HourlyRateOverride * x.HoursWorked, 2);
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
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();


			/* Figure out total Adjustments */
			/* Create new Ranch Payroll Lines for all adjustments */
			var adjustmentLines = _context.RanchAdjustmentLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(x => new { x.EmployeeId, x.WeekEndDate, x.IsOriginal }, (key, group) => new
				{
					key.EmployeeId,
					key.WeekEndDate,
					key.IsOriginal,
					Gross = group.Sum(s => s.TotalGross)
				})
				.ToList()
				.GroupBy(x => new { x.EmployeeId, x.WeekEndDate }, (key, group) => new RanchPayLine
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					ShiftDate = key.WeekEndDate,
					PayType = PayType.Adjustment,
					OtherGross = group.Sum(s => s.Gross * (s.IsOriginal ? -1 : 1)),
					TotalGross = group.Sum(s => s.Gross * (s.IsOriginal ? -1 : 1)),
					BatchId = batchId
				})
				.ToList();
			_context.RanchPayLines.AddRange(adjustmentLines);
			_context.SaveChanges();
		}
	}
}
