using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    /// <summary>
    /// Manages the calculation workflow
    /// </summary>
    public class TimeAndAttendanceService
    {
        public void PerformCalculations(int batchId, string company)
        {
            // Perform Gross calculations on all paylines in the batch. 
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
            // Update PSL usage

            /* OT Hours */
            /* DT Hours */
            /* WOT Hours */

            /* OT Gross (Requires effective daily rate) */
            /* DT Gross (Requires effective daily rate) */
            /* WOT Gross (Requires effective weekly rate) */

            /* Seventh Day Pay (Requires effective weekly rate) */

            /* Update Reporting Pay / Comp Time hourly rates (Requires effective weekly rate) */

            /* Update Non-Productive Time hourly rates (Requires effective weekly rate) */






        }
    }
}
