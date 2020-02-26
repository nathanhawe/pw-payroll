using Payroll.Data;
using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    
    public class DailySummaryCalculator
    {
        private readonly PayrollContext _context;
        private readonly IMinimumWageService _minimumWageService;

        public DailySummaryCalculator(PayrollContext context, IMinimumWageService minimumWageService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(context));
        }

        public List<DailySummary> GetDailySummaries(int batchId)
        {
            throw new NotImplementedException();
        }
    }
}
