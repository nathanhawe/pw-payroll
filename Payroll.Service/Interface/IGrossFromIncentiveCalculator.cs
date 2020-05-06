using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IGrossFromIncentiveCalculator
	{
		void CalculateGrossFromIncentive(List<PlantPayLine> plantPayLines);
	}
}
