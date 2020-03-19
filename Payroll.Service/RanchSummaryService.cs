using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	public class RanchSummaryService
	{
		private readonly PayrollContext _context;

		public RanchSummaryService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public List<RanchSummary> CreateSummariesForBatch(int batchId)
		{
			throw new NotImplementedException();
		}
	}
}
