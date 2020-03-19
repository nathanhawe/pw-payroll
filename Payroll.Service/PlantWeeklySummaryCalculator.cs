using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Creates weekly summaries from plant daily summaries.
	/// </summary>
	public class PlantWeeklySummaryCalculator
	{
		private readonly IRoundingService _roundingService;

		public PlantWeeklySummaryCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Produces <c>WeeklySummary</c> objects using tee provided collections of plant <c>DailySummary</c> and <c>MinimumMakeUp</c> objects.
		/// </summary>
		/// <param name="dailySummaries"></param>
		/// <param name="minimumMakeUps"></param>
		/// <returns></returns>
		public List<WeeklySummary> GetWeeklySummary(List<DailySummary> dailySummaries, List<MinimumMakeUp> minimumMakeUps)
		{
			var weeklySummaries = dailySummaries
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId }, (key, group) => new WeeklySummary
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					Crew = group.OrderByDescending(o => o.ShiftDate).First().Crew,
					FiveEight = false,
					MinimumWage = group.Max(x => x.MinimumWage),
					TotalHours = group.Sum(x => x.TotalHours),
					NonProductiveTime = group.Sum(x => x.NonProductiveTime),
					TotalGross = group.Sum(x => x.TotalGross),
					EffectiveHourlyRate = 0,
					TotalOverTimeHours = group.Sum(x => x.OverTimeHours),
					TotalDoubleTimeHours = group.Sum(x => x.DoubleTimeHours)
				})
				.ToList();

			// Add minimum assurance into the weekly summaries' total gross and calculate new effective rate.
			decimal divisor;
			weeklySummaries.ForEach(x =>
			{
				x.TotalGross += minimumMakeUps.Where(w => w.WeekEndDate == x.WeekEndDate && w.EmployeeId == x.EmployeeId).Sum(s => s.Gross);

				divisor = x.TotalHours - x.NonProductiveTime;
				x.EffectiveHourlyRate = divisor > 0 ? _roundingService.Round(x.TotalGross / divisor, 2) : 0;
			});

			return weeklySummaries;
		}
	}
}
