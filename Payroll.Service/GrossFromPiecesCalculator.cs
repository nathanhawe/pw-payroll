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
    }
}
