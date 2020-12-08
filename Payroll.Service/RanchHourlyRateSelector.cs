using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Selects hourly rates for ranch pay and adjustment lines.
	/// </summary>
	public class RanchHourlyRateSelector : IRanchHourlyRateSelector
	{
		private readonly ICrewLaborWageService _crewLaborWageService;
		private readonly IMinimumWageService _minimumWageService;

		public RanchHourlyRateSelector(ICrewLaborWageService crewLaborWageService, IMinimumWageService minimumWageService)
		{
			_crewLaborWageService = crewLaborWageService ?? throw new ArgumentNullException(nameof(crewLaborWageService));
			_minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(minimumWageService));
		}

		/// <summary>
		/// Returns an hourly rate based on the parameter values provided.
		/// </summary>
		/// <param name="payType"></param>
		/// <param name="crew"></param>
		/// <param name="laborCode"></param>
		/// <param name="employeeHourlyRate"></param>
		/// <param name="hourlyRateOverride"></param>
		/// <param name="shiftDate"></param>
		/// <param name="payLineHourlyRate">Assumed to be the 90-Day Hourly Rate for sick leave and COVID-19 lines, othewise 0</param>
		/// <returns></returns>
		public decimal GetHourlyRate(string payType, int crew, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, DateTime shiftDate, decimal payLineHourlyRate)
		{
			if (!IsAnAcceptablePayType(payType)) return 0;

			var minimumWageRate = _minimumWageService.GetMinimumWageOnDate(shiftDate);
				
			if (hourlyRateOverride > 0) return hourlyRateOverride;
			if (payType == PayType.SickLeave) return payLineHourlyRate;
			if (payType == PayType.Covid19) return Math.Max(payLineHourlyRate, CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate));
			if (laborCode == (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay) return AlmondHarvestEquipmentOperatorDay(shiftDate, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight) return AlmondHarvestEquipmentOperatorNight(shiftDate, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.AlmondHarvestGeneral) return AlmondHarvestGeneral(shiftDate, crew, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.CrewHelper) return CrewHelper(shiftDate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.CrewHelper_BonusRate) return CrewHelper_BonusRate(shiftDate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.GrapeHarvestSupport) return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.Girdling && shiftDate >= new DateTime(2020, 3, 21)) return GirdlingRate(minimumWageRate);
			if (laborCode == (int)RanchLaborCode.RecoveryTime) return RecoveryTimeRate();
			if (laborCode == (int)RanchLaborCode.NonProductiveTime) return NonProductiveTimeRate();
			if (laborCode == (int)RanchLaborCode.QualityControl && shiftDate >= new DateTime(2020, 5, 11)) return QualityControlRate(shiftDate, employeeHourlyRate, minimumWageRate);
			if (crew == (int)Crew.WestTractor_Night) return WestTractor_NightRate(shiftDate, employeeHourlyRate, minimumWageRate);
			if (crew == (int)Crew.LightDuty_East) return CrewLaborRate(shiftDate, minimumWageRate);
			if (crew == (int)Crew.LightDuty_West) return CrewLaborRate(shiftDate, minimumWageRate);
			if (crew == (int)Crew.JoseLuisRodriguez && laborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew) return GraftingBuddingExpertCrewRate(shiftDate, minimumWageRate);
			if (crew > 100) return CrewLaborRate(shiftDate, minimumWageRate);
			return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
		}
		
		/// <summary>
		/// Returns true if the provided pay type would result in an hourly rate being assigned.
		/// </summary>
		/// <param name="payType"></param>
		/// <returns></returns>
		private bool IsAnAcceptablePayType(string payType)
		{
			if(
				payType == PayType.Regular 
				|| payType == PayType.HourlyPlusPieces 
				|| payType == PayType.Vacation 
				|| payType == PayType.Holiday
				|| payType == PayType.Bereavement
				|| payType == PayType.CompTime
				|| payType == PayType.ReportingPay
				|| payType == PayType.SpecialAdjustment
				|| payType == PayType.SickLeave
				|| payType == PayType.Covid19)
			{
				return true;
			}

			return false;
		}

		private decimal AlmondHarvestEquipmentOperatorDay(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			var crewLaborRate = CrewLaborRate(shiftDate, minimumWageRate);
			if (employeeHourlyRate > crewLaborRate)
			{
				return employeeHourlyRate;
			}
			else
			{
				return Math.Max(14.25M, minimumWageRate);
			}
		}

		private decimal AlmondHarvestEquipmentOperatorNight(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			var crewLaborRate = CrewLaborRate(shiftDate, minimumWageRate);
			if(employeeHourlyRate > crewLaborRate)
			{
				return employeeHourlyRate + 1M;
			}
			else
			{
				return Math.Max(15.25M, minimumWageRate);
			}
		}

		private decimal AlmondHarvestGeneral(DateTime shiftDate, int crew, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			var crewLaborRate = CrewLaborRate(shiftDate, minimumWageRate);
			if(crew == (int)Crew.AlmondHarvest_Nights)
			{
				if(employeeHourlyRate > crewLaborRate)
				{
					return employeeHourlyRate;
				}
				else
				{
					return Math.Max(14M, minimumWageRate);
				}
			}
			else
			{
				return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
			}
		}

		private decimal CrewHelper(DateTime shiftDate, decimal minimumWageRate)
		{
			// Before 12/7/2020 this is GrapeHarvestCrewHelper
			if(shiftDate < new DateTime(2020, 12, 7)) return Math.Max(12M, minimumWageRate);

			// On or after 12/7/2020 this is CrewHelper
			return CrewLaborRate(shiftDate, minimumWageRate);
		}

		private decimal CrewHelper_BonusRate(DateTime shiftDate, decimal minimumWageRate)
		{
			// Before 12/7/2020 this is GrapeHarvestCrewHelper_BonusRate
			if (shiftDate < new DateTime(2020, 12, 7)) return 0M;

			// On or after 12/7/2020 this is CrewHelper_BonusRate
			return CrewLaborRate(shiftDate, minimumWageRate);
		}

		private decimal GirdlingRate(decimal minimumWageRate) => Math.Max(15.50M, minimumWageRate);

		private decimal RecoveryTimeRate() => 0M;

		private decimal NonProductiveTimeRate() => 0M;

		private decimal QualityControlRate(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate) + .25M);
		}

		private decimal WestTractor_NightRate(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate) + .5M;
		}

		private decimal GraftingBuddingExpertCrewRate(DateTime shiftDate, decimal minimumWageRate)
		{
			if(shiftDate < new DateTime(2018, 2, 2))
			{
				return Math.Max(14M, minimumWageRate);
			}
			else
			{
				return Math.Max(15M, minimumWageRate);
			}
		}

		private decimal CulturalRate(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate));
		}

		private decimal CrewLaborRate(DateTime shiftDate, decimal minimumWageRate)
		{
			return Math.Max(_crewLaborWageService.GetWage(shiftDate), minimumWageRate);
		}
	}
}
