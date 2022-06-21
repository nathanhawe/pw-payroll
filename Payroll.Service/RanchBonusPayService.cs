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
			var results = CalculateIndividualBonuses(batchId);
			results.AddRange(CalculateGroupHarvestBonuses(batchId));
			results.AddRange(CalculateIndividualHarvestBonuses(batchId));
			
			return results;
		}

		/// <summary>
		/// Calculates productivity bonuses based on individual performance using the <c>RanchBonusPieceRate</c> records.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateIndividualBonuses(int batchId)
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

		#region - Harvest Group Bonus
		/// <summary>
		/// Calculates individual bonuses based on crew performance.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateGroupHarvestBonuses(int batchId)
		{
			var results = new List<RanchPayLine>();
			
			// TODO: Make this data driven.
			results.AddRange(CreateGroupHarvestBucketBonuses(batchId));
			results.AddRange(CreateGroupHarvestToteBonuses(batchId));

			return results;
		}

		private List<RanchPayLine> CreateGroupHarvestBucketBonuses(int batchId)
		{
			var results = new List<RanchPayLine>();
			var rate = .1M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x => !x.IsDeleted && x.BatchId == batchId && x.PayType == PayType.Regular && (x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Bucket || x.LaborCode == (int)RanchLaborCode.TractorPieceRateHarvest_Bucket))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					Hours = group.Sum(x => x.HoursWorked),
					Pieces = group.Sum(x => x.Pieces)
				})
				.ToList();

			// Group the employee groupings into crews to be used for proportional distribution of group bonus.
			var crewGroups = employeeGroups
				.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.ShiftDate,
					key.BlockId,
					TotalHours = group.Sum(x => x.Hours),
					TotalPieces = group.Sum(x => x.Pieces)
				})
				.ToList();

			// For each crew group, lookup up the specific bonus and distribute it proportionally across all participating employees.
			foreach (var crew in crewGroups)
			{
				var employees = employeeGroups.Where(x => x.Crew == crew.Crew && x.ShiftDate == crew.ShiftDate && x.BlockId == crew.BlockId && x.Hours > 0).ToList();
				foreach (var employee in employees)
				{
					decimal percentage = _roundingService.Round((employee.Hours / crew.TotalHours), 4);
					decimal pieces = _roundingService.Round(percentage * crew.TotalPieces, 2);
					decimal gross = _roundingService.Round(pieces * rate, 2);

					if (gross > 0)
					{
						results.Add(new RanchPayLine
						{
							BatchId = batchId,
							EmployeeId = employee.EmployeeId,
							ShiftDate = employee.ShiftDate,
							BlockId = employee.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = rate,
							OtherGross = gross,
							TotalGross = gross,
						});
					}
				}
			}

			return results;
		}

		private List<RanchPayLine> CreateGroupHarvestToteBonuses(int batchId)
		{
			var results = new List<RanchPayLine>();
			var rate = .12M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x => !x.IsDeleted && x.BatchId == batchId && x.PayType == PayType.Regular && (x.LaborCode == (int)RanchLaborCode.PieceRateHarvest_Tote || x.LaborCode == (int)RanchLaborCode.TractorPieceRateHarvest_Tote))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					Hours = group.Sum(x => x.HoursWorked),
					Pieces = group.Sum(x => x.Pieces)
				})
				.ToList();

			// Group the employee groupings into crews to be used for proportional distribution of group bonus.
			var crewGroups = employeeGroups
				.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.ShiftDate,
					key.BlockId,
					TotalHours = group.Sum(x => x.Hours),
					TotalPieces = group.Sum(x => x.Pieces)
				})
				.ToList();

			// For each crew group, lookup up the specific bonus and distribute it proportionally across all participating employees.
			foreach (var crew in crewGroups)
			{
				var employees = employeeGroups.Where(x => x.Crew == crew.Crew && x.ShiftDate == crew.ShiftDate && x.BlockId == crew.BlockId && x.Hours > 0).ToList();
				foreach (var employee in employees)
				{
					decimal percentage = _roundingService.Round((employee.Hours / crew.TotalHours), 4);
					decimal pieces = _roundingService.Round(percentage * crew.TotalPieces, 2);
					decimal gross = _roundingService.Round(pieces * rate, 2);

					if (gross > 0)
					{
						results.Add(new RanchPayLine
						{
							BatchId = batchId,
							EmployeeId = employee.EmployeeId,
							ShiftDate = employee.ShiftDate,
							BlockId = employee.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.PieceRateHarvest_Tote,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = rate,
							OtherGross = gross,
							TotalGross = gross,
						});
					}
				}
			}

			return results;
		}
