using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Performs hourly rate calculation for plants.
	/// </summary>
	public class PlantHourlyRateSelector : IPlantHourlyRateSelector
	{
		private readonly IMinimumWageService _minimumWageService;

		public PlantHourlyRateSelector(IMinimumWageService minimumWageService)
		{
			_minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(minimumWageService));
		}

		/// <summary>
		/// Returns the effective hourly rate for the parameter values provided.
		/// </summary>
		/// <param name="payType"></param>
		/// <param name="laborCode"></param>
		/// <param name="employeeHourlyRate"></param>
		/// <param name="hourlyRateOverride"></param>
		/// <param name="isH2A"></param>
		/// <param name="plant"></param>
		/// <param name="shiftDate"></param>
		/// <param name="payLineHourlyRate">Assumed to be the 90-Day Hourly Rate for sick leave and COVID-19 lines, othewise 0</param>
		/// <param name="positionTitle"></param>
		/// <returns></returns>
		public decimal GetHourlyRate(
			string payType,
			int laborCode,
			decimal employeeHourlyRate,
			decimal hourlyRateOverride,
			bool isH2A,
			Domain.Constants.Plant plant,
			DateTime shiftDate,
			decimal payLineHourlyRate,
			string positionTitle)
		{
			var minimumWage = _minimumWageService.GetMinimumWageOnDate(shiftDate);
			var calculatedEmployeeRate = EmployeeHourlyRateCalculation(employeeHourlyRate, hourlyRateOverride, minimumWage);

			if (
				payType != PayType.Regular
				&& payType != PayType.CompTime
				&& payType != PayType.ReportingPay
				&& payType != PayType.Holiday
				&& payType != PayType.Bereavement
				&& payType != PayType.Vacation
				&& payType != PayType.SpecialAdjustment
				&& payType != PayType.SickLeave
				&& payType != PayType.Covid19
				&& payType != PayType.Covid19WageContinuation
				&& payType != PayType.Covid19W
				&& payType != PayType.PremiumPay)
			{
				return 0;
			}

			if (hourlyRateOverride > 0)
			{
				return hourlyRateOverride;
			}

			if (payType == PayType.SickLeave)
			{
				if (isH2A)
				{
					return H2ARate(shiftDate);
				}
				else
				{
					return payLineHourlyRate;
				}
			}

			if (
				payType == PayType.Covid19
				|| payType == PayType.Covid19WageContinuation
				|| payType == PayType.Covid19W)
			{
				if (isH2A)
				{
					return H2ARate(shiftDate);
				}
				else
				{
					return Math.Max(payLineHourlyRate, calculatedEmployeeRate);
				}
			}

			if (payType == PayType.PremiumPay) return 0M;

			if (isH2A)
			{
				return H2ARate(shiftDate);
			}

			if (laborCode == (int)PlantLaborCode.PackerNoPieces && plant == Plant.Cutler && positionTitle == PositionTitle.PackerL2 && shiftDate >= new DateTime(2022, 4, 18))
			{
				return RatePackerL2(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.GeneralLabor && plant == Plant.Cutler && positionTitle == PositionTitle.Spider && shiftDate >= new DateTime(2023, 7, 24))
			{
				return RateSpider(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.Palletizing || laborCode == (int)PlantLaborCode.LightDuty_Palletizing)
			{
				return Rate125(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.NightSanitation || laborCode == (int)PlantLaborCode.LightDuty_NightSanitation)
			{
				return Rate535(shiftDate, plant, calculatedEmployeeRate, minimumWage, H2ARate(shiftDate));
			}

			if (laborCode == (int)PlantLaborCode.NightShiftSupervision)
			{
				return Rate536(shiftDate, plant, calculatedEmployeeRate);
			}

			if (laborCode == (int)PlantLaborCode.Receiving_Break)
			{
				return Rate503(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.ReceivingFreshFruit || laborCode == (int)PlantLaborCode.LightDuty_ReceivingFreshFruit)
			{
				return Rate503(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.FoodSafetyNightShift)
			{
				return Rate536(shiftDate, plant, calculatedEmployeeRate);
			}

			if (laborCode == (int)PlantLaborCode.Charoleros && plant == Plant.Cutler && shiftDate >= new DateTime(2023, 5, 15))
			{
				return Math.Max(employeeHourlyRate, minimumWage + .5M);
			}

			return EmployeeHourlyRateCalculation(employeeHourlyRate, hourlyRateOverride, minimumWage);
		}

		/// <summary>
		/// Returns the effective H-2A rate for the provided shift date.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		private decimal H2ARate(DateTime shiftDate)
		{
			if (shiftDate < new DateTime(2020, 5, 18))
			{
				return 13.92M;
			}
			else if (shiftDate < new DateTime(2021, 4, 26))
			{
				return 14.77M;
			}
			else if (shiftDate < new DateTime(2022, 5, 9)) 
			{
				return 16.05M;
			}
			else if (shiftDate < new DateTime(2023, 5, 1))
			{
				return 17.51M;
			}
			else return 18.65M;
		}
		
		/// <summary>
		/// Returns the calculated rate for L2 Packers in Cutler
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="plant"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <param name="minimumWage"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private decimal RatePackerL2(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate, decimal minimumWage)
		{
			if(shiftDate < new DateTime(2023, 5, 1))
			{
				return Math.Max(calculatedEmployeeRate, 15.5M);
			}
			else return Math.Max(calculatedEmployeeRate, 15.75M);
		}


		/// <summary>
		/// Returns the calculated rate for Palletizer.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="plant"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <param name="minimumWage"></param>
		/// <returns></returns>
		private decimal Rate125(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate, decimal minimumWage)
		{
			if(shiftDate < new DateTime(2019, 5, 27))
			{
				return Math.Max(calculatedEmployeeRate, 12.5M);
			}
			else if (shiftDate < new DateTime(2020, 3, 2))
			{
				if(plant == Plant.Cutler)
				{
					return (minimumWage + 0.5M);
				}
				else
				{
					return Math.Max(calculatedEmployeeRate, 13M);
				}
			}
			else if (shiftDate < new DateTime(2020, 5, 18))
			{
				if (plant == Plant.Cutler)
				{
					return Math.Max(calculatedEmployeeRate, (minimumWage + 1M));
				}
				else
				{
					return Math.Max(calculatedEmployeeRate, 14.77M);
				}
			}
			else if (shiftDate < new DateTime(2021, 3, 22))
			{
				if (plant == Plant.Cutler)
				{
					return Math.Max(calculatedEmployeeRate, (minimumWage + 1.5M));
				}
				else
				{
					return Math.Max(calculatedEmployeeRate, 14.77M);
				}
			}
			else if (shiftDate < new DateTime(2022, 4, 18))
			{
				if (plant == Plant.Sanger || plant == Plant.Kerman || plant == Plant.Cutler)
				{
					return Math.Max(calculatedEmployeeRate, 16.05M);
				}
				else return calculatedEmployeeRate;
			}
			else
			{
				if (plant == Plant.Sanger || plant == Plant.Kerman || plant == Plant.Cutler)
				{
					return Math.Max(calculatedEmployeeRate, minimumWage + 1.5M);
				}
				else return calculatedEmployeeRate;
			}
		}

		/// <summary>
		/// Returns the calculated rate for Bucket Loader.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="plant"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <param name="minimumWage"></param>
		/// <returns></returns>
		private decimal Rate503(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate, decimal minimumWage)
		{
			if (shiftDate < new DateTime(2019, 5, 27))
			{
				return Math.Max(calculatedEmployeeRate, 12M);
			}
			else if (shiftDate < new DateTime(2020, 3, 2))
			{
				return Math.Max(calculatedEmployeeRate, 13M);
			}
			else if (shiftDate < new DateTime(2021, 3, 22))
			{
				if (plant == Plant.Cutler)
				{
					return calculatedEmployeeRate;
				}
				else
				{
					return Math.Max(calculatedEmployeeRate, (minimumWage + 1));
				}
			}
			else if (shiftDate < new DateTime(2022, 4, 18))
			{
				if (plant == Plant.Sanger || plant == Plant.Kerman)
				{
					return Math.Max(calculatedEmployeeRate, 15);
				}
				else return calculatedEmployeeRate;
			}
			else
			{
				return Math.Max(calculatedEmployeeRate, minimumWage + 1);
			}
		}

		/// <summary>
		/// Returns the calculated rate for Night Sanitation Labor.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="plant"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <param name="minimumWage"></param>
		/// <param name="h2ARate"></param>
		/// <returns></returns>
		private decimal Rate535(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate, decimal minimumWage, decimal h2ARate)
		{
			if (shiftDate < new DateTime(2020, 3, 2))
			{
				if (plant == Plant.Cutler)
				{
					return calculatedEmployeeRate;
				}
				else
				{
					return Math.Max(calculatedEmployeeRate, h2ARate);
				}
			}
			else if (shiftDate < new DateTime(2021, 3, 22))
			{
				if (plant == Plant.Cutler)
				{
					return Math.Max(calculatedEmployeeRate, (minimumWage + 1));
				}
				else
				{
					return Math.Max(calculatedEmployeeRate, 14.77M);
				}
			}
			else if (shiftDate < new DateTime(2022, 4, 18))
			{
				if (plant == Plant.Cutler)
				{
					return Math.Max(calculatedEmployeeRate, (minimumWage + 1));
				}
				else if (plant == Plant.Sanger || plant == Plant.Kerman)
				{
					return Math.Max(calculatedEmployeeRate, 16.05M);
				}
				else return calculatedEmployeeRate;
			}
			else if (shiftDate < new DateTime(2023, 4, 29))
			{
				if (plant == Plant.Cutler || plant == Plant.Sanger || plant == Plant.Kerman || plant == Plant.Reedley)
				{
					return Math.Max(calculatedEmployeeRate, 17.51M);
				}
				else return calculatedEmployeeRate;
			}
			else
			{
				if (plant == Plant.Cutler || plant == Plant.Reedley)
				{
					return Math.Max(calculatedEmployeeRate, 17.51M);
				}
				else if (plant == Plant.Sanger || plant == Plant.Kerman)
				{
					return Math.Max(calculatedEmployeeRate, 18.65M);
				}
				else return calculatedEmployeeRate;
			}
			
		}

		/// <summary>
		/// Returns the calculated rate for Night Shift Supervision
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <returns></returns>
		private decimal Rate536(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate)
		{
			if (shiftDate < new DateTime(2020, 10, 19))
			{
				return (calculatedEmployeeRate + 3M);
			}
			else if (shiftDate < new DateTime(2021, 3, 22))
			{
				return calculatedEmployeeRate;
			}
			else if (shiftDate < new DateTime(2022, 4, 18))
			{
				if ((plant == Plant.Sanger || plant == Plant.Kerman || plant == Plant.Cutler) 
					&& shiftDate.Month >= 5 
					&& (shiftDate.Month < 10 || (shiftDate.Month == 10 && shiftDate.Day <= 15)))
				{
					return (calculatedEmployeeRate + 3M);
				}
				else return calculatedEmployeeRate;
			}
			else if (shiftDate < new DateTime(2023, 5, 1))
			{
				if ((plant == Plant.Sanger || plant == Plant.Kerman || plant == Plant.Cutler || plant == Plant.Reedley)
					&& shiftDate.Month >= 5
					&& (shiftDate.Month < 10 || (shiftDate.Month == 10 && shiftDate.Day <= 15)))
				{
					return (calculatedEmployeeRate + 2M);
				}
				else return calculatedEmployeeRate;
			}
			else
			{
				if ((plant == Plant.Sanger || plant == Plant.Kerman || plant == Plant.Cutler)
					&& shiftDate.Month >= 5
					&& (shiftDate.Month < 10 || (shiftDate.Month == 10 && shiftDate.Day <= 15)))
				{
					return (calculatedEmployeeRate + 2M);
				}
				else return calculatedEmployeeRate;
			}
			
		}

		/// <summary>
		/// Returns the effective rate for the "SPIDER" job
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="plant"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <param name="minimumWage"></param>
		/// <returns></returns>
		private decimal RateSpider(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate, decimal minimumWage)
		{
			return Math.Max(calculatedEmployeeRate, 15.75M);
		}

		/// <summary>
		/// Returns the basic employee hourly rate calculated value.  This mimics the [EmployeeHourlyRateCalc] formula
		/// field in Quick Base.
		/// </summary>
		/// <param name="employeeHourlyRate"></param>
		/// <param name="hourlyRateOverride"></param>
		/// <param name="minimumWage"></param>
		/// <returns></returns>
		private decimal EmployeeHourlyRateCalculation(decimal employeeHourlyRate, decimal hourlyRateOverride, decimal minimumWage)
		{
			// Override rate always takes precedence
			if (hourlyRateOverride > 0) return hourlyRateOverride;

			return Math.Max(employeeHourlyRate, minimumWage);
		}
	}
}
