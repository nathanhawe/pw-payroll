using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Performs incentive gross calculation on plant pay lines.
	/// </summary>
	public class GrossFromIncentiveCalculator : IGrossFromIncentiveCalculator
	{
		private readonly IRoundingService _roundingService;

		public GrossFromIncentiveCalculator(IRoundingService roundingService)
		{
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Sets the value of <c>GrossFromIncentive</c> for each of the <c>PlantPayLines</c> provided.
		/// </summary>
		/// <param name="plantPayLines"></param>
		public void CalculateGrossFromIncentive(List<PlantPayLine> plantPayLines)
		{
			// Plant pay lines can receive hourly incentives for labor code 555/558, non-discretionary bonus, or piece incentives.
			// 555/558 incentive cannot be applied at the same time as non-discretionary bonus and piece incentives.  But non-discretionary
			// bonus and piece incentives can technically be applied together.
			foreach (var payLine in plantPayLines)
			{
				var nonDiscretionaryBonus = 0M;
				if (
					payLine.LaborCode == (int)PlantLaborCode.TallyTagWriter 
					|| payLine.LaborCode == (int)PlantLaborCode.TagWriterLead)
				{
					HourlyIncentive(payLine);
				}
				else
				{
					nonDiscretionaryBonus = GetNonDiscretionaryBonus(payLine);
					PieceIncentive(payLine);
				}

				payLine.GrossFromIncentive = _roundingService.Round(payLine.GrossFromIncentive + nonDiscretionaryBonus, 2);
			}
		}

		/// <summary>
		/// Sets the value of <c>GrossFromIncentive</c> on the provided <c>PlantPayLine</c> based on hourly incentives.
		/// </summary>
		/// <param name="payLine"></param>
		private void HourlyIncentive(PlantPayLine payLine)
		{
			/* 
			 [555 Rate] = If([Incentive Disqualified]=true,0,2)
			 [558 Rate] = If([Incentive Disqualified]=true,0,1)
			*/
			var grossIncentive = 0M;

			if (!payLine.IsIncentiveDisqualified)
			{
				if(payLine.LaborCode == (int)PlantLaborCode.TagWriterLead)
				{
					grossIncentive = payLine.HoursWorked;
				}
				else
				{
					grossIncentive = payLine.HoursWorked * 2M;
				}
			}

			payLine.GrossFromIncentive = grossIncentive;
		}

		/// <summary>
		/// Sets the value of <c>GrossFromIncentive</c> on the provided <c>PlantPayLine</c> based on piece rate incentives.
		/// </summary>
		/// <param name="payLine"></param>
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

		/// <summary>
		/// Returns the Non-Discretionary Bonus for the passed <c>PlantPayLine</c> if the <c>NonDiscretionaryBonusRate</c>
		/// value has been set.
		/// </summary>
		/// <param name="payLine"></param>
		/// <returns></returns>
		private decimal GetNonDiscretionaryBonus(PlantPayLine payLine)
		{
			if (payLine.NonDiscretionaryBonusRate > 0)
			{
				return payLine.HoursWorked * payLine.NonDiscretionaryBonusRate;
			}
			else return 0M;
		}
	}
}
