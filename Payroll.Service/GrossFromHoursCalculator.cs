using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Performs gross from hours worked calculations on pay and adjustment lines.
	/// </summary>
	public class GrossFromHoursCalculator : IGrossFromHoursCalculator
	{
		private readonly IRanchHourlyRateSelector _ranchHourlyRateSelector;
		private readonly IPlantHourlyRateSelector _plantHourlyRateSelector;
		private readonly IRoundingService _roundingService;

		public GrossFromHoursCalculator(
			IRanchHourlyRateSelector ranchHourlyRateSelector, 
			IPlantHourlyRateSelector plantHourlyRateSelector,
			IRoundingService roundingService)
		{
			_ranchHourlyRateSelector = ranchHourlyRateSelector ?? throw new ArgumentNullException(nameof(ranchHourlyRateSelector));
			_plantHourlyRateSelector = plantHourlyRateSelector ?? throw new ArgumentNullException(nameof(plantHourlyRateSelector));
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Sets the value of <c>GrossFromHours</c> for each <c>RanchPayLine</c> passed in.
		/// </summary>
		/// <param name="ranchPayLines"></param>
		public void CalculateGrossFromHours(List<RanchPayLine> ranchPayLines)
		{
			decimal hourlyRate;
			foreach (var payLine in ranchPayLines)
			{
				hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(
					payLine.PayType,
					payLine.Crew,
					payLine.LaborCode,
					payLine.EmployeeHourlyRate,
					payLine.HourlyRateOverride,
					payLine.ShiftDate,
					payLine.HourlyRate);

				payLine.GrossFromHours = _roundingService.Round(payLine.HoursWorked * hourlyRate, 2);
				payLine.HourlyRate = hourlyRate;
			}
		}

		/// <summary>
		/// Sets the value of <c>GrossFromHours</c> for each <c>PlantPayLine</c> passed in.
		/// </summary>
		/// <param name="plantPayLines"></param>
		public void CalculateGrossFromHours(List<PlantPayLine> plantPayLines)
		{
			decimal hourlyRate;
			Plant plant;
			foreach (var payLine in plantPayLines)
			{
				plant = GetPlantFromInteger(payLine.Plant);
				
				hourlyRate = _plantHourlyRateSelector.GetHourlyRate(
					payLine.PayType,
					payLine.LaborCode,
					payLine.EmployeeHourlyRate,
					payLine.HourlyRateOverride,
					payLine.IsH2A,
					plant,
					payLine.ShiftDate,
					payLine.HourlyRate);
				
				payLine.GrossFromHours = _roundingService.Round(payLine.HoursWorked * hourlyRate, 2);
				payLine.HourlyRate = hourlyRate;
			}
		}

		/// <summary>
		/// Sets the value of <c>GrossFromHours</c> for each <c>RanchAdjustmentLine</c> passed in.
		/// </summary>
		/// <param name="ranchAdjustmentLines"></param>
		public void CalculateGrossFromHours(List<RanchAdjustmentLine> ranchAdjustmentLines)
		{
			decimal hourlyRate;
			foreach (var adjustmentLine in ranchAdjustmentLines)
			{
				if (adjustmentLine.UseOldHourlyRate)
				{
					hourlyRate = adjustmentLine.OldHourlyRate;
				}
				else
				{
					hourlyRate = _ranchHourlyRateSelector.GetHourlyRate(
						adjustmentLine.PayType, 
						adjustmentLine.Crew, 
						adjustmentLine.LaborCode, 
						adjustmentLine.EmployeeHourlyRate, 
						adjustmentLine.HourlyRateOverride,
						adjustmentLine.ShiftDate,
						adjustmentLine.HourlyRate);
				}

				adjustmentLine.GrossFromHours = _roundingService.Round(adjustmentLine.HoursWorked * hourlyRate, 2);
				adjustmentLine.HourlyRate = hourlyRate;
			}
		}

		/// <summary>
		/// Sets the value of <c>GrossFromHours</c> for each <c>PlantAdjustmentLine</c> passed in.
		/// </summary>
		/// <param name="plantAdjustmentLines"></param>
		public void CalculateGrossFromHours(List<PlantAdjustmentLine> plantAdjustmentLines)
		{
			decimal hourlyRate;
			Plant plant;

			foreach (var adjustmentLine in plantAdjustmentLines)
			{
				if (adjustmentLine.UseOldHourlyRate)
				{
					hourlyRate = adjustmentLine.OldHourlyRate;
				}
				else
				{
					plant = GetPlantFromInteger(adjustmentLine.Plant);

					hourlyRate = _plantHourlyRateSelector.GetHourlyRate(
						adjustmentLine.PayType, 
						adjustmentLine.LaborCode, 
						adjustmentLine.EmployeeHourlyRate, 
						adjustmentLine.HourlyRateOverride, 
						adjustmentLine.IsH2A, 
						plant, 
						adjustmentLine.ShiftDate,
						adjustmentLine.HourlyRate);
				}

				adjustmentLine.GrossFromHours = _roundingService.Round(adjustmentLine.HoursWorked * hourlyRate, 2);
				adjustmentLine.HourlyRate = hourlyRate;
			}
		}

		/// <summary>
		/// Converts an integer into the appropriate <c>Plant</c> enum value.
		/// </summary>
		/// <param name="plant"></param>
		/// <returns></returns>
		private Plant GetPlantFromInteger(int plant)
		{
			return Enum.IsDefined(typeof(Plant), plant) ? (Plant)plant : Plant.Unknown;
		}

	}
}
