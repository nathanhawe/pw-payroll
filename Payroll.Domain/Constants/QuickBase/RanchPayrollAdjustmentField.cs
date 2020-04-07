using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Ranch Payroll Adjustment table in Quick Base.
	/// </summary>
	public enum RanchPayrollAdjustmentField
	{
		Unknown = 0,
		RecordId = 3,
		Layoff = 92,
		LayoffRunId = 86,
		WeekEndDate = 9,
		ShiftDate = 7,
		Crew = 34,
		EmployeeNumber = 29,
		LaborCode = 59,
		RelatedBlock = 39,
		HoursWorked = 15,
		PayType = 17,
		Pieces = 18,
		PieceRate = 19,
		HourlyRate = 20,
		GrossFromHours = 23,
		GrossFromPieces = 22,
		OtherGross = 24,
		TotalGross = 25,
		AlternativeWorkWeek = 32,
		//FiveEight = ,
		//HourlyRateOverride = 145,
		EmployeeHourlyRate = 33,
		WeekEndOfAdjustmentPaid = 28,
		OriginalOrNew = 27,
		OldHourlyRate = 51,
		UseOldHourlyRate = 52,
	}
}
