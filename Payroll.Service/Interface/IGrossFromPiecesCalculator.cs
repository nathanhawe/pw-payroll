using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface IGrossFromPiecesCalculator
	{
		void CalculateGrossFromPieces(List<RanchPayLine> ranchPayLines);
		void CalculateGrossFromPieces(List<PlantPayLine> plantPayLines);
		void CalculateGrossFromPieces(List<RanchAdjustmentLine> ranchAdjustmentLines);
		void CalculateGrossFromPieces(List<PlantAdjustmentLine> plantAdjustmentLines);
	}
}
