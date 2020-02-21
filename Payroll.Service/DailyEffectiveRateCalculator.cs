using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    
    public class DailyEffectiveRateCalculator
    {
        private readonly PayrollContext _context;
        public DailyEffectiveRateCalculator(PayrollContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<DailyEffectiveRate> GetDailyEffectiveRates(int batchId)
        {
            throw new NotImplementedException();
        }
    }
}
