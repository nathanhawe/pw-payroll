using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class PaidSickLeaveService
    {
        private readonly PayrollContext _context;

        public PaidSickLeaveService(PayrollContext context)
        {
            _context = context;
        }

        public void UpdateTracking(int batchId, string company)
        {
            // Retrieve pay lines

            // Group all pay lines by employee and date, sum hours, sum gross.
            // For each grouping create a new PaidSickLeave or update the existing one

            throw new NotImplementedException();
        }

        public void UpdateUsage(int batchId, string company)
        {
            // Retrieve pay lines

            // Group all pay lines by employee and date
            // Foreach update or add the corresponding 

            throw new NotImplementedException();
        }

        public void CalculateNinetyDay(int batchId, DateTime startDate, DateTime endDate)
        {
            // Retrieve all PSL lines

            // For each PSL within startDate and endDate, figure out 90 day hour and gross totals
            throw new NotImplementedException();
        }

    }
}
