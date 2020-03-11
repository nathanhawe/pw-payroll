using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
    public class MockPlantHourlyRateSelector : IPlantHourlyRateSelector
    {
        public decimal Rate { get; set; }

        public MockPlantHourlyRateSelector(decimal rate)
        {
            Rate = rate;
        }
        
        public decimal GetHourlyRate(string payType, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, bool isH2A, Plant plant, DateTime shiftDate)
        {
            return Rate;
        }
    }
}
