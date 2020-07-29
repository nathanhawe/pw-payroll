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
	public class SummaryBatchService : ISummaryBatchService
	{
		private readonly PayrollContext _context;

		public SummaryBatchService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a new <c>SummaryBatch</c> to the database.
		/// </summary>
		/// <param name="summaryBatch"></param>
		/// <param name="owner"></param>
		public void AddSummaryBatch(SummaryBatch summaryBatch, string owner)
		{
			summaryBatch.IsDeleted = false;
			summaryBatch.IsComplete = false;
			summaryBatch.Owner = owner;
			_context.SummaryBatches.Add(summaryBatch);
			_context.SaveChanges();
		}

		/// <summary>
		/// Returns true if processing rules allow a new summary batch to be created.
		/// </summary>
		/// <returns></returns>
		public bool CanAddSummaryBatch()
		{
			return _context.SummaryBatches.Where(x => !x.IsComplete).Count() == 0;
		}

		/// <summary>
		/// Returns the <c>SummaryBatch</c> with the passed ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public SummaryBatch GetSummaryBatch(int id)
		{
			return _context.SummaryBatches
				.Where(x => x.Id == id && !x.IsDeleted)
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns all of the <c>SummaryBatch</c> records with pagination.
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="itemsPerPage"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<SummaryBatch> GetSummaryBatches(int offset, int limit, bool orderByDescending)
		{
			if (offset < 0) offset = 0;

			var query = _context.SummaryBatches.Where(x => !x.IsDeleted);
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
		/// Returns the summary batch currently being processed or the most recently processed summary batch if no summary batch is currently being processed.
		/// </summary>
		/// <returns></returns>
		public SummaryBatch GetCurrentlyProcessingSummaryBatch()
		{
			return _context.SummaryBatches.OrderByDescending(x => x.Id).FirstOrDefault();
		}

		/// <summary>
		/// Returns the count of active <c>Batch</c> records in the database.
		/// </summary>
		/// <returns></returns>
		public int GetTotalSummaryBatchCount()
		{
			return _context.SummaryBatches.Where(x => !x.IsDeleted).Count();
		}
	}
}
