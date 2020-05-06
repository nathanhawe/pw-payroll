using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;

namespace Payroll.Service
{
	/// <summary>
	/// Performs total gross calculations on pay and adjustment lines.
	/// </summary>
	public class TotalGrossCalculator : ITotalGrossCalculator
	{
		private readonly IRoundingService _roundingService;

		public TotalGrossCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Updates <c>TotalGross</c> for all of the provided <c>RanchPayLines</c>.
		/// TotalGross = GrossFromHours + GrossFromPieces + OtherGross
		/// </summary>
		/// <param name="ranchPayLines"></param>
		public void CalculateTotalGross(List<RanchPayLine> ranchPayLines)
		{
			foreach (var payLine in ranchPayLines)
			{
				payLine.TotalGross = _roundingService.Round(payLine.GrossFromHours + payLine.GrossFromPieces + payLine.OtherGross, 2);
			}
		}

		/// <summary>
		/// Updates <c>TotalGross</c> for all of the provided <c>PlantPayLines</c>.
		/// TotalGross = GrossFromHours + GrossFromPieces + OtherGross + GrossFromIncentive
		/// </summary>
		/// <param name="plantPayLines"></param>
		public void CalculateTotalGross(List<PlantPayLine> plantPayLines)
		{
			foreach (var payLine in plantPayLines)
			{
				payLine.TotalGross = _roundingService.Round(payLine.GrossFromHours + payLine.GrossFromPieces + payLine.OtherGross + payLine.GrossFromIncentive, 2);
			}
		}

		/// <summary>
		/// Updates <c>TotalGross</c> for all of the provided <c>RanchAdjustmentLines</c>.
		/// TotalGross = GrossFromHours + GrossFromPieces + OtherGross
		/// </summary>
		/// <param name="ranchAdjustmentLines"></param>
		public void CalculateTotalGross(List<RanchAdjustmentLine> ranchAdjustmentLines)
		{
			foreach (var adjustmentLine in ranchAdjustmentLines)
			{
				adjustmentLine.TotalGross = _roundingService.Round(adjustmentLine.GrossFromHours + adjustmentLine.GrossFromPieces + adjustmentLine.OtherGross, 2);
			}
		}

		/// <summary>
		/// Updates <c>TotalGross</c> for all of the provided <c>PlantAdjustmentLines</c>.
		/// TotalGross = GrossFromHours + GrossFromPieces + OtherGross + GrossFromIncentive
		/// </summary>
		/// <param name="plantAdjustmentLines"></param>
		public void CalculateTotalGross(List<PlantAdjustmentLine> plantAdjustmentLines)
		{
			foreach (var adjustmentLine in plantAdjustmentLines)
			{
				adjustmentLine.TotalGross = _roundingService.Round(adjustmentLine.GrossFromHours + adjustmentLine.GrossFromPieces + adjustmentLine.OtherGross + adjustmentLine.GrossFromIncentive, 2);
			}
		}
	}
}
