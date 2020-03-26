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
	public class TimeAndAttendanceService
	{
		private Data.PayrollContext _context;

		private CrewBossPayService _crewBossPayService;
		private PaidSickLeaveService _paidSickLeaveService;
		private GrossFromHoursCalculator _grossFromHoursCalculator;
		private GrossFromPiecesCalculator _grossFromPiecesCalculator;
		private GrossFromIncentiveCalculator _grossFromIncentiveCalculator;
		private TotalGrossCalculator _totalGrossCalculator;
		private DailySummaryCalculator _dailySummaryCalculator;
		private RanchWeeklySummaryCalculator _ranchWeeklySummaryCalculator;
		private PlantWeeklySummaryCalculator _plantWeeklySummaryCalculator;
		private RanchDailyOTDTHoursCalculator _ranchDailyOverTimeHoursCalculator;
		private PlantDailyOTDTHoursCalculator _plantDailyOTDTHoursCalculator;
		private RanchWeeklyOTHoursCalculator _ranchWeeklyOverTimeHoursCalculator;
		private PlantWeeklyOTHoursCalculator _plantWeeklyOverTimeHoursCalculator;
		private RanchMinimumMakeUpCalculator _ranchMinimumMakeUpCalculator;
		private PlantMinimumMakeUpCalculator _plantMinimumMakeUpCalculator;
		private RanchSummaryService _ranchSummaryService;
		private PlantSummaryService _plantSummaryService;
		private IRoundingService _roundingService;

		public TimeAndAttendanceService(CrewBossPayService crewBossPayService, PaidSickLeaveService paidSickLeaveService)
		{
			_crewBossPayService = crewBossPayService ?? throw new ArgumentNullException(nameof(crewBossPayService));
			_paidSickLeaveService = paidSickLeaveService ?? throw new ArgumentNullException(nameof(paidSickLeaveService));

		}
		public void PerformCalculations(int batchId, string company)
		{
			switch (company)
			{
				case Company.Plants: PerformPlantCalculations(batchId); break;
				case Company.Ranches: PerformRanchCalculations(batchId); break;
				default: throw new Exception($"Unknown company value '{company}'.");
			}
		}

		public void PerformPlantCalculations(int batchId)
		{
			var company = Company.Plants;

			/* Download Quick Base data */

			/* Gross Calculations */
			// Hourly
			var hourlyLines = _context.PlantPayLines.Where(x => x.BatchId == batchId &&
				(
					x.PayType == PayType.Regular
					//|| x.PayType == PayType.HourlyPlusPieces
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.PlantPayLines.Where(x => x.BatchId == batchId && x.PayType == PayType.Pieces).ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Incentives!!!
			var incentiveLines = _context.PlantPayLines.Where(x => x.BatchId == batchId && (
				x.LaborCode == (int)PlantLaborCode.TallyTagWriter
				|| (x.PayType == PayType.Pieces && !x.IsIncentiveDisqualified))).ToList();
			_grossFromIncentiveCalculator.CalculateGrossFromIncentive(incentiveLines);
			_context.UpdateRange(incentiveLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.PlantPayLines.Where(x => x.BatchId == batchId).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.UpdateRange(payLines);
			_context.SaveChanges();

			/*
				Total Gross = [Pay Type]="41.1-Adjustment" and [41.1 Approval]=false,0,[Gross from Hours]+[Gross from Pieces]+[Other Gross]+[Gross from Incentive])
				[Gross from Hours]
					[Pay Type]="1-Regular" => 		([Hourly Rate]+[RoundingFix])*[Hours Worked]
					[Pay Type]="49-Reporting Pay" => 	[Hourly Rate]*[Hours Worked]
					[Pay Type]="41.1-Adjustment" =>		[Hourly Rate]*[Hours Worked]
					[Pay Type]="48-Comp Time" =>		[Hourly Rate]*[Hours Worked]
					[Pay Type]="7.2-Sick Leave" =>		[Hourly Rate]*[Hours Worked]
					[Pay Type]="7.1-Holiday" =>		[Hourly Rate]*[Hours Worked]
					[Pay Type]="7-Vacation" =>		[Hourly Rate]*[Hours Worked]
					Else => 0
					
					[Hourly Rate]
						[Pay Type]="7.2-Sick Leave" =?	[90 Day Hourly Rate]
						[H-2A] =>			[H-2A Rate]
						[Labor Code]=125 =>		[125 Rate]
						[Labor Code]=151 =>		[151 Rate]
						[Labor Code]=312 =>		[125 Rate]
						[Labor Code]=535 =>		[535 Rate]
						[Labor Code]=536 =>		[536 Rate]
						[Labor Code]=537 =>		[537 Rate]
						[Labor Code]=9503 =>		[503 Rate]
						[Labor Code]=503 =>		[503 Rate]
						ELSE =>				[EmployeeHourlyRateCalc]
						
						[90 Day Hourly Rate] lookup of PSL Tracking Daily: 90 Day Hourly Rate
						[H-2A] checkbox lookup of Employee Master: H-2A (Employee Number)
						[H-2A Rate] = 13.92
						[125 Rate] = 
							[Shift Date]<ToDate("5-27-2019") => If([EmployeeHourlyRateCalc]<12.5,12.5,[EmployeeHourlyRateCalc])
							[H-2A]=true =>(If([Plant]=3,[H-2A Rate],If([EmployeeHourlyRateCalc]<13,13,[EmployeeHourlyRateCalc])))
							[EmployeeHourlyRateCalc] < 13 => 13
							ELSE [EmployeeHourlyRateCalc]
						[151 Rate] = [EmployeeHourlyRateCalc] + 2
						[535 Rate] = 
							[Plant]=11 => [EmployeeHourlyRateCalc]
							[EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
							ELSE [EmployeeHourlyRateCalc]
						[536 Rate] = [EmployeeHourlyRateCalc] + 3
						[537 Rate] = [EmployeeHourlyRateCalc] + 1.5
						[503 Rate] =
							[Shift Date]<ToDate("5-27-2019") => If([EmployeeHourlyRateCalc]<12,12,[EmployeeHourlyRateCalc])
							If([EmployeeHourlyRateCalc]<13,13,[EmployeeHourlyRateCalc]))
						[EmployeeHourlyRateCalc] = 
							IsNull([Hourly Rate Override]) => (If([Employee Hourly Rate]<[Minimum Wage],[Minimum Wage],[Employee Hourly Rate]))
							ELSE [Hourly Rate Override])
						[Hourly Rate Override] is data entry
						[Employee Hourly Rate] is lookup of Employee Master: Plants Hourly Rate
						[Minimum Wage] (same as ranches formula)
						
					[Hours Worked] is data entry
					[RoundingFix] = [Hours Worked]-Floor([Hours Worked]) > 0 => 0.01, ELSE 0
				
				[Gross from Incentive] = If([Labor Code]=555,[555 Rate]*[Hours Worked], ([BonusPieceRate]*[Pieces]))
					[555 Rate] = 
						If([Incentive Disqualified]=true,0,
						[Plant]=3,2,
						[Plant]=2,2,
						[Plant]=4,2,0)
						
					[BonusPieceRate] = If(
						[NonPrimaViolation]="Yes",0,
						[Increased Rate]=true,[IncreasedRate]-[NonPrimaRate],[PrimaRate]-[NonPrimaRate])
						
					[NonPrimaViolation] is text data entry (expected to be "Yes" or "No"
					[Increased Rate] is a checkbox data entry
					[IncreasedRate] is currency data entry
					[NonPrimaRate] is currency data entry
					[PrimaRate] is currency data entry
					[Incentive Disqualified] is a checkbox data entry
			 */

			/* PSL Requires hours and gross for regular, piece; also requires hours on Sick Leave pay types*/
			// Perform PSL calculations
			var startDate = _context.PlantPayLines.Where(x => x.BatchId == batchId).OrderBy(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;
			var endDate = _context.PlantPayLines.Where(x => x.BatchId == batchId).OrderByDescending(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;

			_paidSickLeaveService.UpdateTracking(batchId, company);
			_paidSickLeaveService.CalculateNinetyDay(batchId, company, startDate, endDate);

			// Update PSL usage
			_paidSickLeaveService.UpdateUsage(batchId, company);

			// Set PSL rate and recalculate gross.
			var paidSickLeaves = _context.PlantPayLines
				.Where(x =>
					x.BatchId == batchId
					&& x.PayType == PayType.SickLeave)
				.ToList();
			paidSickLeaves.ForEach(x => x.HourlyRate = _paidSickLeaveService.GetNinetyDayRate(batchId, Company.Plants, x.EmployeeId, x.ShiftDate));
			_grossFromHoursCalculator.CalculateGrossFromHours(paidSickLeaves);
			_totalGrossCalculator.CalculateTotalGross(paidSickLeaves);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
			// Daily summaries group all of the plant pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of plant sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummaries(batchId, company);

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
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross
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
				PayType = PayType.OverTime,
				HoursWorked = x.OverTimeHours,
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
				PayType = PayType.DoubleTime,
				HoursWorked = x.DoubleTimeHours,
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
				PayType = PayType.WeeklyOverTime,
				HoursWorked = x.OverTimeHours
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
			//var reportingPayRecords = _context.PlantPayLines.Where(x => x.BatchId == batchId && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			var reportingPayRecords = _context.PlantPayLines.Where(x => x.BatchId == batchId && x.PayType == PayType.ReportingPay).ToList();
			reportingPayRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.PlantPayLines.Where(x => x.BatchId == batchId && (x.LaborCode == (int)PlantLaborCode.RecoveryTime || x.LaborCode == (int)PlantLaborCode.NonProductiveTime)).ToList();
			nonProductiveRecords.ForEach(x =>
			{
				var weeklySummary = weeklySummaries.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).FirstOrDefault();
				x.HourlyRateOverride = (weeklySummary == null ? 0 : weeklySummary.EffectiveHourlyRate);
			});
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();

			/* Calculate Adjustments */
			CalculatePlantAdjustments(batchId, company);

			/* Create Summaries */
			var summaries = _plantSummaryService.CreateSummariesForBatch(batchId);
			_context.Add(summaries);
			_context.SaveChanges();

			/* Update records to Quick Base */
			// Ranch Payroll Records
			// Ranch Adjustment Records
			// Ranch Summary Records
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
					|| x.PayType == PayType.SickLeave))
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
			var dailySummaries = _dailySummaryCalculator.GetDailySummaries(batchId, company);

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
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross
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
				PayType = PayType.OverTime,
				HoursWorked = x.OverTimeHours,
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
				PayType = PayType.DoubleTime,
				HoursWorked = x.DoubleTimeHours,
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
				PayType = PayType.WeeklyOverTime,
				HoursWorked = x.OverTimeHours
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
				.GroupBy(x => new { x.EmployeeId, x.WeekEndDate }, (key, group) => new
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					Gross = group.Select(g => g.TotalGross * (g.IsOriginal ? -1 : 1)).Sum()
				})
				.Select(x => new PlantPayLine
				{
					EmployeeId = x.EmployeeId,
					WeekEndDate = x.WeekEndDate,
					ShiftDate = x.WeekEndDate,
					PayType = PayType.Adjustment,
					OtherGross = x.Gross,
					TotalGross = x.Gross
				})
				.ToList();
			_context.AddRange(adjustmentLines);
			_context.SaveChanges();
		}


		public void PerformRanchCalculations(int batchId)
		{
			var company = Company.Ranches;

			/* Download Quick Base data */

			/* Crew Boss Calculations */
			var crewBossPayLines = _crewBossPayService.CalculateCrewBossPay(batchId);

			// Add crew boss pay lines to database.
			_context.AddRange(crewBossPayLines);
			_context.SaveChanges();


			/* Gross Calculations */
			// Hourly
			var hourlyLines = _context.RanchPayLines.Where(x => x.BatchId == batchId &&
				(
					x.PayType == PayType.Regular
					|| x.PayType == PayType.HourlyPlusPieces
					|| x.PayType == PayType.SpecialAdjustment
					|| x.PayType == PayType.ReportingPay
					|| x.PayType == PayType.CompTime
					|| x.PayType == PayType.Vacation
					|| x.PayType == PayType.Holiday))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.RanchPayLines.Where(x => x.BatchId == batchId &&
				(
					x.PayType == PayType.Pieces
					|| x.PayType == PayType.HourlyPlusPieces))
				.ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.RanchPayLines.Where(x => x.BatchId == batchId).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.UpdateRange(payLines);
			_context.SaveChanges();

			/*
				In Quick Base the final value of pay line is in the [Total Gross] field which is a formula field:
					If([Pay Type]="41.1-Adjustment" and [41.1 Approval]=false,0,[Gross from Hours]+[Gross from Pieces]+[Other Gross])
				[Gross from Hours]
					Round((If([Pay Type]="4-Pieces",0,
					[Pay Type]="1-Regular",[Hourly Rate]*[Hours Worked],
					[Pay Type]="4.1-Hourly plus Pieces",[Hourly Rate]*[Hours Worked],
					[Pay Type]="41.1-Adjustment",[Hourly Rate]*[Hours Worked],
					[Pay Type]="49-Reporting Pay",[Hourly Rate]*[Hours Worked],
					[Pay Type]="48-Comp Time",[Hourly Rate]*[Hours Worked],
					[Pay Type]="7.2-Sick Leave",[Hourly Rate]*[Hours Worked],
					[Pay Type]="7.1-Holiday",[Hourly Rate]*[Hours Worked],
					[Pay Type]="7-Vacation",[Hourly Rate]*[Hours Worked],0)),0.01)
					
					[Hourly Rate]
						If([Hourly Rate Override]>0,[Hourly Rate Override],
						[Pay Type]="7.2-Sick Leave",[90 Day Hourly Rate],
						[Labor Code]=103 => [LC103Rate] = If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],14.25)
						[Labor Code]=104 => [LC104Rate] = If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate]+1,15.25)
						[Labor Code]=105 => [LC105Rate] = If([Crew]=65,If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],14),If([Employee Hourly Rate]>[Crew Labor Rate],[Employee Hourly Rate],[Crew Labor Rate]))
						[Labor Code]=116 => [LC116Rate] = 12
						**[Labor Code]=117 => [LC117Rate] = NULL**
						[Labor Code]=120 => [LC120Rate] = [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Labor Code]=380,0,
						[Labor Code]=381,0,
						[Crew]=1 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=3 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=7 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=8 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=15 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=56 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=57 => [CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=27 => [Crew27Rate] = [CulturalRate]+0.5
						[Crew]=69 => [CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
						[Crew]=75,[Crew Labor Rate],
						[Crew]=76,[Crew Labor Rate],
						[Crew]=223 and [Labor Code]=217,[GraftingRate],
						[Crew]>100,[Crew Labor Rate],[CulturalRate])
					[Hourly Rate Override] is a data entry field
					[90 Day Hourly Rate] is a lookup value from PSL Tracking Daily: 90 Day Hourly Rate
					[CulturalRate] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
					[CulturalRate_ExceptionCrews] = If([Employee Hourly Rate]<[Crew Labor Rate],[Crew Labor Rate],[Employee Hourly Rate])
					[Crew27Rate] = [CulturalRate]+0.5
					[Crew Labor Rate] = If([Shift Date]<[Crew Labor Rate Change Date],13,14)
					[GraftingRate] = If([Shift Date]>ToDate("2-1-2018"),15,14)
					[Employee Hourly Rate] is a lookup value from Employee Master: Ranches Hourly Rate
					[Crew Labor Rate Change Date] = ToDate("1-20-2020")
					[PSL Tracking Daily: 90 Day Hourly Rate] = If(([90 Day Gross]/[90 Day Hours])>[Minimum Wage],([90 Day Gross]/[90 Day Hours]),[Minimum Wage])
					[Employee Master: Ranches Hourly Rate] is a data entry field
					
					[Minimum Wage] (Ranch Payroll/PSL Tracking Daily) = If(
						[Shift Date]<ToDate("7-1-2014"),ToNumber([Minimum_Wage]),
						[Shift Date]<ToDate("1-1-2016"),ToNumber([Minimum_Wage_20140701]),
						[Shift Date]<ToDate("1-1-2017"),ToNumber([Minimum_Wage_20160101]),
						[Shift Date]<ToDate("1-1-2018"),ToNumber([Minimum_Wage_20170101]),
						[Shift Date]<ToDate("1-1-2019"),ToNumber([Minimum_Wage_20180101]),
						[Shift Date]<ToDate("1-1-2020"),ToNumber([Minimum_Wage_20190101]),
						[Shift Date]<ToDate("1-1-2021"),ToNumber([Minimum_Wage_20200101]),
						[Shift Date]<ToDate("1-1-2022"),ToNumber([Minimum_Wage_20210101]),
						ToNumber([Minimum_Wage_20220101]))
					
					[Minimum_Wage] = 8.00
					[Minimum_Wage_20140701] = 9.00
					[Minimum_Wage_20160101] = 10.00
					[Minimum_Wage_20170101] = 10.50
					[Minimum_Wage_20180101] = 11.00
					[Minimum_Wage_20190101] = 12.00
					[Minimum_Wage_20200101] = 13.00
					[Minimum_Wage_20210101] = 14.00
					[Minimum_Wage_20220101] = 15.00
					[Hours Worked] = If([Manual Input Hours Worked]=0,([Calculated Hours Worked]-[AutoLunchAmount]),[Manual Input Hours Worked])
						[Calculated Hours Worked] is a data entry field
						[AutoLunchAmount] = If([AutoLunch]="Yes",(If([Calculated Hours Worked]>6,0.5,0)),(If([AutoLunch]="Double",(If([Calculated Hours Worked]>6,1,0)),0)))
							[AutoLunch] is a data entry field (select list)               
				[Gross from Pieces]
					If([Pay Type]="4-Pieces",[Pieces]*[Piece Rate],
					[Pay Type]="4.1-Hourly plus Pieces",[Pieces]*[Piece Rate],0)
					
					[Pieces] is a data entry field
					[Piece Rate] is a data entry field

				[Other Gross] is a data entry field
			 */

			/* PSL Requires hours and gross for regular, piece, and CB pay types; also requires hours on Sick Leave pay types*/
			// Perform PSL calculations
			var startDate = _context.RanchPayLines.Where(x => x.BatchId == batchId).OrderBy(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;
			var endDate = _context.RanchPayLines.Where(x => x.BatchId == batchId).OrderByDescending(s => s.ShiftDate).FirstOrDefault()?.ShiftDate ?? DateTime.Now;

			_paidSickLeaveService.UpdateTracking(batchId, company);
			_paidSickLeaveService.CalculateNinetyDay(batchId, company, startDate, endDate);

			// Update PSL usage
			_paidSickLeaveService.UpdateUsage(batchId, company);

			// Set PSL rate and recalculate gross.
			var paidSickLeaves = _context.RanchPayLines
				.Where(x =>
					x.BatchId == batchId
					&& x.PayType == PayType.SickLeave)
				.ToList();
			paidSickLeaves.ForEach(x => x.HourlyRate = _paidSickLeaveService.GetNinetyDayRate(batchId, Company.Ranches, x.EmployeeId, x.ShiftDate));
			_grossFromHoursCalculator.CalculateGrossFromHours(paidSickLeaves);
			_totalGrossCalculator.CalculateTotalGross(paidSickLeaves);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
			// Daily summaries group all of the ranch pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of Crew and last of FiveEight sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummaries(batchId, company);

			// Calculate OT/DT/7th Day Hours
			// This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
			_ranchDailyOverTimeHoursCalculator.SetDailyOTDTHours(dailySummaries);

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
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross
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
				PayType = PayType.OverTime,
				HoursWorked = x.OverTimeHours,
				//HourlyRateOverride = Math.Max(x.EffectiveHourlyRate, x.MinimumWage)
			}).ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(overTimeRecords);
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			_context.AddRange(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				PayType = PayType.DoubleTime,
				HoursWorked = x.DoubleTimeHours,
				//HourlyRateOverride = Math.Max(x.EffectiveHourlyRate, x.MinimumWage)
			}).ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(doubleTimeRecords);
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			_context.AddRange(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				PayType = PayType.WeeklyOverTime,
				HoursWorked = x.OverTimeHours
			}).ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(weeklyOverTimeRecords);
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			_context.AddRange(weeklyOverTimeRecords);


			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			var reportingPayRecords = _context.RanchPayLines.Where(x => x.BatchId == batchId && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			foreach (var record in reportingPayRecords)
			{
				// Needs to select Max from effective and minimum wage.
				//record.HourlyRateOverride = weeklySummaries
				//    .Where(x => x.EmployeeId == record.EmployeeId && x.WeekEndDate == record.WeekEndDate)
				//    .OrderByDescending(x => x.EffectiveHourlyRate)
				//    .FirstOrDefault()
				//    ?.EffectiveHourlyRate
				//    ?? 0;
			}
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.RanchPayLines.Where(x => x.BatchId == batchId && (x.LaborCode == (int)RanchLaborCode.RecoveryTime || x.LaborCode == (int)RanchLaborCode.NonProductiveTime)).ToList();
			foreach (var record in nonProductiveRecords)
			{
				// Needs to select Max from effective and minimum wage.
				//record.HourlyRateOverride = weeklySummaries
				//    .Where(x => x.EmployeeId == record.EmployeeId && x.WeekEndDate == record.WeekEndDate)
				//    .OrderByDescending(x => x.EffectiveHourlyRate)
				//    .FirstOrDefault()
				//    ?.EffectiveHourlyRate
				//    ?? 0;
			}
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();


			/* Calculate Adjustments */
			CalculateRanchAdjustments(batchId, company);

			/* Create Summaries */
			var summaries = _ranchSummaryService.CreateSummariesForBatch(batchId);
			_context.Add(summaries);
			_context.SaveChanges();

			/* Update records to Quick Base */
			// Ranch Payroll Records
			// Ranch Adjustment Records
			// Ranch Summary Records
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
					|| x.PayType == PayType.SickLeave))
				.ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(hourlyLines);
			_context.UpdateRange(hourlyLines);
			_context.SaveChanges();

			// Pieces
			var pieceLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal &&
				(
					x.PayType == PayType.Pieces
					|| x.PayType == PayType.HourlyPlusPieces))
				.ToList();
			_grossFromPiecesCalculator.CalculateGrossFromPieces(pieceLines);
			_context.UpdateRange(pieceLines);
			_context.SaveChanges();

			// Total
			var payLines = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal).ToList();
			_totalGrossCalculator.CalculateTotalGross(payLines);
			_context.UpdateRange(payLines);
			_context.SaveChanges();


			/* OT/DT/Seventh Day Hours */
			// Daily summaries group all of the ranch pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
			// Additionally it selects the last of Crew and last of FiveEight sorting - I believe - on the Quick Base Record ID.
			// This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
			// The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
			var dailySummaries = _dailySummaryCalculator.GetDailySummariesFromAdjustments(batchId, company);

			// Calculate OT/DT/7th Day Hours
			// This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
			_ranchDailyOverTimeHoursCalculator.SetDailyOTDTHours(dailySummaries);

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
				PayType = PayType.MinimumAssurance,
				OtherGross = x.Gross
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
				PayType = PayType.OverTime,
				HoursWorked = x.OverTimeHours,
				//HourlyRateOverride = Math.Max(x.EffectiveHourlyRate, x.MinimumWage)
			}).ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(overTimeRecords);
			_totalGrossCalculator.CalculateTotalGross(overTimeRecords);
			_context.AddRange(overTimeRecords);

			var doubleTimeRecords = dailySummaries.Where(x => x.DoubleTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.ShiftDate,
				PayType = PayType.DoubleTime,
				HoursWorked = x.DoubleTimeHours,
				//HourlyRateOverride = Math.Max(x.EffectiveHourlyRate, x.MinimumWage)
			}).ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(doubleTimeRecords);
			_totalGrossCalculator.CalculateTotalGross(doubleTimeRecords);
			_context.AddRange(doubleTimeRecords);

			/* WOT Gross (Requires effective weekly rate) */
			var weeklyOverTimeRecords = weeklyOt.Where(x => x.OverTimeHours > 0).Select(x => new RanchPayLine
			{
				EmployeeId = x.EmployeeId,
				WeekEndDate = x.WeekEndDate,
				ShiftDate = x.WeekEndDate,
				PayType = PayType.WeeklyOverTime,
				HoursWorked = x.OverTimeHours
			}).ToList();
			_grossFromHoursCalculator.CalculateGrossFromHours(weeklyOverTimeRecords);
			_totalGrossCalculator.CalculateTotalGross(weeklyOverTimeRecords);
			_context.AddRange(weeklyOverTimeRecords);


			/* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */
			var reportingPayRecords = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal && (x.PayType == PayType.CompTime || x.PayType == PayType.ReportingPay)).ToList();
			foreach (var record in reportingPayRecords)
			{
				// Needs to select Max from effective and minimum wage.
				//record.HourlyRateOverride = weeklySummaries
				//    .Where(x => x.EmployeeId == record.EmployeeId && x.WeekEndDate == record.WeekEndDate)
				//    .OrderByDescending(x => x.EffectiveHourlyRate)
				//    .FirstOrDefault()
				//    ?.EffectiveHourlyRate
				//    ?? 0;
			}
			_grossFromHoursCalculator.CalculateGrossFromHours(reportingPayRecords);
			_totalGrossCalculator.CalculateTotalGross(reportingPayRecords);

			/* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
			var nonProductiveRecords = _context.RanchAdjustmentLines.Where(x => x.BatchId == batchId && !x.IsOriginal && (x.LaborCode == (int)RanchLaborCode.RecoveryTime || x.LaborCode == (int)RanchLaborCode.NonProductiveTime)).ToList();
			foreach (var record in nonProductiveRecords)
			{
				// Needs to select Max from effective and minimum wage.
				//record.HourlyRateOverride = weeklySummaries
				//    .Where(x => x.EmployeeId == record.EmployeeId && x.WeekEndDate == record.WeekEndDate)
				//    .OrderByDescending(x => x.EffectiveHourlyRate)
				//    .FirstOrDefault()
				//    ?.EffectiveHourlyRate
				//    ?? 0;
			}
			_grossFromHoursCalculator.CalculateGrossFromHours(nonProductiveRecords);
			_totalGrossCalculator.CalculateTotalGross(nonProductiveRecords);

			_context.SaveChanges();


			/* Figure out total Adjustments */
			/* Create new Ranch Payroll Lines for all adjustments */
			var adjustmentLines = _context.RanchAdjustmentLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(x => new { x.EmployeeId, x.WeekEndDate }, (key, group) => new
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					Gross = group.Select(g => g.TotalGross * (g.IsOriginal ? -1 : 1)).Sum()
				})
				.Select(x => new RanchPayLine
				{
					EmployeeId = x.EmployeeId,
					WeekEndDate = x.WeekEndDate,
					ShiftDate = x.WeekEndDate,
					PayType = PayType.Adjustment,
					OtherGross = x.Gross,
					TotalGross = x.Gross
				})
				.ToList();
			_context.AddRange(adjustmentLines);
			_context.SaveChanges();
		}
	}
}
