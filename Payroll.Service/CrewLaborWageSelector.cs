using Payroll.Data;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class CrewLaborWageSelector : ICrewLaborWageSelector
    {
        private PayrollContext _context;
        public CrewLaborWageSelector(PayrollContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public decimal GetCrewLaborWage(DateTime shiftDate)
        {
            throw new NotImplementedException();
        }
    }
}
