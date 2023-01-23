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
		public List<RanchPayLine> CalculateRanchBonusPayLines(int batchId, DateTime weekEndDate)
		{
			var results = CalculateVariableBonuses(batchId, weekEndDate);
			results.AddRange(CalculateGroupHarvestBonuses(batchId, weekEndDate));
			results.AddRange(CalculateIndividualHarvestBonuses(batchId, weekEndDate));
			results.AddRange(CalculateSummerPruningBonuses(batchId, weekEndDate));
			results.AddRange(CalculateGraftingBonuses(batchId, weekEndDate));
			results.AddRange(CalculateReplantingBonuses(batchId, weekEndDate));
	
			// Only calculate the non-variable winter pruning bonuses before 12/12/2022
			if(weekEndDate <= new DateTime(2022, 12, 11))
				results.AddRange(CalculateWinterPruningBonuses(batchId, weekEndDate));

			return results;
		}

		/// <summary>
		/// Calculates productivity bonuses based on individual performance using the <c>RanchBonusPieceRate</c> records.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateVariableBonuses(int batchId, DateTime weekEndDate)
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
									WeekEndDate = weekEndDate,
									ShiftDate = empGroup.ShiftDate,
									BlockId = empGroup.BlockId,
									Crew = empGroup.MaxCrew,
									LaborCode = laborCode,
									PayType = PayType.ProductionIncentiveBonus,
									Pieces = pieces,
									PieceRate = rate.PerTreeBonus,
									GrossFromPieces = gross,
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
		private List<RanchPayLine> CalculateGroupHarvestBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			
			// TODO: Make this data driven.
			results.AddRange(CreateGroupHarvestBucketBonuses(batchId, weekEndDate));
			results.AddRange(CreateGroupHarvestToteBonuses(batchId, weekEndDate));

			return results;
		}

		private List<RanchPayLine> CreateGroupHarvestBucketBonuses(int batchId, DateTime weekEndDate)
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
							WeekEndDate = weekEndDate,
							ShiftDate = employee.ShiftDate,
							BlockId = employee.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.PieceRateHarvest_Bucket,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = rate,
							GrossFromPieces = gross,
							TotalGross = gross,
						});
					}
				}
			}

			return results;
		}

		private List<RanchPayLine> CreateGroupHarvestToteBonuses(int batchId, DateTime weekEndDate)
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
							WeekEndDate = weekEndDate,
							ShiftDate = employee.ShiftDate,
							BlockId = employee.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.PieceRateHarvest_Tote,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = rate,
							GrossFromPieces = gross,
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
		private List<RanchPayLine> CalculateIndividualHarvestBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();

			// TODO: Make this data driven.
			results.AddRange(CreateIndividualHarvestBucketBonuses(batchId, weekEndDate));
			results.AddRange(CreateIndividualHarvestToteBonuses(batchId, weekEndDate));

			return results;
		}

		private List<RanchPayLine> CreateIndividualHarvestBucketBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			var rate = .1M;
			var tractorDriverRate = .005M;
			var tractorDriverRateSouth = .01M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x => 
					!x.IsDeleted 
					&& x.BatchId == batchId 
					&& x.PayType == PayType.Regular 
					&& (
						x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Bucket 
						|| x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Bucket
						|| x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId, g.LaborCode }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					key.LaborCode,
					Pieces = group.Sum(x => x.LaborCode != (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket ? x.Pieces : 0),
					TDPieces = group.Sum(x => x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket ? x.Pieces : 0)
				})
				.ToList();

			// Group the employee groupings into crews to be used for tractor driver bonuses
			var crewGroups = employeeGroups
				.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.ShiftDate,
					key.BlockId,
					TotalPieces = group.Sum(x => x.Pieces + x.TDPieces)
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
						WeekEndDate = weekEndDate,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = employee.LaborCode,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						GrossFromPieces = gross,
						TotalGross = gross,
					});
				}
			}


			// For each crew group, look up tractor drivers and calculate tractor driver bonus.
			foreach (var crew in crewGroups)
			{
				var employees = _context.RanchPayLines
					.Where(x => !x.IsDeleted && x.BatchId == batchId && x.Crew == crew.Crew && x.ShiftDate == crew.ShiftDate && x.BlockId == crew.BlockId && x.PayType == PayType.Regular && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket)
					.Select(s => new { s.EmployeeId, s.CrewLocation })
					.Distinct()
					.ToList();

				foreach (var employee in employees)
				{
					decimal pieces = crew.TotalPieces;
					var pieceRate = employee.CrewLocation == (int)Location.South ? tractorDriverRateSouth : tractorDriverRate;
					decimal gross = _roundingService.Round(pieces * pieceRate, 2);

					if (gross > 0)
					{
						results.Add(new RanchPayLine
						{
							BatchId = batchId,
							EmployeeId = employee.EmployeeId,
							WeekEndDate = weekEndDate,
							ShiftDate = crew.ShiftDate,
							BlockId = crew.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Bucket,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = pieceRate,
							GrossFromPieces = gross,
							TotalGross = gross,
							CrewLocation = employee.CrewLocation,
						});
					}
				}
			}

			return results;
		}

		private List<RanchPayLine> CreateIndividualHarvestToteBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			var rate = .12M;
			var tractorDriverRate = .005M;
			var tractorDriverRateSouth = .01M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x => 
					!x.IsDeleted 
					&& x.BatchId == batchId 
					&& x.PayType == PayType.Regular 
					&& (
						x.LaborCode == (int)RanchLaborCode.Individual_PieceRateHarvest_Tote
						|| x.LaborCode == (int)RanchLaborCode.Individual_LightDuty_PieceRateHarvest_Tote
						|| x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId, g.LaborCode }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					key.LaborCode,
					Pieces = group.Sum(x => x.LaborCode != (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote ? x.Pieces : 0),
					TDPieces = group.Sum(x => x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote ? x.Pieces : 0)
				})
				.ToList();

			// Group the employee groupings into crews to be used for tractor driver bonuses
			var crewGroups = employeeGroups
				.GroupBy(g => new { g.Crew, g.ShiftDate, g.BlockId }, (key, group) => new
				{
					key.Crew,
					key.ShiftDate,
					key.BlockId,
					TotalPieces = group.Sum(x => x.Pieces + x.TDPieces)
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
						WeekEndDate = weekEndDate,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = employee.LaborCode,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						GrossFromPieces = gross,
						TotalGross = gross,
					});
				}
			}


			// For each crew group, look up tractor drivers and calculate tractor driver bonus.
			foreach (var crew in crewGroups)
			{
				var employees = _context.RanchPayLines
					.Where(x => !x.IsDeleted && x.BatchId == batchId && x.Crew == crew.Crew && x.ShiftDate == crew.ShiftDate && x.BlockId == crew.BlockId && x.PayType == PayType.Regular && x.LaborCode == (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote)
					.Select(s => new {s.EmployeeId, s.CrewLocation})
					.Distinct()
					.ToList();

				foreach (var employee in employees)
				{
					decimal pieces = crew.TotalPieces;
					var pieceRate = employee.CrewLocation == (int)Location.South ? tractorDriverRateSouth : tractorDriverRate;
					decimal gross = _roundingService.Round(pieces * pieceRate, 2);

					if (gross > 0)
					{
						results.Add(new RanchPayLine
						{
							BatchId = batchId,
							EmployeeId = employee.EmployeeId,
							WeekEndDate = weekEndDate,
							ShiftDate = crew.ShiftDate,
							BlockId = crew.BlockId,
							Crew = crew.Crew,
							LaborCode = (int)RanchLaborCode.Individual_TractorPieceRateHarvest_Tote,
							PayType = PayType.ProductionIncentiveBonus,
							Pieces = pieces,
							PieceRate = pieceRate,
							GrossFromPieces = gross,
							TotalGross = gross,
							CrewLocation = employee.CrewLocation,
						});
					}
				}
			}

			return results;
		}
		#endregion

		#region - Pruning Bonus

		/// <summary>
		/// Calculates summer pruning bonuses based on individual performance.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="weekEndDate"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateSummerPruningBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			var rate = .10M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x =>
					!x.IsDeleted
					&& x.BatchId == batchId
					&& x.PayType == PayType.Regular
					&& (
						x.LaborCode == (int)RanchLaborCode.PieceRatePruningSummer
						))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId, g.LaborCode }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					key.LaborCode,
					Pieces = group.Sum(x => x.Pieces)					
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
						WeekEndDate = weekEndDate,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = employee.LaborCode,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						GrossFromPieces = gross,
						TotalGross = gross,
					});
				}
			}

			return results;
		}

		/// <summary>
		/// Calculates winter pruning bonuses based on individual performance.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="weekEndDate"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateWinterPruningBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			var rate = .25M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x =>
					!x.IsDeleted
					&& x.BatchId == batchId
					&& x.PayType == PayType.Regular
					&& (
						x.LaborCode == (int)RanchLaborCode.PieceRatePruningWinter
						))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId, g.LaborCode }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					key.LaborCode,
					Pieces = group.Sum(x => x.Pieces)
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
						WeekEndDate = weekEndDate,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = employee.LaborCode,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						GrossFromPieces = gross,
						TotalGross = gross,
					});
				}
			}

			return results;
		}

		#endregion

		/// <summary>
		/// Calculate grafting piece bonuses
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="weekEndDate"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateGraftingBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			var rate = .05M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x =>
					!x.IsDeleted
					&& x.BatchId == batchId
					&& x.PayType == PayType.Regular
					&& (
						x.LaborCode == (int)RanchLaborCode.Grafting_BuddingExpertCrew
						))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId, g.LaborCode }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					key.LaborCode,
					Pieces = group.Sum(x => x.Pieces)
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
						WeekEndDate = weekEndDate,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = employee.LaborCode,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						GrossFromPieces = gross,
						TotalGross = gross,
					});
				}
			}

			return results;
		}

		/// <summary>
		/// Calculate replanting piece bonuses
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="weekEndDate"></param>
		/// <returns></returns>
		private List<RanchPayLine> CalculateReplantingBonuses(int batchId, DateTime weekEndDate)
		{
			var results = new List<RanchPayLine>();
			var rate = .1M;

			// Group records for this labor code by crew, employee number, shift date, and block summing the hours and pieces.
			var employeeGroups = _context.RanchPayLines
				.Where(x =>
					!x.IsDeleted
					&& x.BatchId == batchId
					&& x.PayType == PayType.Regular
					&& (
						x.LaborCode == (int)RanchLaborCode.Replanting
						))
				.GroupBy(g => new { g.Crew, g.EmployeeId, g.ShiftDate, g.BlockId, g.LaborCode }, (key, group) => new
				{
					key.Crew,
					key.EmployeeId,
					key.ShiftDate,
					key.BlockId,
					key.LaborCode,
					Pieces = group.Sum(x => x.Pieces)
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
						WeekEndDate = weekEndDate,
						ShiftDate = employee.ShiftDate,
						BlockId = employee.BlockId,
						Crew = employee.Crew,
						LaborCode = employee.LaborCode,
						PayType = PayType.ProductionIncentiveBonus,
						Pieces = employee.Pieces,
						PieceRate = rate,
						GrossFromPieces = gross,
						TotalGross = gross,
					});
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
