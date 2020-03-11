using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class PlantHourlyRateSelector : IPlantHourlyRateSelector
    {
        private readonly IMinimumWageService _minimumWageService;

        public PlantHourlyRateSelector(IMinimumWageService minimumWageService)
        {
            _minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(minimumWageService));
        }

        public decimal GetHourlyRate(string payType, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, bool isH2A, Domain.Constants.Plant plant, DateTime shiftDate)
        {
            throw new NotImplementedException();
        }
    }
}
