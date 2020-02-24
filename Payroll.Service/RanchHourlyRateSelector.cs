using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class RanchHourlyRateSelector : IRanchHourlyRateSelector
    {
        private ICrewLaborWageSelector _crewLaborWageSelector;
        public RanchHourlyRateSelector(ICrewLaborWageSelector crewLaborWageSelector)
        {
            _crewLaborWageSelector = crewLaborWageSelector ?? throw new ArgumentNullException(nameof(crewLaborWageSelector));
        }

        public decimal GetRanchHourlyRate(string payType, int crew, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride)
        {
            throw new NotImplementedException();
        }
    }
}
