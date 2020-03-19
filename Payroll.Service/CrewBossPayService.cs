using Payroll.Data;
using Payroll.Domain;
using System;
using System.Collections.Generic;

namespace Payroll.Service
{
	public class CrewBossPayService
	{
		private readonly PayrollContext _context;
		private readonly CrewBossWageSelector _wageSelector;

		public CrewBossPayService(PayrollContext context, CrewBossWageSelector wageSelector)
		{
			_context = context;
			_wageSelector = wageSelector;
		}

		public List<RanchPayLine> CalculateCrewBossPay(int batchId)
		{
			// Retrieve crew bosses for batch
			// For each crew boss in the batch
			// If daily south -> Apply Rate
			// If daily hourly -> Apply Rate
			// If hourly vines -> Lookup and apply rate based on worker count
			// If hourly trees -> Lookup and apply rate based on worker count
			// Update worker count and rate in CrewBossPay record
			// Create new RanchPayLine record

			// Add RanchPaylines to DB
			// Save Changes
			throw new NotImplementedException();
		}

		/// <summary>
		/// Selects the appropriate rate for the crew boss and calculates the gross
		/// </summary>
		/// <param name="payLine"></param>
		private void CalculatePay(CrewBossPayLine payLine)
		{
			throw new NotImplementedException();
		}
	}
}
