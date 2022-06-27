using Payroll.Domain;
using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ICrewBossBonusPayService
	{
		List<RanchPayLine> CalculateCrewBossBonusPayLines(int batchId, DateTime weekEndDate);		
	}
}
