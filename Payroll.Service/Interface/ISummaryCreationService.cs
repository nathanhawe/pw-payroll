using Payroll.Domain;

namespace Payroll.Service.Interface
{
	public interface ISummaryCreationService
	{
		void CreateSummaries(int summaryBatchId);
	}
}
