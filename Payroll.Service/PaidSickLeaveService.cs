using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payroll.Service
{
	/// <summary>
	/// Exposes database interactions with the PaidSickLeave table and performs 
	/// PSL calculations.
	/// </summary>
	public class PaidSickLeaveService : IPaidSickLeaveService
	{
		private readonly PayrollContext _context;
		private readonly IRoundingService _roundingService;

		/// <summary>
		/// Type of action performed by UpdateOrInsert method.
		/// </summary>
		private enum UpdateOrInsertType
		{
			All,
			Tracking,
			Usage,
			NinetyDay
		}

		public PaidSickLeaveService(PayrollContext context, IRoundingService roundingService)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_roundingService = roundingService ?? throw new ArgumentNullException();
		}

		/// <summary>
		/// Updates PSL records with new Gross and Hour values for the provided batch and company.
		/// This method creates a new PSL record if a matching one does not already exist.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="company"></param>
		public void UpdateTracking(int batchId, string company)
		{
			if (company == Company.Ranches)
			{
				UpdateRanchTracking(batchId);
			}
			else if (company == Company.Plants)
			{
				UpdatePlantTracking(batchId);
			}
		}

		/// <summary>
		/// Updates PSL records with new HoursUsed based on the provided batch and company.
		/// This method creates a new PSL record if a matching one does not already exist.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="company"></param>
		public void UpdateUsage(int batchId, string company)
		{
			if (company == Company.Ranches)
			{
				UpdateRanchUsage(batchId);
			}
			else if (company == Company.Plants)
			{
				UpdatePlantUsage(batchId);
			}
		}

		/// <summary>
		/// Updates PSL records within the inclusive range of <c>startDate</c> to <c>endDate</c> with
		/// new NinetyDayHours and NinetyDayGross values. This method creates a new PSL record if a 
		/// matching one does not already exist.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="company"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public void CalculateNinetyDay(int batchId, string company, DateTime startDate, DateTime endDate)
		{
			var paidSickLeaves = new List<PaidSickLeave>();
			List<PaidSickLeave> tempPsl;

			// Retrieve all PSL lines
			// For each PSL within startDate and endDate, figure out 90 day hour and gross totals
			for (DateTime calculatingDate = startDate; calculatingDate <= endDate; calculatingDate = calculatingDate.AddDays(1))
			{
				tempPsl = (
					from psl in _context.PaidSickLeaves
					join sub in _context.PaidSickLeaves on new { psl.BatchId, psl.Company, psl.EmployeeId, ShiftDate = calculatingDate } equals new { sub.BatchId, sub.Company, sub.EmployeeId, sub.ShiftDate } into j
					from sub in j.DefaultIfEmpty()
					where
						psl.BatchId == batchId
						&& psl.Company == company
						&& psl.ShiftDate >= calculatingDate.AddDays(-91)
						&& psl.ShiftDate < calculatingDate
						&& psl.IsDeleted == false
					select new
					{
						psl.EmployeeId,
						psl.ShiftDate,
						psl.Hours,
						psl.Gross,
						sub.Id
					})
					.GroupBy(g => new { g.EmployeeId }, (key, group) => new PaidSickLeave
					{
						BatchId = batchId,
						EmployeeId = key.EmployeeId,
						ShiftDate = calculatingDate,
						Company = company,
						NinetyDayHours = group.Sum(x => x.Hours),
						NinetyDayGross = group.Sum(x => x.Gross),
						Id = group.Max(x => x.Id)
					})
					.ToList();

				paidSickLeaves.AddRange(tempPsl);
			}

			UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.NinetyDay);
		}

		/// <summary>
		/// Returns the calculated ninety day rate for the provided parameter values.  This method assumes that ninety day gross and ninety day hours
		/// total values have been calculated already.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="company"></param>
		/// <param name="employeeId"></param>
		/// <param name="shiftDate"></param>
		/// <returns></returns>
		public decimal GetNinetyDayRate(int batchId, string company, string employeeId, DateTime shiftDate)
		{
			var psl = _context.PaidSickLeaves
				.Where(x =>
					x.BatchId == batchId
					&& x.EmployeeId == employeeId
					&& x.ShiftDate == shiftDate
					&& x.Company == company
					&& !x.IsDeleted)
				.FirstOrDefault();

			if ((psl?.NinetyDayHours ?? 0) == 0) return 0;

			return _roundingService.Round(psl.NinetyDayGross / psl.NinetyDayHours, 2);
		}

		/// <summary>
		/// Private helper method to update existing PSL records or insert new ones.
		/// </summary>
		/// <param name="paidSickLeaves"></param>
		/// <param name="type"></param>
		private void UpdateOrInsert(List<PaidSickLeave> paidSickLeaves, UpdateOrInsertType type)
		{
			PaidSickLeave existingSickLeave;

			foreach (var psl in paidSickLeaves)
			{
				existingSickLeave = (psl.Id > 0 ? _context.PaidSickLeaves.Find(psl.Id) : null);
				
				if (existingSickLeave != null)
				{
					if (type == UpdateOrInsertType.Tracking || type == UpdateOrInsertType.All)
					{
						existingSickLeave.Hours = psl.Hours;
						existingSickLeave.Gross = psl.Gross;
					}
					if (type == UpdateOrInsertType.Usage || type == UpdateOrInsertType.All)
					{
						existingSickLeave.HoursUsed = psl.HoursUsed;
					}
					if (type == UpdateOrInsertType.NinetyDay || type == UpdateOrInsertType.All)
					{
						existingSickLeave.NinetyDayHours = psl.NinetyDayHours;
						existingSickLeave.NinetyDayGross = psl.NinetyDayGross;
					}
				}
				else
				{
					_context.Add(psl);
				}
			}

			// Commit all changes made above
			_context.SaveChanges();
		}

		/// <summary>
		/// Private helper method to update ranch accrual tracking.
		/// </summary>
		/// <param name="batchId"></param>
		private void UpdateRanchTracking(int batchId)
		{
			// Retrieve ranch paylines with Regular, Pieces, Hourly Plus Pieces, and Crew Boss pay types
			// Group all pay lines by employee and date, sum hours, sum gross.
			var paidSickLeaves = (
				from payLine in _context.RanchPayLines
				join psl in _context.PaidSickLeaves on new { payLine.BatchId, payLine.EmployeeId, payLine.ShiftDate, IsDeleted = false } equals new { psl.BatchId, psl.EmployeeId, psl.ShiftDate, psl.IsDeleted } into j
				from psl in j.DefaultIfEmpty()
				where
					payLine.BatchId == batchId
					&& (
						payLine.PayType == PayType.Regular
						|| payLine.PayType == PayType.Pieces
						|| payLine.PayType == PayType.HourlyPlusPieces
						|| payLine.PayType == PayType.CBCommission
						|| payLine.PayType == PayType.CBDaily
						|| payLine.PayType == PayType.CBHourlyTrees
						|| payLine.PayType == PayType.CBHourlyVines
						|| payLine.PayType == PayType.CBPerWorker
						|| payLine.PayType == PayType.CBSouthDaily
						|| payLine.PayType == PayType.CBSouthHourly
						)
					&& payLine.IsDeleted == false
				select new
				{
					payLine.EmployeeId,
					payLine.ShiftDate,
					payLine.HoursWorked,
					payLine.TotalGross,
					PaidSickLeaveId = psl.Id
				})
				.GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					ShiftDate = key.ShiftDate,
					Hours = group.Sum(x => x.HoursWorked),
					Gross = group.Sum(x => x.TotalGross),
					Id = group.Max(x => x.PaidSickLeaveId),
					Company = Company.Ranches
				})
				.ToList();

			UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Tracking);
		}

		/// <summary>
		/// Private helper method to update plant accrual tracking.
		/// </summary>
		/// <param name="batchId"></param>
		private void UpdatePlantTracking(int batchId)
		{
			// Retrieve plant paylines with Regular and Pieces pay types only.
			// Group all pay lines by employee and date, sum hours, sum gross.
			var paidSickLeaves = (
				from payLine in _context.PlantPayLines
				join psl in _context.PaidSickLeaves on new { payLine.BatchId, payLine.EmployeeId, payLine.ShiftDate, IsDeleted = false } equals new { psl.BatchId, psl.EmployeeId, psl.ShiftDate, psl.IsDeleted } into j
				from psl in j.DefaultIfEmpty()
				where
					payLine.BatchId == batchId
					&& (
						payLine.PayType == PayType.Regular
						|| payLine.PayType == PayType.Pieces
						)
					&& payLine.IsDeleted == false
				select new
				{
					payLine.EmployeeId,
					payLine.ShiftDate,
					payLine.HoursWorked,
					payLine.TotalGross,
					PaidSickLeaveId = psl.Id
				})
				.GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					ShiftDate = key.ShiftDate,
					Hours = group.Sum(x => x.HoursWorked),
					Gross = group.Sum(x => x.TotalGross),
					Id = group.Max(x => x.PaidSickLeaveId),
					Company = Company.Plants
				})
				.ToList();


			UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Tracking);
		}

		/// <summary>
		/// Private helper method to update ranch PSL usage.
		/// </summary>
		/// <param name="batchId"></param>
		private void UpdateRanchUsage(int batchId)
		{
			var paidSickLeaves = (
				from payLine in _context.RanchPayLines
				join psl in _context.PaidSickLeaves on new { payLine.BatchId, payLine.EmployeeId, payLine.ShiftDate, IsDeleted = false } equals new { psl.BatchId, psl.EmployeeId, psl.ShiftDate, psl.IsDeleted } into j
				from psl in j.DefaultIfEmpty()
				where
					payLine.BatchId == batchId
					&& payLine.PayType == PayType.SickLeave
					&& payLine.IsDeleted == false
				select new
				{
					payLine.EmployeeId,
					payLine.ShiftDate,
					payLine.HoursWorked,
					PaidSickLeaveId = psl.Id
				})
				.GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					ShiftDate = key.ShiftDate,
					HoursUsed = group.Sum(x => x.HoursWorked),
					Id = group.Max(x => x.PaidSickLeaveId),
					Company = Company.Ranches
				})
				.ToList();

			UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Usage);
		}

		/// <summary>
		/// Private helper method to update plant PSL usage.
		/// </summary>
		/// <param name="batchId"></param>
		private void UpdatePlantUsage(int batchId)
		{
			var paidSickLeaves = (
				from payLine in _context.PlantPayLines
				join psl in _context.PaidSickLeaves on new { payLine.BatchId, payLine.EmployeeId, payLine.ShiftDate, IsDeleted = false } equals new { psl.BatchId, psl.EmployeeId, psl.ShiftDate, psl.IsDeleted } into j
				from psl in j.DefaultIfEmpty()
				where
					payLine.BatchId == batchId
					&& payLine.PayType == PayType.SickLeave
					&& payLine.IsDeleted == false
				select new
				{
					payLine.EmployeeId,
					payLine.ShiftDate,
					payLine.HoursWorked,
					PaidSickLeaveId = psl.Id
				})
				.GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
				{
					BatchId = batchId,
					EmployeeId = key.EmployeeId,
					ShiftDate = key.ShiftDate,
					HoursUsed = group.Sum(x => x.HoursWorked),
					Id = group.Max(x => x.PaidSickLeaveId),
					Company = Company.Plants
				})
				.ToList();

			UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Usage);
		}
	}
}
