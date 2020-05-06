using System;

namespace Payroll.Service.Interface
{
	public interface ICrewBossWageSelector
	{
		decimal GetWage(DateTime shiftDate, int countOfWorkers);
	}
}
