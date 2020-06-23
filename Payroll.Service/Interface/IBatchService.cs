using Payroll.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IBatchService
	{
		void AddBatch(Domain.Batch batch, string owner);
		Domain.Batch GetBatch(int id);
		List<Domain.Batch> GetBatches(int offset, int limit, bool orderByDescending);
		bool CanAddBatch();
		Domain.Batch GetCurrentlyProcessingBatch();
		int GetTotalBatchCount();
	}
}
