using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ICrewBossWageService
	{
		void AddWage(Domain.CrewBossWage crewBossWage);
		Domain.CrewBossWage GetWage(int id);
		List<Domain.CrewBossWage> GetWages();
		Domain.CrewBossWage UpdateWage(int id, Domain.CrewBossWage crewBossWage);
		Domain.CrewBossWage DeleteWage(int id);
		decimal GetWage(DateTime shiftDate, int countOfWorkers);
	}
}
