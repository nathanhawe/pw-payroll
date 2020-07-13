﻿using System;
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


		/// <summary>
		/// [Manual Input Hours Worked] - Numeric
		/// </summary>
		ManualInputHoursWorked = 28,

		HoursWorked = 29,
		PayType = 30,
		Pieces = 31,
		PieceRate = 32,
		HourlyRate = 33,
		GrossFromHours = 39,
		GrossFromPieces = 34,

		/// <summary>
		/// [OT DT WOT Rate] - Currency indicates the rate used for OT, DT, and WOT records.
		/// </summary>
		OtDtWotRate = 42,
		OtherGross = 43,
		TotalGross = 44,
		AlternativeWorkWeek = 50,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric indicates the number of hours for OT, DT, and WOT records.
		/// </summary>
		OtDtWotHours = 60,
		FiveEight = 131,
		HourlyRateOverride = 145,
		EmployeeHourlyRate = 36,

		/// <summary>
		/// [41.1 Approval] - Checkbox indicates that a pay line with pay type 41.1 is approved to be paid out.
		/// </summary>
		SpecialAdjustmentApproval = 254,
		
		/// <summary>
		/// [Batch ID] - Numeric indicates the BatchId value during the last calculations this record was used in.
		/// </summary>
		BatchId = 281,

		/// <summary>
		/// [Calculated Hourly Rate] - The hourly rate selected by the calculations.
		/// </summary>
		CalculatedHourlyRate = 282,

		/// <summary>
		/// [Calculated Gross From Hours] - The final gross from hours value.
		/// </summary>
		CalculatedGrossFromHours = 283,

		/// <summary>
		/// [Calculated Gross From Pieces] - This final gross from pieces value.
		/// </summary>
		CalculatedGrossFromPieces = 284,

		/// <summary>
		/// [Calculated Total Gross] - The final total gross value for the line.
		/// </summary>
		CalculatedTotalGross = 285
	}
}
