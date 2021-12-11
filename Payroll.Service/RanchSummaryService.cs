using Microsoft.EntityFrameworkCore.Query;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Generates ranch summaries.
	/// </summary>
	public class RanchSummaryService : Interface.IRanchSummaryService
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
			// Entity Framework cannot execute sub queries to determine Cultural and COVID hours only through SQL.
			// Therefore, we process as much as we can on SQL and then calculate the cultural hours in memory.
			var temp = _context.RanchPayLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId, g.Crew, g.LaborCode }, (key, group) => new
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					Crew = key.Crew,
					LastCrew = group.Max(x => x.LastCrew),
					TotalHours = group.Sum(x => x.HoursWorked),
					TotalGross = group.Sum(x => x.TotalGross),
					LaborCode = key.LaborCode
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
					CulturalHours = group.Where(x => x.Crew <= 99 && x.Crew != (int)Crew.LightDuty_East && x.Crew != (int)Crew.LightDuty_South && x.Crew != (int)Crew.LightDuty_West).Sum(x => x.TotalHours),
					CovidHours = group.Where(x => x.LaborCode == (int)RanchLaborCode.Covid19).Sum(x => x.TotalHours)
				})
				.ToList();

			return summaries;
		}

		/// <summary>
		/// Create <c>RanchSummary</c> records based on the provided list of <c>RanchPayLine</c>s.  If the provided lines are
		/// for more than one week end date there may be multiple summaries created per employee (one per unique week end date).
		/// </summary>
		/// <param name="ranchPayLines"></param>
		/// <returns></returns>
		public List<RanchSummary> CreateSummariesFromList(List<RanchPayLine> ranchPayLines)
		{
			var summaries = ranchPayLines
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId }, (key, group) => new RanchSummary
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					LastCrew = group.Max(x => x.LastCrew),
					TotalHours = group.Sum(x => x.HoursWorked),
					TotalGross = group.Sum(x => x.TotalGross),
					CulturalHours = group.Where(x => x.Crew <= 99 && x.Crew != (int)Crew.LightDuty_East && x.Crew != (int)Crew.LightDuty_South && x.Crew != (int)Crew.LightDuty_West).Sum(x => x.HoursWorked),
					CovidHours = group.Where(x => x.LaborCode == (int)RanchLaborCode.Covid19).Sum(x => x.HoursWorked)
				})
				.ToList();

			return summaries;
		}
	}
}
