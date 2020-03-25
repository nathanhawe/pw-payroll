using Payroll.Data;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Selects effective crew labor rates.
	/// </summary>
	public class CrewLaborWageSelector : ICrewLaborWageSelector
	{
		private readonly PayrollContext _context;

		public CrewLaborWageSelector(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Returns the crew labor rate effective as of the provided shift date.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		public decimal GetCrewLaborWage(DateTime shiftDate)
		{
			return _context.CrewLaborWages
				.Where(x =>
					!x.IsDeleted
					&& x.EffectiveDate <= shiftDate)
				.OrderByDescending(o => o.EffectiveDate)
				.FirstOrDefault()
				?.Wage ?? 0;
		}
	}
}
