using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class GrossFromHoursCalculator
    {
        private IRanchHourlyRateSelector _ranchHourlyRateSelector;
        public GrossFromHoursCalculator(IRanchHourlyRateSelector ranchHourlyRateSelector)
        {
            _ranchHourlyRateSelector = ranchHourlyRateSelector ?? throw new ArgumentNullException(nameof(ranchHourlyRateSelector));
        }
        public void CalculateGrossFromHours(List<RanchPayLine> ranchPayLines)
        {
            //decimal hourlyRate;
            //foreach(var payLine in ranchPayLines)
            //{
            //    hourlyRate = _ranchHourlyRateSelector.GetRanchHourlyRate(payLine.PayType, payLine.Crew, payLine.LaborCode, payLine.EmployeeHourlyRate, payLine.HourlyRateOverride);
            //    payLine.GrossFromHours = Math.Round(payLine.HoursWorked * hourlyRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }
    }
}
