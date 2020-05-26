using Payroll.Data;
using Payroll.Domain;
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
		public List<Batch> GetBatches(int pageNumber, int itemsPerPage, bool orderByDescending)
		{
			if (pageNumber < 1) pageNumber = 1;

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
				.Skip((pageNumber - 1) * itemsPerPage)
				.Take(itemsPerPage)
				.ToList();
		}
	}
}
