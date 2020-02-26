using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
    interface IWeeklyOTHoursCalculator
    {
        public List<WeeklyOverTimeHours> GetWeeklyOTHours(List<WeeklySummary> weeklySummaries);
    }
}
