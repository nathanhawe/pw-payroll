using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Service
{
	/// <summary>
	/// Process ranch productivity incentive bonus pay lines
	/// </summary>
	public class RanchBonusPayService : Interface.IRanchBonusPayService
	{
		private readonly PayrollContext _context;
		private readonly IRoundingService _roundingService;

		public RanchBonusPayService(
			PayrollContext context, 
			IRoundingService roundingService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Calculates ranch productivity bonus pay lines returning a list of new, untracked <c>RanchPayLine</c> objects.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		public List<RanchPayLine> CalculateRanchBonusPayLines(int batchId)
		{
			var results = new List<RanchPayLine>();

			// Get a list of the bonus piece rates/thresholds
			List<RanchBonusPieceRate> bonusPieceRates = GetBonusPieceRatesForBatch(batchId);

			// For each distinct labor code in the bonus piece rates list, query ranch pay lines and calculate possible bonus.
			List<int> thresholdLaborCodes = bonusPieceRates.Select(s => s.LaborCode).Distinct().ToList();
			foreach(var laborCode in thresholdLaborCodes)
			{
				// Group records for this labor code by employee number, shift date, and block summing the hours and pieces.
				var groups = _context.RanchPayLines
					.Where(x => !x.IsDeleted && x.BatchId == batchId && x.LaborCode == laborCode && x.PayType == PayType.Regular)
					.GroupBy(g => new { g.EmployeeId, g.ShiftDate, g.BlockId }, (key, group) => new
					{
						key.EmployeeId,
						key.ShiftDate,
						key.BlockId,
						MaxCrew = group.Max(x => x.Crew),
						Hours = group.Sum(x => x.HoursWorked),
						Pieces = group.Sum(x => x.Pieces)
					})
					.ToList();

				// For each grouping record, lookup up the specific threshold and calculate the bonus.
				foreach(var empGroup in groups)
				{
					var rate = bonusPieceRates
						.Where(x =>
							!x.IsDeleted
							&& x.LaborCode == laborCode
							&& x.BlockId == empGroup.BlockId
							&& x.EffectiveDate <= empGroup.ShiftDate)
						.OrderByDescending(o => o.EffectiveDate)
						.ThenByDescending(o => o.QuickBaseRecordId)
						.FirstOrDefault();

					if(rate != null)
					{
						decimal threshold = _roundingService.Round((rate.PerHourThreshold * empGroup.Hours), 2);

						if (empGroup.Pieces > threshold)
						{
							// Create record
							decimal pieces = _roundingService.Round((empGroup.Pieces - threshold), 2);
							decimal gross = _roundingService.Round(pieces * rate.PerTreeBonus, 2);
							if(gross > 0)
							{
								results.Add(new RanchPayLine
								{
									BatchId = batchId,
									EmployeeId = empGroup.EmployeeId,
									ShiftDate = empGroup.ShiftDate,
									BlockId = empGroup.BlockId,
									Crew = empGroup.MaxCrew,
									LaborCode = laborCode,
									PayType = PayType.ProductionIncentiveBonus,
									Pieces = pieces,
									PieceRate = rate.PerTreeBonus,
									OtherGross = gross,
									TotalGross = gross,
								});
							}
						}
					}
				}
			}

			return results;
		}

		private List<RanchBonusPieceRate> GetBonusPieceRatesForBatch(int batchId)
		{
			return _context.RanchBonusPieceRates.Where(x => !x.IsDeleted && x.BatchId == batchId).ToList();
		}
	}
}
