using Payroll.Domain;
using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchBonusPayService
	{
		List<RanchPayLine> CalculateRanchBonusPayLines(int batchId, DateTime weekEndDate);
		void SetRegularLineGroupDesignation(List<RanchBonusPieceRate> bonusPieceRates, List<RanchPayLine> ranchPayLines);
	}
}
