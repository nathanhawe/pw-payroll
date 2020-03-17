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
	public class GrossFromHoursCalculator
	{
		private readonly IRanchHourlyRateSelector _ranchHourlyRateSelector;
		private readonly IPlantHourlyRateSelector _plantHourlyRateSelector;

		public GrossFromHoursCalculator(IRanchHourlyRateSelector ranchHourlyRateSelector, IPlantHourlyRateSelector plantHourlyRateSelector)
		{
			_ranchHourlyRateSelector = ranchHourlyRateSelector ?? throw new ArgumentNullException(nameof(ranchHourlyRateSelector));
			_plantHourlyRateSelector = plantHourlyRateSelector ?? throw new ArgumentNullException(nameof(plantHourlyRateSelector));
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
					payLine.HourlyRateOverride);

				payLine.GrossFromHours = Round(payLine.HoursWorked * hourlyRate, 2);
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
					payLine.ShiftDate);

				payLine.GrossFromHours = Round(payLine.HoursWorked * hourlyRate, 2);
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
						0);
				}

				adjustmentLine.GrossFromHours = Round(adjustmentLine.HoursWorked * hourlyRate, 2);
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
						0, 
						adjustmentLine.IsH2A, 
						plant, 
						adjustmentLine.ShiftDate);
				}

				adjustmentLine.GrossFromHours = Round(adjustmentLine.HoursWorked * hourlyRate, 2);
			}
		}

		/// <summary>
		/// Rounds the provided value to the decimal places specified.  This method ensures
		/// that rounding is performed accurately for six decimal places.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="decimalPlaces"></param>
		/// <returns></returns>
		private decimal Round(decimal value, int decimalPlaces)
		{
			for(int i = 6; i > decimalPlaces; i--)
			{
				value = Math.Round(value, i, MidpointRounding.AwayFromZero);
			}
			return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
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
