using Microsoft.EntityFrameworkCore.Query;
using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Generates Plant Summaries
	/// </summary>
	public class PlantSummaryService : Interface.IPlantSummaryService
	{
		private readonly PayrollContext _context;

		public PlantSummaryService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Create <c>PlantSummary</c> records based on the provided batch ID number.
		/// </summary>
		/// <param name="batchId"></param>
		/// <returns></returns>
		public List<PlantSummary> CreateSummariesForBatch(int batchId)
		{
			// Entity Framework cannot execute sub queries to determine COVID hours only through SQL.
			// Therefore, we process as much as we can on SQL and then calculate the cultural hours in memory.
			var temp = _context.PlantPayLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId, g.LaborCode }, (key, group) => new
				{
					BatchId = batchId,
					key.EmployeeId,
					key.WeekEndDate,
					key.LaborCode,
					TotalHours = group.Sum(x => x.HoursWorked),
					TotalGross = group.Sum(x => x.TotalGross)
				})
				.ToList();

			var summaries = temp
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId }, (key, group) => new PlantSummary
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					TotalHours = group.Sum(x => x.TotalHours),
					TotalGross = group.Sum(x => x.TotalGross),
					CovidHours = group.Where(x => x.LaborCode == (int)PlantLaborCode.Covid19).Sum(x => x.TotalHours)
				})
				.ToList();

			return summaries;
		}

		/// <summary>
		/// Create <c>PlantSummary</c> records based on the provided list of <c>PlantPayLine</c>s.  If the provided lines are
		/// for more than one week end date there may be multiple summaries created per employee (one per unique week end date).
		/// </summary>
		/// <param name="plantPayLines"></param>
		/// <returns></returns>
		public List<PlantSummary> CreateSummariesFromList(List<PlantPayLine> plantPayLines)
		{
			var summaries = plantPayLines
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId }, (key, group) => new PlantSummary
				{
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					TotalHours = group.Sum(x => x.HoursWorked),
					TotalGross = group.Sum(x => x.TotalGross),
					CovidHours = group.Where(x => x.LaborCode == (int)PlantLaborCode.Covid19).Sum(x => x.HoursWorked)
				})
				.ToList();
			
			return summaries;
		}
	}
}
