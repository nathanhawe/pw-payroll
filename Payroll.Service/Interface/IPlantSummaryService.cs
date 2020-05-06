using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IPlantSummaryService
	{
		List<PlantSummary> CreateSummariesForBatch(int batchId);
	}
}
