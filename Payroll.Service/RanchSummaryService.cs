using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Generates ranch summaries.
	/// </summary>
	public class RanchSummaryService
	{
		private readonly PayrollContext _context;

		public RanchSummaryService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Create <c>RanchSummary</c> records based on the provided batch ID number.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		public List<RanchSummary> CreateSummariesForBatch(int batchId)
		{
			// Entity Framework cannot execute sub queries to determine CulturalHours only through SQL.
			// Therefore, we process as much as we can on SQL and then calculate the cultural hours in memory.
			var temp = _context.RanchPayLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId, g.Crew }, (key, group) => new
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					Crew = key.Crew,
					LastCrew = group.Max(x => x.LastCrew),
					TotalHours = group.Sum(x => x.HoursWorked),
					TotalGross = group.Sum(x => x.TotalGross)
				})
				.ToList();

			var summaries = temp
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId }, (key, group) => new RanchSummary
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					LastCrew = group.Max(x => x.LastCrew),
					TotalHours = group.Sum(x => x.TotalHours),
					TotalGross = group.Sum(x => x.TotalGross),
					CulturalHours = group.Where(x => x.Crew < 60).Sum(x => x.TotalHours)
				})
				.ToList();

			return summaries;
		}
	}
}
