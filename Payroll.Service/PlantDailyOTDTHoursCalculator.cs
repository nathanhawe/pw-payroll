using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Performs over time and double time hours calculation for plant pay and adjustment lines.
	/// </summary>
	public class PlantDailyOTDTHoursCalculator : IDailyOTDTHoursCalculator
	{
		public decimal RegularOverTimeThreshold { get; } = 8;
		public decimal RegularDoubleTimeThreshold { get; } = 12;
		public decimal AlternativeOverTimeThreshold { get; } = 10;
		public decimal AlternativeDoubleTimeThreshold { get; } = 12;

		/// <summary>
		/// Sets the values of <c>OverTimeHours</c> and <c>DoubleTimeHours</c> for the provided <c>DailySummary</c> objects.
		/// Over time and double time hours are calculated based on the summary's <c>AlternativeWorkWeek</c> flag.
		/// </summary>
		/// <param name="dailySummaries"></param>
		public void SetDailyOTDTHours(List<DailySummary> dailySummaries)
		{
			bool calculateSeventhDay;

			foreach (var summary in dailySummaries)
			{
				if(summary.ShiftDate.DayOfWeek == DayOfWeek.Sunday)
				{
					calculateSeventhDay = HasSevenDays(summary, dailySummaries);
				}
				else
				{
					calculateSeventhDay = false;
				}

				if (summary.AlternativeWorkWeek)
				{
					SetDailyOTDTHoursWithThresholds(summary, AlternativeOverTimeThreshold, AlternativeDoubleTimeThreshold, calculateSeventhDay);
				}
				else
				{
					SetDailyOTDTHoursWithThresholds(summary, RegularOverTimeThreshold, RegularDoubleTimeThreshold, calculateSeventhDay); 
				}
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
			if(isSeventhDay)
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
