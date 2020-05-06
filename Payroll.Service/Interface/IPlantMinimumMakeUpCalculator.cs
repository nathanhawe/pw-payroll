using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IPlantMinimumMakeUpCalculator
	{
		List<MinimumMakeUp> GetMinimumMakeUps(List<DailySummary> dailySummaries);
	}
}
