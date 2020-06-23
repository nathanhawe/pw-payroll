using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ICrewBossWageService
	{
		void AddWage(Domain.CrewBossWage crewBossWage);
		Domain.CrewBossWage GetWage(int id);
		List<Domain.CrewBossWage> GetWages(int offset, int limit, bool orderByDescending);
		Domain.CrewBossWage UpdateWage(int id, Domain.CrewBossWage crewBossWage);
		Domain.CrewBossWage DeleteWage(int id);
		decimal GetWage(DateTime shiftDate, int countOfWorkers);
		int GetTotalCrewBossWageCount();
	}
}
