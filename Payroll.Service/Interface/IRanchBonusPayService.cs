using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IRanchBonusPayService
	{
		List<RanchPayLine> CalculateRanchBonusPayLines(int batchId);		
	}
}
