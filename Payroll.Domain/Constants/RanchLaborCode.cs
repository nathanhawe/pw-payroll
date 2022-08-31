using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants
{
	public enum RanchLaborCode
	{
		Unknown = 0,
		AlmondHarvestEquipmentOperatorDay = 103,
		AlmondHarvestEquipmentOperatorNight = 104,
		AlmondHarvestGeneral = 105,
		CrewHelper = 116,
		CrewHelper_BonusRate = 117,
		GrapeHarvestSupport = 120,
		Pruning_Winter = 207,
		Thinning = 208,
		Pruning_Summer = 209,
		Girdling = 215,
		Grafting_BuddingExpertCrew = 217,
		SeasonalEquipmentOperator = 234,
		PieceRatePruningSummer = 261,
		PieceRatePruningWinter = 262,
		PieceRateColorPruning = 288,
		TractorPieceRateHarvest_Tote = 330,
		PieceRateHarvest_Bucket = 336,
		PieceRateThinning = 351,
		PieceRateHarvest_Tote = 372,
		TractorPieceRateHarvest_Bucket = 373,
		RecoveryTime = 380,
		NonProductiveTime = 381,
		QualityControl = 551,
		Covid19 = 600,
		Individual_PieceRateHarvest_Bucket = 9336,
		Individual_PieceRateHarvest_Tote = 9372,
		Individual_TractorPieceRateHarvest_Bucket = 9373,
		Individual_TractorPieceRateHarvest_Tote = 9330,
		Individual_LightDuty_PieceRateHarvest_Bucket = 9374,
		Individual_LightDuty_PieceRateHarvest_Tote = 9375
	}
}
