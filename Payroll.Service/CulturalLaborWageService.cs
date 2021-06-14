using Payroll.Data;
using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Service
{
	/// <summary>
	/// Handles interactions with the cultural labor wage data source.
	/// </summary>
	public class CulturalLaborWageService : ICulturalLaborWageService
	{
		private readonly PayrollContext _context;
		
		public CulturalLaborWageService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>CulturalLaborWage</c> to the database.
		/// </summary>
		/// <param name="culturalLaborWage"></param>
		public void AddWage(CulturalLaborWage culturalLaborWage)
		{
			culturalLaborWage.IsDeleted = false;
			_context.CulturalLaborWages.Add(culturalLaborWage);
			_context.SaveChanges();
		}

		/// <summary>
		/// Logically deletes the <c>CulturalLaborWage</c> with the matching ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CulturalLaborWage DeleteWage(int id)
		{
			var wage = _context.CulturalLaborWages
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
		/// Returns the total count of active <c>CulturalLaborWage</c> records.
		/// </summary>
		/// <returns></returns>
		public int GetTotalCulturalLaborWageCount()
		{
			return _context.CulturalLaborWages.Where(x => !x.IsDeleted).Count();
		}

		/// <summary>
		/// Returns the <c>CulturalLaborWage</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CulturalLaborWage GetWage(int id)
		{
			var wage = _context.CulturalLaborWages
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();

			return wage;
		}

		/// <summary>
		/// Returns the cultural labor wage effective as of the proivded shift date.
		/// </summary>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		public decimal GetWage(DateTime shiftDate)
		{
			return _context.CulturalLaborWages
				.Where(x =>
					!x.IsDeleted
					&& x.EffectiveDate <= shiftDate)
				.OrderByDescending(o => o.EffectiveDate)
				.FirstOrDefault()
				?.Wage ?? 0;
		}

		/// <summary>
		/// Returns all of the <c>CulturalLaborWage</c> records.  Ignores orderByDescending flag.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<CulturalLaborWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			return _context.CulturalLaborWages
				.Where(x => !x.IsDeleted)
				.OrderByDescending(o => o.EffectiveDate)
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Updates the existing <c>CulturalLaborWage</c> that matches the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="culturalLaborWage"></param>
		/// <returns></returns>
		public CulturalLaborWage UpdateWage(int id, CulturalLaborWage culturalLaborWage)
		{
			if (culturalLaborWage == null) throw new ArgumentNullException(nameof(culturalLaborWage));

			var existingWage = _context.CulturalLaborWages
				.Where(x => !x.IsDeleted && x.Id == id)
				.FirstOrDefault();

			if (existingWage == null) throw new Exception($"CulturalLaborWage with ID '{id}' was not found.");

			existingWage.EffectiveDate = culturalLaborWage.EffectiveDate;
			existingWage.Wage = culturalLaborWage.Wage;
			_context.SaveChanges();

			return existingWage;
		}
	}
}
