using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Ranch Payroll table in Quick Base.
	/// </summary>
	public enum RanchPayrollField
	{
		Unknown = 0,
		RecordId = 3,
		LayoffPay = 180,
		LayoffRunId = 213,
		WeekEndDate = 13,
		ShiftDate = 11,
		Crew = 23,
		LastCrew = 114,
		EmployeeNumber = 7,
		LaborCode = 112,
		RelatedBlock = 19,
		HoursWorked = 29,
		PayType = 30,
		Pieces = 31,
		PieceRate = 32,
		HourlyRate = 33,
		GrossFromHours = 39,
		GrossFromPieces = 34,
		OtherGross = 43,
		TotalGross = 44,
		AlternativeWorkWeek = 50,
		FiveEight = 131,
		HourlyRateOverride = 145,
		EmployeeHourlyRate = 36,

		/// <summary>
		/// [41.1 Approval] - Checkbox indicates that a pay line with pay type 41.1 is approved to be paid out.
		/// </summary>
		SpecialAdjustmentApproval = 254
	}
}
