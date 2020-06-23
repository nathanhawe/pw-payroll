using Microsoft.EntityFrameworkCore.Internal;
using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Handles interactions with the crew boss wage data source
	/// </summary>
	public class CrewBossWageService : Interface.ICrewBossWageService
	{
		private readonly PayrollContext _context;

		public CrewBossWageService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>CrewBossWage</c> to the database.
		/// </summary>
		/// <param name="crewBossWage"></param>
		public void AddWage(CrewBossWage crewBossWage)
		{
			crewBossWage.IsDeleted = false;
			_context.Add(crewBossWage);
			_context.SaveChanges();
		}

		/// <summary>
		/// Logically deletes the <c>CrewBossWage</c> with the matching ID.
		/// </summary>
		/// <param name="id"></param>
		public CrewBossWage DeleteWage(int id)
		{
			var crewBossWage = _context.CrewBossWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if(crewBossWage != null)
			{
				crewBossWage.IsDeleted = true;
				_context.SaveChanges();
			}
			
			return crewBossWage;
		}

		/// <summary>
		/// Returns the count of all active <c>CrewBossWage</c> records.
		/// </summary>
		/// <returns></returns>
		public int GetTotalCrewBossWageCount()
		{
			return _context.CrewBossWages.Where(x => !x.IsDeleted).Count();
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

		/// <summary>
		/// Returns the <c>CrewBossWage</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CrewBossWage GetWage(int id)
		{
			var crewBossWage = _context.CrewBossWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			return crewBossWage;
			
		}

		/// <summary>
		/// Returns all of the <c>CrewBossWage</c> records.  Ignores orderByDescending flag.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<CrewBossWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			if (offset < 0) offset = 0;

			return _context.CrewBossWages
				.Where(x => !x.IsDeleted)
				.OrderBy(o => o.WorkerCountThreshold)
				.ThenByDescending(o => o.EffectiveDate)
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Updates the existing <c>CrewBossWage</c> with the passed ID number.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="crewBossWage"></param>
		public CrewBossWage UpdateWage(int id, CrewBossWage crewBossWage)
		{
			if (crewBossWage == null) throw new ArgumentNullException(nameof(crewBossWage));

			var existingWage = _context.CrewBossWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if (existingWage == null) throw new Exception($"CrewBossWage with ID '{id}' was not found.");

			existingWage.EffectiveDate = crewBossWage.EffectiveDate;
			existingWage.Wage = crewBossWage.Wage;
			existingWage.WorkerCountThreshold = crewBossWage.WorkerCountThreshold;
			_context.SaveChanges();

			return existingWage;
		}
	}
}