#endregion

		#region - Harvest Individual Bonus
		/// <summary>
		/// Calculates individual bonuses based on individual performance.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateIndividualHarvestBonuses(int batchId)
		{
			var results = new List<RanchPayLine>();

			// TODO: Make this data driven.
			results.AddRange(CreateIndividualHarvestBucketBonuses(batchId));
			results.AddRange(CreateIndividualHarvestToteBonuses(batchId));

			return results;
		}

		private List<RanchPayLine> CreateIndividualHarvestBucketBonuses(int batchId)
		{
			var results = new List<RanchPayLine>();
			var rate = .1M;
			var tractorDriverRate = .005M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x => !x.IsDeleted && x.BatchId == batchId && x.PayType == PayType.Regular && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket)
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					Pieces = group.Sum(x => x.Pieces)
				})
				.ToList();

			// Group the employee groupings into crews to be used for tractor driver bonuses
			var crewGroups = employeeGroups
				.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.ShiftDate,
					key.BlockId,
					TotalPieces = group.Sum(x => x.Pieces)
				})
				.ToList();


			// Calculate individual bonus for each employee.
			foreach (var employee in employeeGroups)
			{
				decimal gross = _roundingService.Round(employee.Pieces * rate, 2);
				if(gross > 0)
				{
					results.Add(new RanchPayLine
					{
						BatchId = batchId,
						EmployeeId = employee.EmployeeId,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						OtherGross = gross,
						TotalGross = gross,
					});
				}
			}


			// For each crew group, look up tractor drivers and calculate tractor driver bonus.
			foreach (var crew in crewGroups)
			{
				var employees = _context.RanchPayLines
					.Where(x => !x.IsDeleted && x.BatchId == batchId && x.Crew == crew.Crew && x.ShiftDate == crew.ShiftDate && x.BlockId == crew.BlockId && x.PayType == PayType.Regular && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket)
					.Select(s => new { s.EmployeeId })
					.Distinct()
					.ToList();

				foreach (var employee in employees)
				{
					decimal pieces = crew.TotalPieces;
					decimal gross = _roundingService.Round(pieces * tractorDriverRate, 2);

					if (gross > 0)
					{
						results.Add(new RanchPayLine
						{
							BatchId = batchId,
							EmployeeId = employee.EmployeeId,
							ShiftDate = crew.ShiftDate,
							BlockId = crew.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = tractorDriverRate,
							OtherGross = gross,
							TotalGross = gross,
						});
					}
				}
			}

			return results;
		}

		private List<RanchPayLine> CreateIndividualHarvestToteBonuses(int batchId)
		{
			var results = new List<RanchPayLine>();
			var rate = .12M;
			var tractorDriverRate = .005M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x => !x.IsDeleted && x.BatchId == batchId && x.PayType == PayType.Regular && x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote)
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					Pieces = group.Sum(x => x.Pieces)
				})
				.ToList();

			// Group the employee groupings into crews to be used for tractor driver bonuses
			var crewGroups = employeeGroups
				.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.ShiftDate,
					key.BlockId,
					TotalPieces = group.Sum(x => x.Pieces)
				})
				.ToList();


			// Calculate individual bonus for each employee.
			foreach (var employee in employeeGroups)
			{
				decimal gross = _roundingService.Round(employee.Pieces * rate, 2);
				if (gross > 0)
				{
					results.Add(new RanchPayLine
					{
						BatchId = batchId,
						EmployeeId = employee.EmployeeId,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = (int)RanchLaborCode.Individual_PieceRateHarvest_Tote,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						OtherGross = gross,
						TotalGross = gross,
					});
				}
			}


			// For each crew group, look up tractor drivers and calculate tractor driver bonus.
			foreach (var crew in crewGroups)
			{
				var employees = _context.RanchPayLines
					.Where(x => !x.IsDeleted && x.BatchId == batchId && x.Crew == crew.Crew && x.ShiftDate == crew.ShiftDate && x.BlockId == crew.BlockId && x.PayType == PayType.Regular && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote)
					.Select(s => new {s.EmployeeId})
					.Distinct()
					.ToList();

				foreach (var employee in employees)
				{
					decimal pieces = crew.TotalPieces;
					decimal gross = _roundingService.Round(pieces * tractorDriverRate, 2);

					if (gross > 0)
					{
						results.Add(new RanchPayLine
						{
							BatchId = batchId,
							EmployeeId = employee.EmployeeId,
							ShiftDate = crew.ShiftDate,
							BlockId = crew.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = tractorDriverRate,
							OtherGross = gross,
							TotalGross = gross,
						});
					}
				}
			}

			return results;
		}
		#endregion

		private List<RanchBonusPieceRate> GetBonusPieceRatesForBatch(int batchId)
		{
			return _context.RanchBonusPieceRates.Where(x => !x.IsDeleted && x.BatchId == batchId).ToList();
		}
		
	}
}
