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
	/// Handles interactions with the crew labor wage data source.
	/// </summary>
	public class CrewLaborWageService : ICrewLaborWageService
	{
		private readonly PayrollContext _context;

		public CrewLaborWageService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>CrewLaborWage</c> to the database.
		/// </summary>
		/// <param name="crewLaborWage"></param>
		public void AddWage(CrewLaborWage crewLaborWage)
		{
			crewLaborWage.IsDeleted = false;
			_context.CrewLaborWages.Add(crewLaborWage);
			_context.SaveChanges();
		}

		/// <summary>
		/// Logically deletes the <c>CrewLaborWage</c> with the matching ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CrewLaborWage DeleteWage(int id)
		{
			var crewLaborWage = _context.CrewLaborWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if(crewLaborWage != null)
			{
				crewLaborWage.IsDeleted = true;
				_context.SaveChanges();
			}

			return crewLaborWage;
		}

		/// <summary>
		/// Returns the total count of active <c>CrewLaborWage</c> records.
		/// </summary>
		/// <returns></returns>
		public int GetTotalCrewLaborWageCount()
		{
			return _context.CrewLaborWages.Where(x => !x.IsDeleted).Count();
		}

		/// <summary>
		/// Returns the crew labor rate effective as of the provided shift date.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		public decimal GetWage(DateTime shiftDate)
		{
			return _context.CrewLaborWages
				.Where(x =>
					!x.IsDeleted
					&& x.EffectiveDate <= shiftDate)
				.OrderByDescending(o => o.EffectiveDate)
				.FirstOrDefault()
				?.Wage ?? 0;
		}

		/// <summary>
		/// Returns the <c>CrewLaborWage</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CrewLaborWage GetWage(int id)
		{
			var crewLaborWage = _context.CrewLaborWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			return crewLaborWage;
		}

		/// <summary>
		/// Returns all of the <c>CrewLaborWage</c> records.  Ignores orderByDescending flag.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<CrewLaborWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			return _context.CrewLaborWages
				.Where(x => !x.IsDeleted)
				.OrderByDescending(o => o.EffectiveDate)
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Updates the existing <c>CrewLaborWage</c> with the passed ID number.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="crewLaborWage"></param>
		/// <returns></returns>
		public CrewLaborWage UpdateWage(int id, CrewLaborWage crewLaborWage)
		{
			if (crewLaborWage == null) throw new ArgumentNullException(nameof(crewLaborWage));

			var existingWage = _context.CrewLaborWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			if (existingWage == null) throw new Exception($"CrewLaborWage with ID '{id}' was not found.");

			existingWage.EffectiveDate = crewLaborWage.EffectiveDate;
			existingWage.Wage = crewLaborWage.Wage;
			_context.SaveChanges();

			return existingWage;

		}
	}
}
