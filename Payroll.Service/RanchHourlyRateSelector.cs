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

		public RanchHourlyRateSelector(ICrewLaborWageService crewLaborWageService)
		{
			_crewLaborWageService = crewLaborWageService ?? throw new ArgumentNullException(nameof(crewLaborWageService));
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
		/// <returns></returns>
		public decimal GetHourlyRate(string payType, int crew, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, DateTime shiftDate)
		{
			if (!IsAnAcceptablePayType(payType)) return 0;
				
			if (hourlyRateOverride > 0) return hourlyRateOverride;
			if (payType == PayType.SickLeave) return 0;
			if (laborCode == (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay) return AlmondHarvestEquipmentOperatorDay(shiftDate, employeeHourlyRate);
			if (laborCode == (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight) return AlmondHarvestEquipmentOperatorNight(shiftDate, employeeHourlyRate);
			if (laborCode == (int)RanchLaborCode.AlmondHarvestGeneral) return AlmondHarvestGeneral(shiftDate, crew, employeeHourlyRate);
			if (laborCode == (int)RanchLaborCode.GrapeHarvestCrewHelper) return GrapeHarvestCrewHelper();
			if (laborCode == (int)RanchLaborCode.GrapeHarvestCrewHelper_BonusRate) return GrapeHarvestCrewHelper_BonusRate();
			if (laborCode == (int)RanchLaborCode.GrapeHarvestSupport) return CulturalRate(shiftDate, employeeHourlyRate);
			if (laborCode == (int)RanchLaborCode.Girdling && shiftDate >= new DateTime(2020, 3, 21)) return GirdlingRate();
			if (laborCode == (int)RanchLaborCode.RecoveryTime) return RecoveryTimeRate();
			if (laborCode == (int)RanchLaborCode.NonProductiveTime) return NonProductiveTimeRate();
			if (laborCode == (int)RanchLaborCode.QualityControl && shiftDate >= new DateTime(2020, 5, 11)) return QualityControlRate(shiftDate, employeeHourlyRate);
			if (crew == (int)Crew.WestTractor_Night) return WestTractor_NightRate(shiftDate, employeeHourlyRate);
			if (crew == (int)Crew.LightDuty_East) return CrewLaborRate(shiftDate);
			if (crew == (int)Crew.LightDuty_West) return CrewLaborRate(shiftDate);
			if (crew == (int)Crew.JoseLuisRodriguez && laborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew) return GraftingBuddingExpertCrewRate(shiftDate);
			if (crew > 100) return CrewLaborRate(shiftDate);
			return CulturalRate(shiftDate, employeeHourlyRate);
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
				|| payType == PayType.SpecialAdjustment)
			{
				return true;
			}

			return false;
		}

		private decimal AlmondHarvestEquipmentOperatorDay(DateTime shiftDate, decimal employeeHourlyRate)
		{
			if(employeeHourlyRate > CrewLaborRate(shiftDate))
			{
				return employeeHourlyRate;
			}
			else
			{
				return 14.25M;
			}
		}

		private decimal AlmondHarvestEquipmentOperatorNight(DateTime shiftDate, decimal employeeHourlyRate)
		{
			if(employeeHourlyRate > CrewLaborRate(shiftDate))
			{
				return employeeHourlyRate + 1M;
			}
			else
			{
				return 15.25M;
			}
		}

		private decimal AlmondHarvestGeneral(DateTime shiftDate, int crew, decimal employeeHourlyRate)
		{
			if(crew == (int)Crew.AlmondHarvest_Nights)
			{
				if(employeeHourlyRate > CrewLaborRate(shiftDate))
				{
					return employeeHourlyRate;
				}
				else
				{
					return 14M;
				}
			}
			else
			{
				return CulturalRate(shiftDate, employeeHourlyRate);
			}
		}

		private decimal GrapeHarvestCrewHelper() => 12M;

		private decimal GrapeHarvestCrewHelper_BonusRate() => 0M;

		private decimal GirdlingRate() => 15.50M;

		private decimal RecoveryTimeRate() => 0M;

		private decimal NonProductiveTimeRate() => 0M;

		private decimal QualityControlRate(DateTime shiftDate, decimal employeeHourlyRate)
		{
			return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate)+.25M);
		}

		private decimal WestTractor_NightRate(DateTime shiftDate, decimal employeeHourlyRate)
		{
			return CulturalRate(shiftDate, employeeHourlyRate) + .5M;
		}

		private decimal GraftingBuddingExpertCrewRate(DateTime shiftDate)
		{
			if(shiftDate < new DateTime(2018, 2, 2))
			{
				return 14M;
			}
			else
			{
				return 15M;
			}
		}

		private decimal CulturalRate(DateTime shiftDate, decimal employeeHourlyRate)
		{
			return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate));
		}

		private decimal CrewLaborRate(DateTime shiftDate)
		{
			return _crewLaborWageService.GetWage(shiftDate);
		}
	}
}
