using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchWeeklySummaryCalculator
	{
		List<WeeklySummary> GetWeeklySummary(List<DailySummary> dailySummaries);
	}
}
