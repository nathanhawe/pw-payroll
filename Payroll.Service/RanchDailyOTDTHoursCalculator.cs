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
	/// Performs daily over time and double time calculations for ranch pay and adjustment daily summaries.
	/// </summary>
	public class RanchDailyOTDTHoursCalculator : IRanchDailyOTDTHoursCalculator
	{
		public decimal OfficeRegularOverTimeThreshold { get; } = 8;
		public decimal OfficeRegularDoubleTimeThreshold { get; } = 12;
		public decimal OfficeAlternativeOverTimeThreshold { get; } = 10;
		public decimal OfficeAlternativeDoubleTimeThreshold { get; } = 12;



		/// <summary>
		/// Sets the values of <c>OverTimeHours</c> and <c>DoubleTimeHours</c> for the provided <c>DailySummary</c> objects.
		/// Over time and double time hours are calculated based on the summary's <c>Crew</c>, <c>FiveEights</c> flag, and
		/// <c>AlternativeWorkWeek</c> flag.
		/// </summary>
		/// <param name="dailySummaries"></param>
		public void SetDailyOTDTHours(List<DailySummary> dailySummaries)
		{
			bool calculateSeventhDay;

			foreach (var summary in dailySummaries)
			{
				if (summary.ShiftDate.DayOfWeek == DayOfWeek.Sunday)
				{
					calculateSeventhDay = HasSevenDays(summary, dailySummaries);
				}
				else
				{
					calculateSeventhDay = false;
				}

				if(summary.Crew == (int)Crew.OfficeClerical_EastWest || summary.Crew == (int)Crew.OfficeClerical_South)
				{
					OfficeClericalWageOrder(summary, calculateSeventhDay);
					
				}
				else
				{
					AgricultureWageOrder(summary, calculateSeventhDay);
				}
			}
		}

		/// <summary>
		/// Calculates daily over time based on wage order 4 office/clerical.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="calculateSeventhDay"></param>
		private void OfficeClericalWageOrder(DailySummary summary, bool calculateSeventhDay)
		{
			// Crews 8 and 9 may have employees with alternative work weeks.
			if (summary.AlternativeWorkWeek)
			{
				SetDailyOTDTHoursWithThresholds(summary, OfficeAlternativeOverTimeThreshold, OfficeAlternativeDoubleTimeThreshold, calculateSeventhDay);
			}
			else
			{
				SetDailyOTDTHoursWithThresholds(summary, OfficeRegularOverTimeThreshold, OfficeRegularDoubleTimeThreshold, calculateSeventhDay);
			}
		}

		/// <summary>
		/// Calculates daily over time based on wage order 14 agricultural labor.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="calculateSeventhDay"></param>
		private void AgricultureWageOrder(DailySummary summary, bool calculateSeventhDay)
		{
			// Regular crews may be paid using office/clerical by selecting the FiveEights flag.
			// Regular, non-FiveEight crews do not have a daily double time threshold until 1/1/2022.
			if (summary.FiveEight)
			{
				SetDailyOTDTHoursWithThresholds(summary, OfficeRegularOverTimeThreshold, OfficeRegularDoubleTimeThreshold, calculateSeventhDay);
			}
			else if(summary.ShiftDate < new DateTime(2019, 1, 1))
			{
				SetDailyOTDTHoursWithThresholds(summary, 10M, 9999M, calculateSeventhDay);
			}
			else if(summary.ShiftDate < new DateTime(2020, 1, 1))
			{
				SetDailyOTDTHoursWithThresholds(summary, 9.5M, 9999M, calculateSeventhDay);
			}
			else if(summary.ShiftDate < new DateTime(2021, 1, 1))
			{
				SetDailyOTDTHoursWithThresholds(summary, 9M, 9999M, calculateSeventhDay);
			}
			else if(summary.ShiftDate < new DateTime(2022, 1, 1))
			{
				SetDailyOTDTHoursWithThresholds(summary, 8.5M, 9999M, calculateSeventhDay);
			}
			else
			{
				SetDailyOTDTHoursWithThresholds(summary, OfficeRegularOverTimeThreshold, OfficeRegularDoubleTimeThreshold, calculateSeventhDay);
			}
		}

		/// <summary>
		/// Sets the values of <c>DoubleTimeHours</c> and <c>OverTimeHours</c> based on the provided OT and DT thresholds.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="overTimeThreshold"></param>
		/// <param name="doubleTimeThreshold"></param>
		/// <param name="isSeventhDay"></param>
		private void SetDailyOTDTHoursWithThresholds(DailySummary summary, decimal overTimeThreshold, decimal doubleTimeThreshold, bool isSeventhDay)
		{
			// If this employee has worked all seven days and this is the seventh day
			// then calculate seventh day pay.
			if (isSeventhDay)
			{
				overTimeThreshold = 0;
				doubleTimeThreshold = 8;
			}

			if (summary.TotalHours > doubleTimeThreshold)
			{
				summary.DoubleTimeHours = summary.TotalHours - doubleTimeThreshold;
				summary.OverTimeHours = doubleTimeThreshold - overTimeThreshold;
			}
			else if (summary.TotalHours > overTimeThreshold)
			{
				summary.DoubleTimeHours = 0;
				summary.OverTimeHours = summary.TotalHours - overTimeThreshold;
			}
			else
			{
				summary.DoubleTimeHours = summary.OverTimeHours = 0;
			}
		}

		/// <summary>
		/// Returns true if employee in provided <c>DailySummary</c> worked seven days in the same week ending period.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="dailySummaries"></param>
		/// <returns></returns>
		private bool HasSevenDays(DailySummary summary, List<DailySummary> dailySummaries)
		{
			var distinctDays = dailySummaries
				.Where(x =>
					x.WeekEndDate == summary.WeekEndDate
					&& x.EmployeeId == summary.EmployeeId
					&& x.TotalHours > 0)
				.GroupBy(g => g.ShiftDate)
				.Count();

			return (distinctDays == 7);
		}
	}
}
