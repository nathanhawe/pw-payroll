using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IGrossFromHoursCalculator
	{
		void CalculateGrossFromHours(List<RanchPayLine> ranchPayLines);
		void CalculateGrossFromHours(List<PlantPayLine> plantPayLines);
		void CalculateGrossFromHours(List<RanchAdjustmentLine> ranchAdjustmentLines);
		void CalculateGrossFromHours(List<PlantAdjustmentLine> plantAdjustmentLines);
	}
}
