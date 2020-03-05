using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class TotalGrossCalculator
    {
        public void CalculateTotalGross(List<RanchPayLine> ranchPayLines)
        {
            //foreach(var payLine in ranchPayLines)
            //{
            //    payLine.TotalGross = payLine.GrossFromHours + payLine.GrossFromPieces + payLine.OtherGross;
            //}
        }

        public void CalculateTotalGross(List<PlantPayLine> plantPayLines)
        {
            //foreach (var payLine in plantPayLines)
            //{
            //    payLine.TotalGross = payLine.GrossFromHours + payLine.GrossFromPieces + payLine.OtherGross + payLine.GrossFromIncentive;
            //}
        }

        public void CalculateTotalGross(List<RanchAdjustmentLine> ranchAdjustmentLines)
        {
            //foreach (var adjustmentLine in ranchAdjustmentLines)
            //{
            //    adjustmentLine.TotalGross = adjustmentLine.GrossFromHours + adjustmentLine.GrossFromPieces + adjustmentLine.OtherGross;
            //}
        }
    }
}
