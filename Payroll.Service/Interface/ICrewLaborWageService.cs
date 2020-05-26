using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ICrewLaborWageService
	{
		void AddWage(Domain.CrewLaborWage crewLaborWage);
		Domain.CrewLaborWage GetWage(int id);
		List<Domain.CrewLaborWage> GetWages();
		Domain.CrewLaborWage UpdateWage(int id, Domain.CrewLaborWage crewLaborWage);
		Domain.CrewLaborWage DeleteWage(int id);
		public decimal GetWage(DateTime shiftDate);
	}
}
