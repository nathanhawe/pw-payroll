using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    
    public class DailySummaryCalculator
    {
        private readonly PayrollContext _context;
        public DailySummaryCalculator(PayrollContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<DailySummary> GetDailySummaries(int batchId)
        {
            throw new NotImplementedException();
        }
    }
}
