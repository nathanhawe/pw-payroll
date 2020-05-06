using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Crew Boss Pay table in Quick Base
	/// </summary>
	public enum CrewBossPayField
	{
		Unknown = 0,
		LayoffRunId = 55,
		LayoffPay = 51,
		RecordId = 3,
		WeekEndDate = 35,
		ShiftDate = 9,
		Crew = 6,
		CBPayMethod = 12,
		CountOfWorkers = 10,
		HoursWorkedByCB = 13,
		HourlyRate = 45,
		TotalGross = 27,
		EmployeeNumber = 15
	}
}
