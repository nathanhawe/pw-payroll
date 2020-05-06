using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchSummaryService
	{
		List<RanchSummary> CreateSummariesForBatch(int batchId);
	}
}
