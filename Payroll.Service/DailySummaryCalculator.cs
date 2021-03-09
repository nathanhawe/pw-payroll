using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Creates <c>DailySummary</c> objects.
	/// </summary>
	public class DailySummaryCalculator : IDailySummaryCalculator
	{
		private readonly PayrollContext _context;
		private readonly IMinimumWageService _minimumWageService;
		private readonly IRoundingService _roundingService;
		private readonly ICrewLaborWageService _crewLaborWageService;

		private class CommonLineProperties
		{
			public int QuickBaseRecordId { get; set; }
			public string EmployeeId { get; set; }
			public DateTime WeekEndDate { get; set; }
			public bool AlternativeWorkWeek { get; set; }
			public DateTime ShiftDate { get; set; }
			public int Crew { get; set; }
			public bool FiveEight { get; set; }
			public decimal TotalGross { get; set; }
			public decimal HoursWorked { get; set; }
			public decimal LaborCode { get; set; }
			public bool UseCrewLaborRateForPlantMinimumAssurance { get; set; }

		}

		public DailySummaryCalculator(
			PayrollContext context,
			IMinimumWageService minimumWageService,
			IRoundingService roundingService,
			ICrewLaborWageService crewLaborWageService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_minimumWageService = minimumWageService ?? throw new ArgumentNullException(nameof(context));
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
			_crewLaborWageService = crewLaborWageService ?? throw new ArgumentNullException(nameof(crewLaborWageService));
		}

		/// <summary>
		/// Creates <c>DailySummary</c> objects from the pay lines of the provided batchId and <c>Company</c>.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public List<DailySummary> GetDailySummaries(int batchId, string company)
		{
			var summaries = new List<DailySummary>();

			if (company == Company.Ranches)
			{
				summaries = GetDailyRanchSummaries(batchId);
			}
			else if (company == Company.Plants)
			{
				summaries = GetDailyPlantSummaries(batchId);
			}

			return summaries;
		}

		/// <summary>
		/// Creates <c>DailySummary</c> objects from the adjustment lines of the provided batchId and <c>Company</c>.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public List<DailySummary> GetDailySummariesFromAdjustments(int batchId, string company)
		{
			var summaries = new List<DailySummary>();

			if (company == Company.Ranches)
			{
				summaries = GetDailyRanchAdjustmentSummaries(batchId);
			}
			else if (company == Company.Plants)
			{
				summaries = GetDailyPlantAdjustmentSummaries(batchId);
			}

			return summaries;
		}

		/// <summary>
		/// Produces ranch pay lines daily summaries.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<DailySummary> GetDailyRanchSummaries(int batchId)
		{
			/*
			 Select only pay types:
				1-Regular
				4-Pieces
				4.1-Hourly plus Pieces
				4.2-Production Incentive Bonus
				8.1-CB Daily Rate
				8.2-CB Per Worker Rate
				8.3-CB Hourly Trees
				8.4-CB Hourly Vines
				8.5-CB South Daily
				8.6-CB South Hourly
				9-Commission
			*/

			var commonLineProperties = _context.RanchPayLines
				.Where(x =>
					x.BatchId == batchId
					&& (
						x.PayType == PayType.Regular
						|| x.PayType == PayType.Pieces
						|| x.PayType == PayType.HourlyPlusPieces
						|| x.PayType == PayType.ProductionIncentiveBonus
						|| x.PayType == PayType.CBDaily
						|| x.PayType == PayType.CBPerWorker
						|| x.PayType == PayType.CBHourlyTrees
						|| x.PayType == PayType.CBHourlyVines
						|| x.PayType == PayType.CBSouthDaily
						|| x.PayType == PayType.CBSouthHourly
						|| x.PayType == PayType.CBCommission))
				.Select(x => new CommonLineProperties
				{
					EmployeeId = x.EmployeeId,
					WeekEndDate = x.WeekEndDate,
					AlternativeWorkWeek = x.AlternativeWorkWeek,
					ShiftDate = x.ShiftDate,
					Crew = x.Crew,
					FiveEight = x.FiveEight,
					TotalGross = x.TotalGross,
					HoursWorked = x.HoursWorked,
					LaborCode = x.LaborCode,
					QuickBaseRecordId = x.QuickBaseRecordId
				})
				.ToList();

			var summaries = RanchGrouping(commonLineProperties);
			SetRates(summaries);

			return summaries;
		}

		/// <summary>
		/// Produces ranch adjustment line daily summaries.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<DailySummary> GetDailyRanchAdjustmentSummaries(int batchId)
		{
			/*
			 Select only pay types:
				1-Regular
				4-Pieces
				4.1-Hourly plus Pieces
				4.2-Production Incentive Bonus
				8.1-CB Daily Rate
				8.2-CB Per Worker Rate
				8.3-CB Hourly Trees
				8.4-CB Hourly Vines
				8.5-CB South Daily
				8.6-CB South Hourly
				9-Commission
			*/

			var commonLineProperties = _context.RanchAdjustmentLines
				.Where(x =>
					x.BatchId == batchId
					&& (
						x.PayType == PayType.Regular
						|| x.PayType == PayType.Pieces
						|| x.PayType == PayType.HourlyPlusPieces
						|| x.PayType == PayType.ProductionIncentiveBonus
						|| x.PayType == PayType.CBDaily
						|| x.PayType == PayType.CBPerWorker
						|| x.PayType == PayType.CBHourlyTrees
						|| x.PayType == PayType.CBHourlyVines
						|| x.PayType == PayType.CBSouthDaily
						|| x.PayType == PayType.CBSouthHourly
						|| x.PayType == PayType.CBCommission)
					&& !x.IsOriginal)
				.Select(x => new CommonLineProperties
				{
					EmployeeId = x.EmployeeId,
					WeekEndDate = x.WeekEndDate,
					AlternativeWorkWeek = x.AlternativeWorkWeek,
					ShiftDate = x.ShiftDate,
					Crew = x.Crew,
					FiveEight = x.FiveEight,
					TotalGross = x.TotalGross,
					HoursWorked = x.HoursWorked,
					LaborCode = x.LaborCode,
					QuickBaseRecordId = x.QuickBaseRecordId
				})
				.ToList();

			var summaries = RanchGrouping(commonLineProperties);
			SetRates(summaries);

			return summaries;
		}

		/// <summary>
		/// Produces plant pay line daily summaries.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<DailySummary> GetDailyPlantSummaries(int batchId)
		{
			/*
			 Select only pay types:
				1-Regular
				4-Pieces
			*/

			var commonLineProperties = _context.PlantPayLines
				.Where(x =>
					x.BatchId == batchId
					&& (
						x.PayType == PayType.Regular
						|| x.PayType == PayType.Pieces))
				.Select(x => new CommonLineProperties
				{
					EmployeeId = x.EmployeeId,
					WeekEndDate = x.WeekEndDate,
					AlternativeWorkWeek = x.AlternativeWorkWeek,
					ShiftDate = x.ShiftDate,
					Crew = x.Plant,
					TotalGross = x.TotalGross,
					HoursWorked = x.HoursWorked,
					LaborCode = x.LaborCode,
					QuickBaseRecordId = x.QuickBaseRecordId,
					UseCrewLaborRateForPlantMinimumAssurance = x.UseCrewLaborRateForMinimumAssurance
				})
				.ToList();

			var summaries = PlantGrouping(commonLineProperties);
			SetRates(summaries);

			return summaries;
		}

		/// <summary>
		/// Produces plant adjustment line daily summaries.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		private List<DailySummary> GetDailyPlantAdjustmentSummaries(int batchId)
		{
			/*
			 Select only pay types:
				1-Regular
				4-Pieces
			*/

			var commonLineProperties = _context.PlantAdjustmentLines
				.Where(x =>
					x.BatchId == batchId
					&& (
						x.PayType == PayType.Regular
						|| x.PayType == PayType.Pieces)
					&& !x.IsOriginal)
				.Select(x => new CommonLineProperties
				{
					EmployeeId = x.EmployeeId,
					WeekEndDate = x.WeekEndDate,
					AlternativeWorkWeek = x.AlternativeWorkWeek,
					ShiftDate = x.ShiftDate,
					Crew = x.Plant,
					TotalGross = x.TotalGross,
					HoursWorked = x.HoursWorked,
					LaborCode = x.LaborCode,
					QuickBaseRecordId = x.QuickBaseRecordId,
					UseCrewLaborRateForPlantMinimumAssurance = x.UseCrewLaborRateForMinimumAssurance
				})
				.ToList();

			var summaries = PlantGrouping(commonLineProperties);
			SetRates(summaries);

			return summaries;
		}

		/// <summary>
		/// Groups <c>CommonLineProperties</c> for ranch adjustment and pay lines.
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		private List<DailySummary> RanchGrouping(List<CommonLineProperties> common)
		{
			/* 
			 Use last of crew when records are sorted by Quick Base record ID
			 Use last of FiveEights when sorted by record ID
			 Grouped by EmployeeID
			 Grouped by Week Ending Date
			 Grouped by ShiftDate
			 Grouped by Alternative Work Week
			 Summarizes Hours, Total Gross, Non-Productive Time
			*/

			var summaries = common
				.GroupBy(g => new { g.EmployeeId, g.WeekEndDate, g.AlternativeWorkWeek, g.ShiftDate })
				.Select(x => new DailySummary
				{
					EmployeeId = x.Key.EmployeeId,
					WeekEndDate = x.Key.WeekEndDate,
					AlternativeWorkWeek = x.Key.AlternativeWorkWeek,
					ShiftDate = x.Key.ShiftDate,
					Crew = x.OrderByDescending(o => o.QuickBaseRecordId).First().Crew,
					FiveEight = x.OrderByDescending(o => o.QuickBaseRecordId).First().FiveEight,
					TotalGross = x.Sum(s => s.TotalGross),
					TotalHours = x.Sum(s => s.HoursWorked),
					NonProductiveTime = x.Where(w => w.LaborCode == 380 || w.LaborCode == 381).Sum(s => s.HoursWorked),
					NonProductiveGross = x.Where(w => w.LaborCode == 380 || w.LaborCode == 381).Sum(s => s.TotalGross),
				})
				.ToList();

			return summaries;
		}

		/// <summary>
		/// Groups <c>CommonLineProperties</c> for plant adjustment and pay lines.
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		private List<DailySummary> PlantGrouping(List<CommonLineProperties> common)
		{
			/* 
			 Use last of plant (crew) when records are sorted by Quick Base record ID
			 Grouped by EmployeeID
			 Grouped by Week Ending Date
			 Grouped by ShiftDate
			 Grouped by Alternative Work Week
			 Summarizes Hours, Total Gross, Non-Productive Time
			*/

			var summaries = common
				.GroupBy(g => new { g.EmployeeId, g.WeekEndDate, g.AlternativeWorkWeek, g.ShiftDate })
				.Select(x => new DailySummary
				{
					EmployeeId = x.Key.EmployeeId,
					WeekEndDate = x.Key.WeekEndDate,
					AlternativeWorkWeek = x.Key.AlternativeWorkWeek,
					ShiftDate = x.Key.ShiftDate,
					Crew = x.OrderByDescending(o => o.QuickBaseRecordId).First().Crew,
					TotalGross = x.Sum(s => s.TotalGross),
					TotalHours = x.Sum(s => s.HoursWorked),
					NonProductiveTime = x.Where(w => w.LaborCode == 380 || w.LaborCode == 381).Sum(s => s.HoursWorked),
					NonProductiveGross = x.Where(w => w.LaborCode == 380 || w.LaborCode == 381).Sum(s => s.TotalGross),
					UseCrewLaborRateForPlantMinimumAssurance = x.OrderByDescending(o => o.UseCrewLaborRateForPlantMinimumAssurance).First().UseCrewLaborRateForPlantMinimumAssurance
				})
				.ToList();

			return summaries;
		}

		/// <summary>
		/// Sets the value of <c>EffectiveHourlyRate</c> and <c>MinimumWage</c> on provided <c>DailySummary</c> objects.
		/// </summary>
		/// <param name="summaries"></param>
		private void SetRates(List<DailySummary> summaries)
		{
			// Calculate effective hourly rate and select minimum wage
			decimal divisor;
			summaries.ForEach(x =>
			{
				divisor = x.TotalHours - x.NonProductiveTime;
				if (divisor > 0)
				{
					x.EffectiveHourlyRate = _roundingService.Round((x.TotalGross-x.NonProductiveGross) / divisor, 2);
				}
				else
				{
					x.EffectiveHourlyRate = 0;
				}

				x.MinimumWage = _minimumWageService.GetMinimumWageOnDate(x.ShiftDate);

				// If crew labor rate should be used for minimum assurance use the larger of the current minimum wage
				// and the current crew labor rate
				if (x.UseCrewLaborRateForPlantMinimumAssurance)
				{
					x.MinimumWage = Math.Max(x.MinimumWage, _crewLaborWageService.GetWage(x.ShiftDate));
				}
				
				
			});
		}
	}
}
