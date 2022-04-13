using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Service that handles the creation of production incentive bonus ranch payroll records for crew bosses
	/// based on the rates/effective dates in CrewBossBonusPayRates and total crew pieces.
	/// </summary>
	public class CrewBossBonusPayService : Interface.ICrewBossBonusPayService
	{
		private readonly PayrollContext _context;
		private readonly IRoundingService _roundingService;

		public CrewBossBonusPayService(
			PayrollContext context,
			IRoundingService roundingService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(context));
		}

		public List<RanchPayLine> CalculateCrewBossBonusPayLines(int batchId)
		{
			var results = new List<RanchPayLine>();

			// Get a list of effective crewboss bonus piece rates
			List<CrewBossBonusPieceRate> bonusPieceRates = GetCrewBossBonusPieceRatesForBatch(batchId);

			// Get a list of distinct crew boss and shift dates
			var cbDates = _context.CrewBossPayLines
				.Where(x => !x.IsDeleted && x.BatchId == batchId)
				.Select(s => new
				{
					s.EmployeeId,
					s.Crew,
					s.ShiftDate
				})
				.Distinct()
				.ToList();

			// For each labor code in bonus piece rates list, calculate the possible bonus for each crew boss
			List<int> bonusLaborCodes = bonusPieceRates.Select(s => s.LaborCode).Distinct().ToList();
			foreach(var laborCode in bonusLaborCodes)
			{
				// Group each crew that worked this labor code by crew, shift date, and Block ID
				var groups = _context.RanchPayLines
					.Where(x => !x.IsDeleted && x.BatchId == batchId && x.LaborCode == laborCode && x.PayType == PayType.Regular)
					.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
					{
						key.Crew,
						key.ShiftDate,
						key.BlockId,
						TotalPieces = group.Sum(x => x.Pieces)
					})
					.ToList();

				// Check each crew boss to see if it has matching crew groups
				foreach(var crewBoss in cbDates)
				{
					var crewGroups = groups.Where(x => x.TotalPieces > 0 && x.Crew == crewBoss.Crew && x.ShiftDate == crewBoss.ShiftDate).ToList();
					var rate = bonusPieceRates
						.Where(x =>
							!x.IsDeleted
							&& x.LaborCode == laborCode
							&& x.EffectiveDate <= crewBoss.ShiftDate)
						.OrderByDescending(o => o.EffectiveDate)
						.ThenByDescending(o => o.Id)
						.FirstOrDefault();

					// Short circuit loop if the rate doesn't exist or is 0.
					if (rate == null || rate.PerTreeBonus <= 0) continue;

					foreach(var group in crewGroups)
					{
						decimal gross = _roundingService.Round(group.TotalPieces * rate.PerTreeBonus, 2);
						if (gross > 0)
						{
							results.Add(new RanchPayLine
							{
								BatchId = batchId,
								EmployeeId = crewBoss.EmployeeId,
								ShiftDate = group.ShiftDate,
								BlockId = group.BlockId,
								Crew = crewBoss.Crew,
								LaborCode = laborCode,
								PayType = PayType.ProductionIncentiveBonus,
								Pieces = group.TotalPieces,
								PieceRate = rate.PerTreeBonus,
								OtherGross = gross,
								TotalGross = gross,
							});
						}
					}
				}
			}

			return results;
		}

		private List<CrewBossBonusPieceRate> GetCrewBossBonusPieceRatesForBatch(int batchId)
		{
			return _context.CrewBossBonusPieceRates.Where(x => !x.IsDeleted).ToList();
		}
	}
}
