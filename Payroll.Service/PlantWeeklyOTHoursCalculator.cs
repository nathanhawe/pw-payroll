using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Performs weekly over time hours calculations for plant weekly summaries.
	/// </summary>
	public class PlantWeeklyOTHoursCalculator : IWeeklyOTHoursCalculator
	{
		private decimal WeeklyOverTimeThreshold { get; } = 40M;
		private readonly IRoundingService _roundingService;

		public PlantWeeklyOTHoursCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Creates new <c>WeeklyOverTimeHours</c> for the provided <c>WeeklySummary</c> objects.  This method 
		/// expects there is only one <c>WeeklySummary</c> for each employee and week end date.
		/// </summary>
		/// <param name="weeklySummaries"></param>
		/// <returns></returns>
		public List<WeeklyOverTimeHours> GetWeeklyOTHours(List<WeeklySummary> weeklySummaries)
		{
			decimal total;
			var weeklyOverTimeHours = new List<WeeklyOverTimeHours>();

			foreach(var summary in weeklySummaries)
			{
				total = _roundingService.Round(summary.TotalHours - summary.TotalOverTimeHours - summary.TotalDoubleTimeHours - WeeklyOverTimeThreshold, 2);
				if(total > 0)
				{
					weeklyOverTimeHours.Add(new WeeklyOverTimeHours
					{
						EmployeeId = summary.EmployeeId,
						Crew = summary.Crew,
						WeekEndDate = summary.WeekEndDate,
						OverTimeHours = total
					});
				}
			}

			return weeklyOverTimeHours;
		}
	}
}
