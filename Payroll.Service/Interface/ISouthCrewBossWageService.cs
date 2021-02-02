using System;
using System.Collections.Generic;

namespace Payroll.Service.Interface
{
	public interface ISouthCrewBossWageService
	{
		void AddWage(Domain.SouthCrewBossWage wage);
		Domain.SouthCrewBossWage GetWage(int id);
		List<Domain.SouthCrewBossWage> GetWages(int offset, int limit, bool orderByDescending);
		Domain.SouthCrewBossWage UpdateWage(int id, Domain.SouthCrewBossWage wage);
		Domain.SouthCrewBossWage DeleteWage(int id);
		decimal GetWage(DateTime shiftDate, int countOfWorkers);
		int GetTotalWageCount();
	}
}
