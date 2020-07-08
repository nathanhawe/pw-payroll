using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Creates weekly summaries from ranch daily summaries.
	/// </summary>
	public class RanchWeeklySummaryCalculator : IRanchWeeklySummaryCalculator
	{
		private readonly IRoundingService _roundingService = new RoundingService();

		public RanchWeeklySummaryCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Produces <c>WeeklySummary</c> objects using the provided collection of ranch <c>DailySummary</c> objects.
		/// </summary>
		/// <param name="dailySummaries"></param>
		/// <returns></returns>
		public List<WeeklySummary> GetWeeklySummary(List<DailySummary> dailySummaries)
		{
			var weeklySummaries = dailySummaries
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId, g.MinimumWage }, (key, group) => new WeeklySummary
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					MinimumWage = key.MinimumWage,
					Crew = group.OrderByDescending(o => o.ShiftDate).First().Crew,
					FiveEight = group.OrderByDescending(o => o.ShiftDate).First().FiveEight,
					TotalHours = group.Sum(x => x.TotalHours),
					NonProductiveTime = group.Sum(x => x.NonProductiveTime),
					NonProductiveGross = group.Sum(x => x.NonProductiveGross),
					TotalGross = group.Sum(x => x.TotalGross),
					EffectiveHourlyRate = 0,
					TotalOverTimeHours = group.Sum(x => x.OverTimeHours),
					TotalDoubleTimeHours = group.Sum(x => x.DoubleTimeHours)
				})
				.ToList();

			// Calculate new effective rate.
			decimal divisor;
			weeklySummaries.ForEach(x =>
			{
				divisor = x.TotalHours - x.NonProductiveTime;
				x.EffectiveHourlyRate = divisor > 0 ? _roundingService.Round((x.TotalGross - x.NonProductiveGross) / divisor, 2) : 0;
			});

			return weeklySummaries;
		}
	}
}
