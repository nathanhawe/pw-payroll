using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IMinimumWageService
	{
		void AddWage(Domain.MinimumWage minimumWage);
		Domain.MinimumWage GetWage(int id);
		List<Domain.MinimumWage> GetWages(int offset, int limit, bool orderByDescending);
		Domain.MinimumWage UpdateWage(int id, Domain.MinimumWage minimumWage);
		Domain.MinimumWage DeleteWage(int id);
		public decimal GetMinimumWageOnDate(DateTime date);
		int GetTotalMininumWageCount();
	}
}
