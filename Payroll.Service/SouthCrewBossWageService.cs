using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Service
{
	/// <summary>
	/// Handles interactions with the south crew boss wage data source
	/// </summary>
	public class SouthCrewBossWageService : Interface.ISouthCrewBossWageService
	{
		private readonly PayrollContext _context;

		public SouthCrewBossWageService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>SouthCrewBossWage</c> to the database.
		/// </summary>
		/// <param name="wage"></param>
		public void AddWage(SouthCrewBossWage wage)
		{
			wage.IsDeleted = false;
			_context.Add(wage);
			_context.SaveChanges();
		}

		/// <summary>
		/// Logically deletes the <c>SouthCrewBossWage</c> with the matching ID.
		/// </summary>
		/// <param name="id"></param>
		public SouthCrewBossWage DeleteWage(int id)
		{
			var wage = _context.SouthCrewBossWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if(wage != null)
			{
				wage.IsDeleted = true;
				_context.SaveChanges();
			}
			
			return wage;
		}

		/// <summary>
		/// Returns the count of all active <c>SouthCrewBossWage</c> records.
		/// </summary>
		/// <returns></returns>
		public int GetTotalWageCount()
		{
			return _context.SouthCrewBossWages.Where(x => !x.IsDeleted).Count();
		}

		/// <summary>
		/// Returns the south crew boss wage effective for the provided shift date and count of workers.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <param name="countOfWorkers"></param>
		/// <returns></returns>
		public decimal GetWage(DateTime shiftDate, int countOfWorkers)
		{
			return _context.SouthCrewBossWages
				.Where(x =>
					!x.IsDeleted
					&& x.EffectiveDate <= shiftDate
					&& x.WorkerCountThreshold <= countOfWorkers)
				.OrderByDescending(o => o.EffectiveDate)
				.ThenByDescending(o => o.WorkerCountThreshold)
				.FirstOrDefault()
				?.Wage ?? 0;
		}

		/// <summary>
		/// Returns the <c>SouthCrewBossWage</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public SouthCrewBossWage GetWage(int id)
		{
			var wage = _context.SouthCrewBossWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			return wage;
			
		}

		/// <summary>
		/// Returns all of the <c>SouthCrewBossWage</c> records.  Ignores orderByDescending flag.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<SouthCrewBossWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			if (offset < 0) offset = 0;

			return _context.SouthCrewBossWages
				.Where(x => !x.IsDeleted)
				.OrderBy(o => o.WorkerCountThreshold)
				.ThenByDescending(o => o.EffectiveDate)
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Updates the existing <c>SouthCrewBossWage</c> with the passed ID number.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="wage"></param>
		public SouthCrewBossWage UpdateWage(int id, SouthCrewBossWage wage)
		{
			if (wage == null) throw new ArgumentNullException(nameof(wage));

			var existingWage = _context.SouthCrewBossWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if (existingWage == null) throw new Exception($"SouthCrewBossWage with ID '{id}' was not found.");

			existingWage.EffectiveDate = wage.EffectiveDate;
			existingWage.Wage = wage.Wage;
			existingWage.WorkerCountThreshold = wage.WorkerCountThreshold;
			_context.SaveChanges();

			return existingWage;
		}
	}
}
