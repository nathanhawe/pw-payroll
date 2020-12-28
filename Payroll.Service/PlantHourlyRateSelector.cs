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
		/// <returns></returns>
		public decimal GetHourlyRate(string payType, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, bool isH2A, Domain.Constants.Plant plant, DateTime shiftDate, decimal payLineHourlyRate)
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
				&& payType != PayType.Covid19W)
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
			
			if(
				payType == PayType.Covid19 
				|| payType == PayType.Covid19WageContinuation 
				|| payType == PayType.Covid19W)
			{
				if(isH2A)
				{
					return H2ARate(shiftDate);
				}
				else
				{
					return Math.Max(payLineHourlyRate, calculatedEmployeeRate);
				}
			}
			
			if (isH2A)
			{
				return H2ARate(shiftDate);
			}

			if (laborCode == (int)PlantLaborCode.Palletizing)
			{
				return Rate125(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.FreshCut)
			{
				return Rate151(shiftDate, plant, calculatedEmployeeRate);
			}
			
			if (laborCode == (int)PlantLaborCode.ReceivingAndMarkingGrapes)
			{
				// Same as 125
				return Rate125(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}
			
			if (laborCode == (int)PlantLaborCode.NightSanitation)
			{
				return Rate535(shiftDate, plant, calculatedEmployeeRate, minimumWage, H2ARate(shiftDate));
			}
			
			if (laborCode == (int)PlantLaborCode.NightShiftSupervision)
			{
				return Rate536(shiftDate, calculatedEmployeeRate);
			}
			
			if (laborCode == (int)PlantLaborCode.NightShiftAuditor)
			{
				// [537 Rate] = [EmployeeHourlyRateCalc] + 1.5
				return (calculatedEmployeeRate + 1.5M);
			}
			
			if (laborCode == (int)PlantLaborCode.Receiving_Break)
			{
				return Rate503(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}
			
			if (laborCode == (int)PlantLaborCode.ReceivingFreshFruit)
			{
				return Rate503(shiftDate, plant, calculatedEmployeeRate, minimumWage);
			}

			if (laborCode == (int)PlantLaborCode.Covid19PreScreening)
			{
				return Rate602(shiftDate, calculatedEmployeeRate);
			}

			if (laborCode == (int)PlantLaborCode.Covid19Sanitation)
			{
				return Rate603(shiftDate, calculatedEmployeeRate);
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
			if (shiftDate < new DateTime(2020, 5, 18)) return 13.92M;
			return 14.77M;
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
			/*
				 [125 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12.5)
					[Shift Date] < #3/2/2020# => 
						[Plant] = 11 => [Minimum Wage] + 0.5
						ELSE Max([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => MAX([EmployeeHourlyRateCalc], [Minimum Wage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
			*/
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
			else
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
		}

		/// <summary>
		/// Returns the calculated rate for LC151 work.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="plant"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <returns></returns>
		private decimal Rate151(DateTime shiftDate, Plant plant, decimal calculatedEmployeeRate)
		{
			/*
				[151 Rate]
					[Plant] = 2 => [EmployeeHourlyRateCalc] + 2
					ELSE [EmployeeHourlyRateCalc]
			*/
			if(shiftDate < new DateTime(2020, 5, 18))
			{
				return calculatedEmployeeRate + (plant == Plant.Reedley ? 2 : 0);
			}
			else
			{
				return calculatedEmployeeRate + 2.0M;
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
			/*
				[503 Rate] = 
					[Shift Date] < #5-27-2019# => MAX([EmployeeHourlyRateCalc], 12)
					[Shift Date] < #3/2/2020# =>  MAX([EmployeeHourlyRateCalc], 13)
					ELSE 
						[Plant] = 11 => [EmployeeHourlyRateCalc]
						ELSE MAX([EmployeeHourlyRateCalc], Minimum + 1)
			*/
			if (shiftDate < new DateTime(2019, 5, 27))
			{
				return Math.Max(calculatedEmployeeRate, 12M);
			}
			else if (shiftDate < new DateTime(2020, 3, 2))
			{
				return Math.Max(calculatedEmployeeRate, 13M);
			}
			else
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
			/*
				[535 Rate] = 
					[Shift Date] < #3-2-2020# =>
						[Plant]=11 => [EmployeeHourlyRateCalc]
						[EmployeeHourlyRateCalc]<[H-2A Rate] => [H-2A Rate]
						ELSE [EmployeeHourlyRateCalc]
					ELSE
						[Plant]=11 => MAX([EmployeeHourlyRateCalc], [MinimumWage] + 1)
						ELSE MAX([EmployeeHourlyRateCalc], 14.77)
			*/
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
			else
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
		}

		/// <summary>
		/// Returns the calculated rate for Night Shift Supervision
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <returns></returns>
		private decimal Rate536(DateTime shiftDate, decimal calculatedEmployeeRate)
		{
			if (shiftDate < new DateTime(2020, 10, 19))
			{
				return (calculatedEmployeeRate + 3M);
			}
			else return calculatedEmployeeRate;
			
		}

		/// <summary>
		/// Returns the calculated rate for COVID-19 Pre-Screening
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="calculatedEmployeeRate"></param>
		/// <returns></returns>
		private decimal Rate602(DateTime shiftDate, decimal calculatedEmployeeRate)
		{
			if (shiftDate < new DateTime(2020, 4, 27))
			{
				return calculatedEmployeeRate;
			}
			else
			{
				return calculatedEmployeeRate + 1.5M;
			}
		}

		private decimal Rate603(DateTime shiftDate, decimal calculatedEmployeeRate)
		{
			if (shiftDate < new DateTime(2020, 7, 01))
			{
				return calculatedEmployeeRate;
			}
			else
			{
				return Math.Max(calculatedEmployeeRate, 14.5M);
			}
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
