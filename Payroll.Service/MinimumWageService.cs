using Payroll.Data;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	public class MinimumWageService : IMinimumWageService
	{
		private readonly PayrollContext _context;

		public MinimumWageService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Returns the minimum wage effective on the provided date or 0.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public decimal GetMinimumWageOnDate(DateTime date)
		{
			var currentMinimumWage = _context.MinimumWages
				.Where(x => !x.IsDeleted && x.EffectiveDate <= date)
				.OrderByDescending(o => o.EffectiveDate)
				.FirstOrDefault();

			return currentMinimumWage?.Wage ?? 0;
		}
	}
}
