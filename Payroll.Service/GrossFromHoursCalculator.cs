using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class GrossFromHoursCalculator
    {
        private IRanchHourlyRateSelector _ranchHourlyRateSelector;
        private IPlantHourlyRateSelector _plantHourlyRateSelector;

        public GrossFromHoursCalculator(IRanchHourlyRateSelector ranchHourlyRateSelector, IPlantHourlyRateSelector plantHourlyRateSelector)
        {
            _ranchHourlyRateSelector = ranchHourlyRateSelector ?? throw new ArgumentNullException(nameof(ranchHourlyRateSelector));
            _plantHourlyRateSelector = plantHourlyRateSelector ?? throw new ArgumentNullException(nameof(plantHourlyRateSelector));
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

        public void CalculateGrossFromHours(List<PlantPayLine> plantPayLines)
        {
            //decimal hourlyRate;
            //Plant plant;
            //foreach (var payLine in plantPayLines)
            //{
            //    plant = Enum.IsDefined(typeof(Plant), payLine.Plant) ? (Plant)payLine.Plant : Plant.Unknown;
            //    hourlyRate = _plantHourlyRateSelector.GetHourlyRate(payLine.PayType, payLine.LaborCode, payLine.EmployeeHourlyRate, payLine.HourlyRateOverride, payLine.IsH2A, plant);
            //    payLine.GrossFromHours = Math.Round(payLine.HoursWorked * hourlyRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }

        public void CalculateGrossFromHours(List<RanchAdjustmentLine> ranchAdjustmentLines)
        {
            //decimal hourlyRate;
            //foreach (var adjustmentLine in ranchAdjustmentLines)
            //{
            //    if (adjustmentLine.UseOldHourlyRate)
            //    {
            //        hourlyRate = adjustmentLine.OldHourlyRate;
            //    }
            //    else
            //    {
            //        hourlyRate = _ranchHourlyRateSelector.GetRanchHourlyRate(adjustmentLine.PayType, adjustmentLine.Crew, adjustmentLine.LaborCode, adjustmentLine.EmployeeHourlyRate, 0);
            //    }
            //    adjustmentLine.GrossFromHours = Math.Round(adjustmentLine.HoursWorked * hourlyRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }

        
    }
}
