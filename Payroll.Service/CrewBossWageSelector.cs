using Payroll.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Retrieves crew boss wages based on worker count.
	/// </summary>
	public class CrewBossWageSelector : Interface.ICrewBossWageSelector
	{
		private readonly PayrollContext _context;

		public CrewBossWageSelector(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Returns the crew boss wage effective for the provided shift date and count of workers.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="countOfWorkers"></param>
		/// <returns></returns>
		public decimal GetWage(DateTime shiftDate, int countOfWorkers)
		{
			return _context.CrewBossWages
				.Where(x =>
					!x.IsDeleted
					&& x.EffectiveDate <= shiftDate
					&& x.WorkerCountThreshold <= countOfWorkers)
				.OrderByDescending(o => o.EffectiveDate)
				.ThenByDescending(o => o.WorkerCountThreshold)
				.FirstOrDefault()
				?.Wage ?? 0;
		}

	}
}
