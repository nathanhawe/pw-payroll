using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchWeeklyOTHoursCalculator
	{
		List<WeeklyOverTimeHours> GetWeeklyOTHours(List<WeeklySummary> weeklySummaries);
	}
}
