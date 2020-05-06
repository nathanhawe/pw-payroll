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
	/// Performs weekly over time calculations for ranch weekly summaries.
	/// </summary>
	public class RanchWeeklyOTHoursCalculator : IRanchWeeklyOTHoursCalculator
	{
		private readonly IRoundingService _roundingService;

		public RanchWeeklyOTHoursCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Creates new <c>WeeklyOverTimeHours</c> for the provided <c>WeeklySummary</c> objects.
		/// </summary>
		/// <param name="weeklySummaries"></param>
		/// <returns></returns>
		public List<WeeklyOverTimeHours> GetWeeklyOTHours(List<WeeklySummary> weeklySummaries)
		{
			decimal total;
			decimal weeklyOverTimeThreshold;
			var weeklyOverTimeHours = new List<WeeklyOverTimeHours>();

			// There can be multiple summaries per employee and week ending in ranches so they
			// need to be grouped together by employee and week ending date before continuing.
			var groupedWeeklySummaries = weeklySummaries
				.GroupBy(g => new { g.EmployeeId, g.WeekEndDate }, (key, group) => new WeeklySummary
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					Crew = group.OrderByDescending(o => o.MinimumWage).First().Crew,
					FiveEight = group.OrderByDescending(o => o.MinimumWage).First().FiveEight,
					TotalHours = group.Sum(x => x.TotalHours),
					TotalOverTimeHours = group.Sum(x => x.TotalOverTimeHours),
					TotalDoubleTimeHours = group.Sum(x => x.TotalDoubleTimeHours)
				})
				.ToList();

			// Use the grouped summaries to create WeeklyOverTimeHours
			foreach (var summary in groupedWeeklySummaries)
			{
				weeklyOverTimeThreshold = GetWeeklyOverTimeThreshold(summary);
				total = _roundingService.Round(summary.TotalHours - summary.TotalOverTimeHours - summary.TotalDoubleTimeHours - weeklyOverTimeThreshold, 2);
				if (total > 0)
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

		/// <summary>
		/// Returns the weekly over time threshold for the passed summary.
		/// </summary>
		/// <param name="summary"></param>
		/// <returns></returns>
		private decimal GetWeeklyOverTimeThreshold(WeeklySummary summary)
		{
			if (summary.Crew == (int)Crew.OfficeClerical_EastWest || summary.Crew == (int)Crew.OfficeClerical_South) return 40M;
			if (summary.FiveEight) return 40M;
			if (summary.WeekEndDate < new DateTime(2019, 1, 1)) return 60M;
			if (summary.WeekEndDate < new DateTime(2020, 1, 1)) return 55M;
			if (summary.WeekEndDate < new DateTime(2021, 1, 1)) return 50M;
			if (summary.WeekEndDate < new DateTime(2022, 1, 1)) return 45M;
			return 40M;
		}
	}
}
