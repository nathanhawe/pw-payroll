namespace Payroll.Service.Interface
{
	public interface IAuditLockService
	{
		void ProcessAuditLockBatch(int auditLockBatchId);
	}
}
