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
		Thinning = 208,
		Girdling = 215,
		Grafting_BuddingExpertCrew = 217,
		SeasonalEquipmentOperator = 234,
		PieceRatePruningSummer = 261,
		PieceRatePruningWinter = 262,
		PieceRateColorPruning = 288,
		PieceRateHarvest_Bucket = 336,
		PieceRateThinning = 351,
		PieceRateHarvest_Tote = 372,
		RecoveryTime = 380,
		NonProductiveTime = 381,
		QualityControl = 551,
		Covid19 = 600,
	}
}
