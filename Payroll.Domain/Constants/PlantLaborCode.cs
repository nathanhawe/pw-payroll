using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants
{
	public enum PlantLaborCode
	{
		Unknown = 1,
		PackerNoPieces = 115,
		Packing = 123,
		Palletizing = 125,
		FreshCut = 151,
		ReceivingAndMarkingGrapes = 312,
		RecoveryTime = 380,
		NonProductiveTime = 381,
		ReceivingFreshFruit = 503,
		NightSanitation = 535,
		NightShiftSupervision = 536,
		NightShiftAuditor = 537,
		TallyTagWriter = 555,
		TagWriterLead = 558,
		Covid19 = 600,
		Covid19PreScreening = 602,
		Covid19Sanitation = 603,
		Receiving_Break = 9503
	}
}
