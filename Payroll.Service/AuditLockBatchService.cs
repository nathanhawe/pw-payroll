using Payroll.Data;
using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	public class AuditLockBatchService : IAuditLockBatchService
	{
		private readonly PayrollContext _context;

		public AuditLockBatchService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>AuditLockBatch</c> to the database.
		/// </summary>
		/// <param name="batch"></param>
		/// <param name="owner"></param>
		public void AddAuditLockBatch(AuditLockBatch batch, string owner)
		{
			batch.IsDeleted = false;
			batch.IsComplete = false;
			batch.Owner = owner;
			_context.AuditLockBatches.Add(batch);
			_context.SaveChanges();
		}

		/// <summary>
		/// Returns true if processing rules allow a new audit lock batch to be created.
		/// </summary>
		/// <returns></returns>
		public bool CanAddAuditLockBatch()
		{
			//return _context.AuditLockBatches.Where(x => !x.IsDeleted && !x.IsComplete).Count() == 0;
			return true;
		}

		/// <summary>
		/// Returns the <c>AuditLockBatch</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public AuditLockBatch GetAuditLockBatch(int id)
		{
			return _context.AuditLockBatches
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns all of the <c>AuditLockBatch</c> records with pagination.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<AuditLockBatch> GetAuditLockBatches(int offset, int limit, bool orderByDescending)
		{
			if (offset < 0) offset = 0;

			var query = _context.AuditLockBatches.Where(x => !x.IsDeleted);
			if (orderByDescending)
			{
				query = query.OrderByDescending(o => o.Id);
			}
			else
			{
				query = query.OrderBy(o => o.Id);
			}

			return query
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Returns the count of active <c>AuditLockBatch</c> records in the database.
		/// </summary>
		/// <returns></returns>
		public int GetTotalAuditLockBatchCount()
		{
			return _context.AuditLockBatches.Where(x => !x.IsDeleted).Count();
		}
	}
}
