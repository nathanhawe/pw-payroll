using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Performs minimum make up calculations for plant daily summaries.
	/// </summary>
	public class PlantMinimumMakeUpCalculator
	{
		private readonly IRoundingService _roundingService;

		public PlantMinimumMakeUpCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Creates <c>MinimumMakeUp</c>s for the provided <c>DailySummary</c> objects.
		/// </summary>
		/// <param name="dailySummaries"></param>
		/// <returns></returns>
		public List<MinimumMakeUp> GetMinimumMakeUps(List<DailySummary> dailySummaries)
		{
			var minimumMakeUps = new List<MinimumMakeUp>();
			decimal makeUpGross;

			foreach (var summary in dailySummaries)
			{
				if (summary.EffectiveHourlyRate >= summary.MinimumWage) continue;

				makeUpGross = _roundingService.Round((summary.MinimumWage * summary.TotalHours) - summary.TotalGross, 2);

				if (makeUpGross > 0)
				{
					minimumMakeUps.Add(new MinimumMakeUp
					{
						EmployeeId = summary.EmployeeId,
						Crew = summary.Crew,
						WeekEndDate = summary.WeekEndDate,
						ShiftDate = summary.ShiftDate,
						Gross = makeUpGross
					});
				}
			}

			return minimumMakeUps;
		}
	}
}
