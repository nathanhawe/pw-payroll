using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ICrewBossPayService
	{
		List<RanchPayLine> CalculateCrewBossPay(int batchId);		
	}
}
