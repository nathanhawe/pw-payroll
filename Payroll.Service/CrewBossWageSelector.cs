using Payroll.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class CrewBossWageSelector
    {
        private readonly PayrollContext _context;
        public CrewBossWageSelector(PayrollContext context)
        {
            _context = context;
        }

        public decimal GetWage(DateTime shiftDate, int countOfWorkers)
        {
            throw new NotImplementedException();
        }

    }
}
