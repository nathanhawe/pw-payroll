using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Generates Plant Summaries
	/// </summary>
	public class PlantSummaryService
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
			var summaries = _context.PlantPayLines
				.Where(x => x.BatchId == batchId)
				.GroupBy(g => new { g.WeekEndDate, g.EmployeeId }, (key, group) => new PlantSummary
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					WeekEndDate = key.WeekEndDate,
					TotalHours = group.Sum(x => x.HoursWorked),
					TotalGross = group.Sum(x => x.TotalGross)
				})
				.ToList();

			return summaries;
		}
	}
}
