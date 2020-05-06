using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IPlantDailyOTDTHoursCalculator
	{
		public void SetDailyOTDTHours(List<DailySummary> dailySummaries);
	}
}
