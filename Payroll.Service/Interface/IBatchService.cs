using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IBatchService
	{
		void AddBatch(Domain.Batch batch, string owner);
		Domain.Batch GetBatch(int id);
		List<Domain.Batch> GetBatches(int pageNumber, int itemsPerPage, bool orderByDescending);
	}
}
