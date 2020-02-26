using Payroll.Data;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class MinimumWageService : IMinimumWageService
    {
        private readonly PayrollContext _context;

        public MinimumWageService(PayrollContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public decimal GetMinimumWageOnDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
