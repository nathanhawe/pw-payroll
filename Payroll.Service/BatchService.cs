using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	public class BatchService : IBatchService
	{
		private readonly PayrollContext _context;

		public BatchService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>Batch</c> to the database.
		/// </summary>
		/// <param name="batch"></param>
		/// <param name="owner"></param>
		public void AddBatch(Batch batch, string owner)
		{
			batch.IsDeleted = false;
			batch.IsComplete = false;
			batch.Owner = owner;
			_context.Batches.Add(batch);
			_context.SaveChanges();
		}

		/// <summary>
		/// Returns true if processing rules allow a new batch to be created.
		/// </summary>
		/// <returns></returns>
		public bool CanAddBatch()
		{
			return _context.Batches.Where(x => !x.IsComplete).Count() == 0;
		}

		/// <summary>
		/// Returns the <c>Batch</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Batch GetBatch(int id)
		{
			return _context.Batches
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns all of the <c>Batch</c> records with pagination.
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="itemsPerPage"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<Batch> GetBatches(int offset, int limit, bool orderByDescending)
		{
			if (offset < 0) offset = 0;

			var query = _context.Batches.Where(x => !x.IsDeleted);
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
		/// Returns the batch currently being processed or the most recently processed batch if no batch is currently being processed.
		/// </summary>
		/// <returns></returns>
		public Batch GetCurrentlyProcessingBatch()
		{
			return _context.Batches.OrderByDescending(x => x.Id).FirstOrDefault();
		}

		/// <summary>
		/// Returns the count of active <c>Batch</c> records in the database.
		/// </summary>
		/// <returns></returns>
		public int GetTotalBatchCount()
		{
			return _context.Batches.Where(x => !x.IsDeleted).Count();
		}
	}
}
