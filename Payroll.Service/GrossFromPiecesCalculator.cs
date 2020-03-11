using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class GrossFromPiecesCalculator
    {
        public void CalculateGrossFromPieces(List<RanchPayLine> ranchPayLines)
        {
            //foreach(var payLine in ranchPayLines)
            //{
            //    payLine.GrossFromPieces = Math.Round(payLine.Pieces * payLine.PieceRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }

        public void CalculateGrossFromPieces(List<PlantPayLine> plantPayLines)
        {
            //foreach (var payLine in plantPayLines)
            //{
            //    payLine.GrossFromPieces = Math.Round(payLine.Pieces * payLine.NonPrimaRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }

        public void CalculateGrossFromPieces(List<RanchAdjustmentLine> ranchAdjustmentLines)
        {
            //foreach (var adjustmentLine in ranchAdjustmentLines)
            //{
            //    adjustmentLine.GrossFromPieces = Math.Round(adjustmentLine.Pieces * adjustmentLine.PieceRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }

        public void CalculateGrossFromPieces(List<PlantAdjustmentLine> plantAdjustmentLines)
        {
            //foreach (var adjustmentLine in plantAdjustmentLines)
            //{
            //    adjustmentLine.GrossFromPieces = Math.Round(adjustmentLine.Pieces * adjustmentLine.PieceRate, 2, MidpointRounding.ToPositiveInfinity);
            //}
        }
    }
}
