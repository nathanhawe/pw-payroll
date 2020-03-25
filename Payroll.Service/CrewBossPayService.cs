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
	/// Process crew boss pay.
	/// </summary>
	public class CrewBossPayService
	{
		private readonly PayrollContext _context;
		private readonly CrewBossWageSelector _wageSelector;
		private readonly IRoundingService _roundingService;

		public decimal SouthDailyPay { get; } = 170.05M;

		public CrewBossPayService(
			PayrollContext context, 
			CrewBossWageSelector wageSelector, 
			IRoundingService roundingService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_wageSelector = wageSelector ?? throw new ArgumentNullException(nameof(wageSelector));
			_roundingService = roundingService ?? throw new ArgumentNullException(nameof(roundingService));
		}

		/// <summary>
		/// Calculates crew boss pay.  This method updates the affected <c>CrewBossPayLine</c> records and returns a list
		/// of new, untracked <c>RanchPayLine</c> objects.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		public List<RanchPayLine> CalculateCrewBossPay(int batchId)
		{
			var ranchPayLines = new List<RanchPayLine>();

			// Retrieve crew bosses for batch
			var crewBossPayLines = _context.CrewBossPayLines
				.Where(x => x.BatchId == batchId)
				.ToList();

			// For each crew boss in the batch
			foreach(var cbLine in crewBossPayLines)
			{
				// Get worker count
				cbLine.WorkerCount = GetWorkerCount(batchId, cbLine.Crew, cbLine.ShiftDate);
				
				// Get hourly rate
				cbLine.HourlyRate = GetHourlyRate(cbLine.PayMethod, cbLine.WorkerCount, cbLine.ShiftDate);

				// Get gross
				cbLine.Gross = _roundingService.Round(CalculateGross(cbLine), 2);

				// Create new RanchPayLine record
				ranchPayLines.Add(CreateRanchPayLine(cbLine));
			}

			// Save chages to crew boss pay lines and return new ranch pay lines.
			_context.SaveChanges();
			return ranchPayLines;
		}

		/// <summary>
		/// Returns the count of workers for the provided crew and shift date in the specified batch.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="crew"></param>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		private int GetWorkerCount(int batchId, int crew, DateTime shiftDate)
		{
			return _context.RanchPayLines
				.Where(x =>
					x.BatchId == batchId
					&& x.Crew == crew
					&& x.ShiftDate == shiftDate)
				.GroupBy(g => g.EmployeeId, (key, group) => key)
				.Count();
		}

		/// <summary>
		///  Returns the correct hourly rate based on the provided pay method, worker count, and shift date.
		/// </summary>
		/// <param name="payMethod"></param>
		/// <param name="countOfWorkers"></param>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		private decimal GetHourlyRate(string payMethod, int countOfWorkers, DateTime shiftDate)
		{
			switch (payMethod)
			{
				case CrewBossPayMethod.SouthDaily: 
					return 0;
				case CrewBossPayMethod.SouthHourly: 
					return 17.90M;
				case CrewBossPayMethod.HourlyTrees: 
				case CrewBossPayMethod.HourlyVines:
					return _wageSelector.GetWage(shiftDate, countOfWorkers);
				default:
					return 0;
			}
		}

		/// <summary>
		/// Creates a new <c>RanchPayLine</c> based on the provided <c>CrewBossPayLine</c>.
		/// </summary>
		/// <param name="crewBossPayLine"></param>
		/// <returns></returns>
		private RanchPayLine CreateRanchPayLine(CrewBossPayLine crewBossPayLine)
		{
			return new RanchPayLine
			{
				BatchId = crewBossPayLine.BatchId,
				WeekEndDate = crewBossPayLine.WeekEndDate,
				ShiftDate = crewBossPayLine.ShiftDate,
				Crew = crewBossPayLine.Crew,
				EmployeeId = crewBossPayLine.EmployeeId,
				HoursWorked = crewBossPayLine.HoursWorked,
				PayType = GetPayType(crewBossPayLine.PayMethod),
				OtherGross = crewBossPayLine.Gross,
				TotalGross = crewBossPayLine.Gross
			};
		}

		/// <summary>
		/// Returns the gross amount for the provided <c>CrewBossPayLine</c>.
		/// </summary>
		/// <param name="crewBossPayLine"></param>
		/// <returns></returns>
		private decimal CalculateGross(CrewBossPayLine crewBossPayLine)
		{
			switch(crewBossPayLine.PayMethod)
			{
				case CrewBossPayMethod.HourlyTrees:
				case CrewBossPayMethod.HourlyVines:
				case CrewBossPayMethod.SouthHourly:
					return crewBossPayLine.HoursWorked * crewBossPayLine.HourlyRate;
				case CrewBossPayMethod.SouthDaily:
					return SouthDailyPay;
				default: return 0;
			};
		}

		/// <summary>
		/// Returns the correct pay type based on the passed crew boss pay line pay method.
		/// </summary>
		/// <param name="payMethod"></param>
		/// <returns></returns>
		private string GetPayType(string payMethod)
		{
			return payMethod switch
			{
				CrewBossPayMethod.HourlyTrees => PayType.CBHourlyTrees,
				CrewBossPayMethod.HourlyVines => PayType.CBHourlyVines,
				CrewBossPayMethod.SouthDaily => PayType.CBSouthDaily,
				CrewBossPayMethod.SouthHourly => PayType.CBSouthHourly,
				_ => string.Empty,
			};
		}
	}
}
