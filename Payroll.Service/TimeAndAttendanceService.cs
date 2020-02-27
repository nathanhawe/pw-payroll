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
        private TotalGrossCalculator _totalGrossCalculator;
        private DailySummaryCalculator _dailySummaryCalculator;
        private WeeklySummaryCalculator _weeklySummaryCalculator;
        private IDailyOTDTHoursCalculator _dailyOverTimeHoursCalculator;
        private IWeeklyOTHoursCalculator _weeklyOverTimeHoursCalculator;
        private RanchMinimumMakeUpCalculator _minimumMakeUpCalculator;

        public TimeAndAttendanceService(CrewBossPayService crewBossPayService, PaidSickLeaveService paidSickLeaveService)
        {
            _crewBossPayService = crewBossPayService ?? throw new ArgumentNullException(nameof(crewBossPayService));
            _paidSickLeaveService = paidSickLeaveService ?? throw new ArgumentNullException(nameof(paidSickLeaveService));

        }
        public void PerformCalculations(int batchId, string company)
        {
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
            _paidSickLeaveService.CalculateNinetyDay(batchId, startDate, endDate);

            // Update PSL usage
            _paidSickLeaveService.UpdateUsage(batchId, company);

            // Set PSL rate and recalculate gross.



            /* OT/DT/Seventh Day Hours */
            // Daily summaries group all of the ranch pay lines by Employee, Week End Date, Shift Date, Alternative Work Week, and Minimum Wage.
            // Additionally it selects the last of Crew and last of FiveEight sorting - I believe - on the Quick Base Record ID.
            // This needs to be double checked before going to production and the actual rules for the calculation should be confirmed.
            // The effective daily rate is not used in ranches but is used in plants for the purposes of minimum make up.
            var dailySummaries = _dailySummaryCalculator.GetDailySummaries(batchId);

            // Calculate OT/DT/7th Day Hours
            // This uses the information in the daily summary to correctly calculate how many hours are over time and double time if any.
            _dailyOverTimeHoursCalculator.SetDailyOTDTHours(dailySummaries);

            // Create Weekly Summaries groups all of the daily summaries by Employee, Week End Date, and Minimum Wage and summarizes the
            // different types of hours for the week.  This information is used to figure out the effective hourly rate and create minimum
            // assurance lines.
            var weeklySummaries = _weeklySummaryCalculator.GetWeeklySummary(dailySummaries);

            // Minimum Make Up is made by comparing the effective rate against minimum wage.  If minimum wage is greater than the effective rate, the difference
            // should be used to create a minimum make up line and the higher of the two rates is used for OT, DT, etc.  When a week has multiple minimum wages, the
            // the effective rate calculated against the higher minimum wage should be used as the weekly effective rate.
            var minimumMakeUps = _minimumMakeUpCalculator.GetMinimumMakeUps(weeklySummaries);

            /* WOT Hours */
            var weeklyOt = _weeklyOverTimeHoursCalculator.GetWeeklyOTHours(weeklySummaries);

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

            /* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */


            /* Update Non-Productive Time hourly rates (Requires effective weekly rate) */
        }
    }
}
