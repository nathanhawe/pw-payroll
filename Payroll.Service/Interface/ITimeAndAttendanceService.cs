using Payroll.Domain;

namespace Payroll.Service.Interface
{
	public interface ITimeAndAttendanceService
	{
		void PerformCalculations(int batchId);
	}
}
