using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchMinimumMakeUpCalculator
	{
		List<MinimumMakeUp> GetMinimumMakeUps(List<WeeklySummary> weeklySummaries);
	}
}
