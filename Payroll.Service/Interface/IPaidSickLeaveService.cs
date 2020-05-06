using System;

namespace Payroll.Service.Interface
{
	public interface IPaidSickLeaveService
	{
		void UpdateTracking(int batchId, string company);
		void UpdateUsage(int batchId, string company);
		void CalculateNinetyDay(int batchId, string company, DateTime startDate, DateTime endDate);
		decimal GetNinetyDayRate(int batchId, string company, string employeeId, DateTime shiftDate);
	}
}
