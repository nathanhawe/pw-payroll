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
		private readonly ICulturalLaborWageService _culturalLaborWageService;

		public RanchHourlyRateSelector(
			ICrewLaborWageService crewLaborWageService, 
			IMinimumWageService minimumWageService, 
			ICulturalLaborWageService culturalLaborWageService)
		{
			_crewLaborWageService = crewLaborWageService ?? throw new ArgumentNullException(nameof(crewLaborWageService));
			_minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(minimumWageService));
			_culturalLaborWageService = culturalLaborWageService ?? throw new ArgumentNullException(nameof(culturalLaborWageService));
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
			if (payType == PayType.PremiumPay) return 0M;
			if (payType == PayType.SickLeave) return payLineHourlyRate;
			if (
				payType == PayType.Covid19
				|| payType == PayType.Covid19WageContinuation
				|| payType == PayType.Covid19W)
			{
				return CovidRate(shiftDate, employeeHourlyRate, minimumWageRate, crew, payLineHourlyRate);
			}
			if (laborCode == (int)RanchLaborCode.AlmondHarvestEquipmentOperatorDay) return AlmondHarvestEquipmentOperatorDay(shiftDate, employeeHourlyRate, minimumWageRate, crew);
			if (laborCode == (int)RanchLaborCode.AlmondHarvestEquipmentOperatorNight) return AlmondHarvestEquipmentOperatorNight(shiftDate, employeeHourlyRate, minimumWageRate, crew);
			if (laborCode == (int)RanchLaborCode.AlmondHarvestGeneral) return AlmondHarvestGeneral(shiftDate, crew, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.CrewHelper) return CrewHelper(shiftDate, crew, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.CrewHelper_BonusRate) return CrewHelper_BonusRate(shiftDate, crew, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.GrapeHarvestSupport) return GrapeHarvestSupport(shiftDate, crew, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.Girdling && shiftDate >= new DateTime(2020, 3, 21)) return GirdlingRate(shiftDate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew) return GraftingBuddingExpertCrewRate(shiftDate, crew, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.RecoveryTime) return RecoveryTimeRate();
			if (laborCode == (int)RanchLaborCode.NonProductiveTime) return NonProductiveTimeRate();
			if (laborCode == (int)RanchLaborCode.QualityControl && shiftDate >= new DateTime(2020, 5, 11)) return QualityControlRate(shiftDate, crew, employeeHourlyRate, minimumWageRate);
			if (laborCode == (int)RanchLaborCode.SeasonalEquipmentOperator && shiftDate >= new DateTime(2022, 3, 7)) return SeasonalEquipmentOperator(shiftDate, employeeHourlyRate, minimumWageRate);
			if (crew == (int)Crew.WestTractor_Night) return WestTractor_NightRate(shiftDate, employeeHourlyRate, minimumWageRate);
			//if (crew == (int)Crew.LightDuty_East) return CrewLaborRate(shiftDate, minimumWageRate);
			//if (crew == (int)Crew.LightDuty_West) return CrewLaborRate(shiftDate, minimumWageRate);

			if (IsCulturalCrew(crew))
			{
				if (shiftDate < new DateTime(2021, 6, 7)) return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate));
				return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
			}
			
			return CrewLaborRate(shiftDate, minimumWageRate);
			
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
				|| payType == PayType.Covid19
				|| payType == PayType.Covid19WageContinuation
				|| payType == PayType.Covid19W
				|| payType == PayType.PremiumPay)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the rate used for COVID pay types.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="employeeHourlyRate"></param>
		/// <param name="minimumWageRate"></param>
		/// <param name="crew"></param>
		/// <param name="payLineHourlyRate"></param>
		/// <returns></returns>
		private decimal CovidRate(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate, int crew, decimal payLineHourlyRate)
		{
			if (IsCulturalCrew(crew))
			{
				return Math.Max(payLineHourlyRate, CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate));
			}
			else
			{
				return Math.Max(payLineHourlyRate, Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate)));
			}
		}

		private decimal AlmondHarvestEquipmentOperatorDay(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate, int crew)
		{
			if (shiftDate < new DateTime(2021, 6, 7))
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
			else
			{
				if (IsCulturalCrew(crew))
				{
					return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
				}
				else return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		private decimal AlmondHarvestEquipmentOperatorNight(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate, int crew)
		{
			if (shiftDate < new DateTime(2021, 6, 7))
			{
				var crewLaborRate = CrewLaborRate(shiftDate, minimumWageRate);
				if (employeeHourlyRate > crewLaborRate)
				{
					return employeeHourlyRate + 1M;
				}
				else
				{
					return Math.Max(15.25M, minimumWageRate);
				}
			}
			else
			{
				if (IsCulturalCrew(crew)) return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		private decimal AlmondHarvestGeneral(DateTime shiftDate, int crew, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			if (shiftDate < new DateTime(2021, 6, 7))
			{
				var crewLaborRate = CrewLaborRate(shiftDate, minimumWageRate);
				if (crew == (int)Crew.AlmondHarvest_Nights)
				{
					if (employeeHourlyRate > crewLaborRate)
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
					return Math.Max(employeeHourlyRate, crewLaborRate);
				}
			}
			else
			{
				if (IsCulturalCrew(crew)) return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		private decimal CrewHelper(DateTime shiftDate, int crew, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			// Before 12/7/2020 this is GrapeHarvestCrewHelper
			if (shiftDate < new DateTime(2020, 12, 7))
			{
				return Math.Max(12M, minimumWageRate);
			}

			// On or after 12/7/2020 this is CrewHelper
			else if (shiftDate < new DateTime(2021, 6, 7))
			{
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
			else
			{
				if (IsCulturalCrew(crew)) return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		private decimal CrewHelper_BonusRate(DateTime shiftDate, int crew, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			// Before 12/7/2020 this is GrapeHarvestCrewHelper_BonusRate
			if (shiftDate < new DateTime(2020, 12, 7))
			{
				return 0M;
			}
			else if (shiftDate < new DateTime(2021, 6, 7))
			{
				// On or after 12/7/2020 this is CrewHelper_BonusRate
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
			else
			{
				if (IsCulturalCrew(crew)) return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		private decimal GrapeHarvestSupport(DateTime shiftDate, int crew, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			if (shiftDate < new DateTime(2021, 6, 7))
			{
				return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate));
			}
			else
			{
				if (IsCulturalCrew(crew)) return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate);
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		private decimal GirdlingRate(DateTime shiftDate, decimal minimumWageRate) 
		{
			if(shiftDate < new DateTime(2021, 2, 8))
			{
				return Math.Max(15.50M, minimumWageRate);
			}
			else
			{
				return CrewLaborRate(shiftDate, minimumWageRate) + 1;
			}
		}

		private decimal RecoveryTimeRate() => 0M;

		private decimal NonProductiveTimeRate() => 0M;

		private decimal QualityControlRate(DateTime shiftDate, int crew, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			if(shiftDate < new DateTime(2021, 6, 7))
			{
				return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate) + .25M);
			}
			else
			{
				if (IsCulturalCrew(crew)) return Math.Max(employeeHourlyRate, Math.Max(_culturalLaborWageService.GetWage(shiftDate), minimumWageRate) + .25M);
				else return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate) + .25M);
			}
			
		}

		private decimal SeasonalEquipmentOperator(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate) + .5M;
		}

		private decimal WestTractor_NightRate(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			if(shiftDate < new DateTime(2021, 6, 7))
			{
				return Math.Max(employeeHourlyRate, CrewLaborRate(shiftDate, minimumWageRate)) + .5M;
			}
			else
			{
				return CulturalRate(shiftDate, employeeHourlyRate, minimumWageRate) + .5M;
			}
		}

		private decimal GraftingBuddingExpertCrewRate(DateTime shiftDate, int crew, decimal minimumWageRate)
		{
			if(shiftDate < new DateTime(2018, 2, 2) && crew == (int)Crew.JoseLuisRodriguez)
			{
				return Math.Max(14M, minimumWageRate);
			}
			else if(shiftDate < new DateTime(2021, 1, 11) && crew == (int)Crew.JoseLuisRodriguez)
			{
				return Math.Max(15M, minimumWageRate);
			}
			else if(shiftDate >= new DateTime(2021, 1, 11))
			{
				return CrewLaborRate(shiftDate, minimumWageRate) + 1;
			}
			else
			{
				return CrewLaborRate(shiftDate, minimumWageRate);
			}
		}

		/// <summary>
		/// Returns the greater of the employee's hourly rate, the effective minimum wage, and the effective cultural labor rate.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="employeeHourlyRate"></param>
		/// <param name="minimumWageRate"></param>
		/// <returns></returns>
		private decimal CulturalRate(DateTime shiftDate, decimal employeeHourlyRate, decimal minimumWageRate)
		{
			return Math.Max(employeeHourlyRate, Math.Max(_culturalLaborWageService.GetWage(shiftDate), minimumWageRate));
		}

		/// <summary>
		/// Returns the greater of the effective minimum wage and the effective crew labor rate.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="minimumWageRate"></param>
		/// <returns></returns>
		private decimal CrewLaborRate(DateTime shiftDate, decimal minimumWageRate)
		{
			return Math.Max(_crewLaborWageService.GetWage(shiftDate), minimumWageRate);
		}

		/// <summary>
		/// Returns true of the passed crew number is designated for cultural labor.
		/// </summary>
		/// <param name="crew"></param>
		/// <returns></returns>
		private bool IsCulturalCrew(int crew)
		{
			return (crew < 100 && crew != 75 && crew != 76);
		}
	}
}
