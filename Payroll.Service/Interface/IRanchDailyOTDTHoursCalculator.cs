using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchDailyOTDTHoursCalculator
	{
		public void SetDailyOTDTHours(List<DailySummary> dailySummaries);
	}
}
