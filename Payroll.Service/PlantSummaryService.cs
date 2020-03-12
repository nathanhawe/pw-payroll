using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class PlantSummaryService
    {
        private readonly PayrollContext _context;

        public PlantSummaryService(PayrollContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<PlantSummary> CreateSummariesForBatch(int batchId)
        {
            throw new NotImplementedException();
        }
    }
}
