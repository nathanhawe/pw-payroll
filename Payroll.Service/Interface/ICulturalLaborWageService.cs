using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ICulturalLaborWageService
	{
		void AddWage(Domain.CulturalLaborWage culturalLaborWage);
		Domain.CulturalLaborWage GetWage(int id);
		List<Domain.CulturalLaborWage> GetWages(int offset, int limit, bool orderByDescending);
		Domain.CulturalLaborWage UpdateWage(int id, Domain.CulturalLaborWage culturalLaborWage);
		Domain.CulturalLaborWage DeleteWage(int id);
		public decimal GetWage(DateTime shiftDate);
		int GetTotalCulturalLaborWageCount();
	}
}
