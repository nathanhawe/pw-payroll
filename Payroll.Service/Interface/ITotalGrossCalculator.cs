using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ITotalGrossCalculator
	{
		void CalculateTotalGross(List<RanchPayLine> ranchPayLines);
		void CalculateTotalGross(List<PlantPayLine> plantPayLines);
		void CalculateTotalGross(List<RanchAdjustmentLine> ranchAdjustmentLines);
		void CalculateTotalGross(List<PlantAdjustmentLine> plantAdjustmentLines);
	}
}
