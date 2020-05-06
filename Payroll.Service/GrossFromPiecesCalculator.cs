using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Perform pieces gross calculation for pay and adjustment lines.
	/// </summary>
	public class GrossFromPiecesCalculator : IGrossFromPiecesCalculator
	{
		private readonly IRoundingService _roundingService;

		public GrossFromPiecesCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Sets the value of the <c>GrossFromPieces</c> property for each <c>RanchPayLine</c> provided.
		/// </summary>
		/// <param name="ranchPayLines"></param>
		public void CalculateGrossFromPieces(List<RanchPayLine> ranchPayLines)
		{
			foreach (var payLine in ranchPayLines)
			{
				payLine.GrossFromPieces = _roundingService.Round(payLine.Pieces * payLine.PieceRate, 2);
			}
		}

		/// <summary>
		/// Sets the value of the <c>GrossFromPieces</c> property for each <c>PlantPayLine</c> provided.
		/// </summary>
		/// <param name="plantPayLines"></param>
		public void CalculateGrossFromPieces(List<PlantPayLine> plantPayLines)
		{
			foreach (var payLine in plantPayLines)
			{
				payLine.GrossFromPieces = _roundingService.Round(payLine.Pieces * payLine.NonPrimaRate, 2);
			}
		}

		/// <summary>
		/// Sets the value of the <c>GrossFromPieces</c> property for each <c>RanchAdjustmentLine</c> provided.
		/// </summary>
		/// <param name="ranchAdjustmentLines"></param>
		public void CalculateGrossFromPieces(List<RanchAdjustmentLine> ranchAdjustmentLines)
		{
			foreach (var adjustmentLine in ranchAdjustmentLines)
			{
				adjustmentLine.GrossFromPieces = _roundingService.Round(adjustmentLine.Pieces * adjustmentLine.PieceRate, 2);
			}
		}

		/// <summary>
		/// Sets the value of the <c>GrossFromPieces</c> property for each <c>PlantAdjustmentLine</c> provided.
		/// </summary>
		/// <param name="plantAdjustmentLines"></param>
		public void CalculateGrossFromPieces(List<PlantAdjustmentLine> plantAdjustmentLines)
		{
			foreach (var adjustmentLine in plantAdjustmentLines)
			{
				adjustmentLine.GrossFromPieces = _roundingService.Round(adjustmentLine.Pieces * adjustmentLine.PieceRate, 2);
			}
		}
	}
}
