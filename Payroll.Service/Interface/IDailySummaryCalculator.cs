using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IDailySummaryCalculator
	{
		List<DailySummary> GetDailySummaries(int batchId, string company);
		List<DailySummary> GetDailySummariesFromAdjustments(int batchId, string company);
	}
}
