using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IPlantWeeklyOTHoursCalculator
	{
		List<WeeklyOverTimeHours> GetWeeklyOTHours(List<WeeklySummary> weeklySummaries);
	}
}
