using Payroll.Domain;
using Payroll.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
    public class GrossFromIncentiveCalculator
    {
		public void CalculateGrossFromIncentive(List<PlantPayLine> plantPayLines)
		{
			/*
				[Gross from Incentive] = If([Labor Code]=555,[555 Rate]*[Hours Worked], ([BonusPieceRate]*[Pieces]))
				
				[555 Rate] = 
					If([Incentive Disqualified]=true,0,
					[Plant]=3,2,
					[Plant]=2,2,
					[Plant]=4,2,0)
				[BonusPieceRate] = If(
					[NonPrimaViolation]="Yes",0,
					[Increased Rate]=true,[IncreasedRate]-[NonPrimaRate],[PrimaRate]-[NonPrimaRate])
	
				[NonPrimaViolation] is text data entry (expected to be "Yes" or "No"
				[Increased Rate] is a checkbox data entry
				[IncreasedRate] is currency data entry
				[NonPrimaRate] is currency data entry
				[PrimaRate] is currency data entry
				[Incentive Disqualified] is a checkbox data entry
			*/

			//foreach (var payLine in plantPayLines)
			//{
			//	if (payLine.LaborCode == 555)
			//	{
			//		HourlyIncentive(payLine);
			//	}
			//	else
			//	{
			//		PieceIncentive(payLine);
			//	}

			//	payLine.GrossFromIncentive = Math.Round(payLine.GrossFromIncentive, 2, MidpointRounding.ToPositiveInfinity);
			//}
		}

		private void HourlyIncentive(PlantPayLine payLine)
		{
			/* 
			 [555 Rate] = 
					If([Incentive Disqualified]=true,0,
					[Plant]=3,2,
					[Plant]=2,2,
					[Plant]=4,2,0)
			*/
			var grossIncentive = 0M;

			if (!payLine.IsIncentiveDisqualified)
			{
				var plant = Enum.IsDefined(typeof(Plant), payLine.Plant) ? (Plant)payLine.Plant : Plant.Unknown;
				switch (plant)
				{
					case Plant.Kerman:
					case Plant.Sanger:
					case Plant.Reedley:
						grossIncentive = payLine.HoursWorked * 2M;
						break;
					default:
						grossIncentive = 0M;
						break;
				}
			}

			payLine.GrossFromIncentive = grossIncentive;
		}

		private void PieceIncentive(PlantPayLine payLine)
		{
			/*
				[BonusPieceRate] = If(
					[NonPrimaViolation]="Yes",0,
					[Increased Rate]=true,[IncreasedRate]-[NonPrimaRate],[PrimaRate]-[NonPrimaRate])
	
				[NonPrimaViolation] is text data entry (expected to be "Yes" or "No"
				[Increased Rate] is a checkbox data entry
				[IncreasedRate] is currency data entry
				[NonPrimaRate] is currency data entry
				[PrimaRate] is currency data entry
			*/
			var bonusPieceRate = 0M;

			if (!payLine.HasNonPrimaViolation)
			{
				if (payLine.UseIncreasedRate)
				{
					bonusPieceRate = payLine.IncreasedRate - payLine.NonPrimaRate;
				}
				else
				{
					bonusPieceRate = payLine.PrimaRate - payLine.NonPrimaRate;
				}
			}

			payLine.GrossFromIncentive = payLine.Pieces * bonusPieceRate;
		}
	}
}
