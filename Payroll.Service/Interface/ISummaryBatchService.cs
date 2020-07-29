using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ISummaryBatchService
	{
		void AddSummaryBatch(Domain.SummaryBatch batch, string owner);
		Domain.SummaryBatch GetSummaryBatch(int id);
		List<Domain.SummaryBatch> GetSummaryBatches(int offset, int limit, bool orderByDescending);
		bool CanAddSummaryBatch();
		Domain.SummaryBatch GetCurrentlyProcessingSummaryBatch();
		int GetTotalSummaryBatchCount();
	}
}
