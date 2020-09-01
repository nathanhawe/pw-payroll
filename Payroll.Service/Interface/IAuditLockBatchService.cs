using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IAuditLockBatchService
	{
		void AddAuditLockBatch(Domain.AuditLockBatch batch, string owner);
		Domain.AuditLockBatch GetAuditLockBatch(int id);
		List<Domain.AuditLockBatch> GetAuditLockBatches(int offset, int limit, bool orderByDescending);
		bool CanAddAuditLockBatch();
		int GetTotalAuditLockBatchCount();
	}
}
