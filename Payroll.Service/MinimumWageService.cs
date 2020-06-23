using Payroll.Data;
using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Handles interactions with the minimum wage data source
	/// </summary>
	public class MinimumWageService : IMinimumWageService
	{
		private readonly PayrollContext _context;

		public MinimumWageService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>MinimumWage</c> to the database.
		/// </summary>
		/// <param name="minimumWage"></param>
		public void AddWage(MinimumWage minimumWage)
		{
			minimumWage.IsDeleted = false;
			_context.MinimumWages.Add(minimumWage);
			_context.SaveChanges();
		}

		/// <summary>
		/// Logically deletes the <c>MinimumWage</c> with the matching ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public MinimumWage DeleteWage(int id)
		{
			var minimumWage = _context.MinimumWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if (minimumWage != null)
			{
				minimumWage.IsDeleted = true;
				_context.SaveChanges();
			}

			return minimumWage;
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

		/// <summary>
		/// Returns the total count of active <c>MinimumWage</c> records.
		/// </summary>
		/// <returns></returns>
		public int GetTotalMininumWageCount()
		{
			return _context.MinimumWages.Where(x => !x.IsDeleted).Count();
		}

		/// <summary>
		/// Returns the <c>MinimumWage</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public MinimumWage GetWage(int id)
		{
			var minimumWage = _context.MinimumWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			return minimumWage;
		}

		/// <summary>
		/// Returns all of the <c>MinimumWage</c> records.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<MinimumWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			var query = _context.MinimumWages.Where(x => !x.IsDeleted);

			if (orderByDescending)
			{
				query = query.OrderByDescending(o => o.EffectiveDate);
			}
			else
			{
				query = query.OrderBy(o => o.EffectiveDate);
			}
			
			return query
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Updates the existing <c>MinimumWage</c> with the passed ID number.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="minimumWage"></param>
		/// <returns></returns>
		public MinimumWage UpdateWage(int id, MinimumWage minimumWage)
		{
			if (minimumWage == null) throw new ArgumentNullException(nameof(minimumWage));

			var existingWage = _context.MinimumWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if (existingWage == null) throw new Exception($"MinimumWage with ID '{id}' was not found.");

			existingWage.EffectiveDate = minimumWage.EffectiveDate;
			existingWage.Wage = minimumWage.Wage;
			_context.SaveChanges();

			return existingWage;
		}
	}
}
