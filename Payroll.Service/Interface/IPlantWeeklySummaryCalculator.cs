using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IPlantWeeklySummaryCalculator
	{
		List<WeeklySummary> GetWeeklySummary(List<DailySummary> dailySummaries, List<MinimumMakeUp> minimumMakeUps);
	}
}
