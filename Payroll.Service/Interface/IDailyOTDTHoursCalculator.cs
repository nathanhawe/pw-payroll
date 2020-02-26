using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
    public interface IDailyOTDTHoursCalculator
    {
        public void SetDailyOTDTHours(List<DailySummary> dailySummaries);
    }
}
