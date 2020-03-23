using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
    public class MockRanchHourlyRateSelector : Payroll.Service.Interface.IRanchHourlyRateSelector
    {
        public decimal Rate { get; set; }

        public MockRanchHourlyRateSelector(decimal rate)
        {
            Rate = rate;
        }

        public decimal GetHourlyRate(string payType, int crew, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, DateTime shiftDate)
        {
            return Rate;
        }
    }
}
