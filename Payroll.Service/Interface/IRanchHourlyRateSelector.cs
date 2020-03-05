using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
    public interface IRanchHourlyRateSelector
    {
        public decimal GetHourlyRate(string payType, int crew, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride);
    }
}
