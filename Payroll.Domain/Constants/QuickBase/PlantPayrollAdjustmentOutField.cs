namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Plant Payroll Adjustment Out table in Quick Base.
	/// </summary>
	public enum PlantPayrollAdjustmentOutField
	{
		/// <summary>
		/// Not mapped
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// [Employee Number] - Text
		/// </summary>
		EmployeeNumber = 6,

		/// <summary>
		/// [Labor Code] - Numeric
		/// </summary>
		LaborCode = 17,

		/// <summary>
		/// [Layoff Run ID] - Numeric
		/// </summary>
		LayoffRunId = 19,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 21,

		/// <summary>
		/// [Box Style] - Numeric
		/// </summary>
		BoxStyle = 22,

		/// <summary>
		/// [Box Style Description] - Text
		/// </summary>
		BoxStyleDescription = 23,

		/// <summary>
		/// [End Time] - Date/Time
		/// </summary>
		EndTime = 25,

		/// <summary>
		/// [Gross From Hours] - Currency
		/// </summary>
		GrossFromHours = 26,

		/// <summary>
		/// [Gross From Incentive] - Currency
		/// </summary>
		GrossFromIncentive = 27,

		/// <summary>
		/// [Gross From Pieces] - Currency
		/// </summary>
		GrossFromPieces = 28,

		/// <summary>
		/// [H-2A Hours Offered] - Numeric
		/// </summary>
		H2AHoursOffered = 29,

		/// <summary>
		/// [Hourly Rate] - Currency
		/// </summary>
		HourlyRate = 30,

		/// <summary>
		/// [Hours Worked] - Numeric
		/// </summary>
		HoursWorked = 31,

		/// <summary>
		/// [Incentive Disqualified] - Checkbox
		/// </summary>
		IncentiveDisqualified = 32,

		/// <summary>
		/// [Old Hourly Rate] - Currency
		/// </summary>
		OldHourlyRate = 33,

		/// <summary>
		/// [Original Or New] - Text (Multiple Choice)
		/// </summary>
		OriginalOrNew = 34,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric
		/// </summary>
		OtDtWotHours = 35,

		/// <summary>
		/// [OT DT WOT Rate] - Currency
		/// </summary>
		OtDtWotRate = 36,

		/// <summary>
		/// [Other Gross] - Currency
		/// </summary>
		OtherGross = 37,

		/// <summary>
		/// [Pay Type] - Text (Multiple Choice)
		/// </summary>
		PayType = 38,

		/// <summary>
		/// [PieceRate
		/// </summary>
		PieceRate = 39,

		/// <summary>
		/// [Pieces] - Numeric
		/// </summary>
		Pieces = 40,

		/// <summary>
		/// [Plant] - Numeric
		/// </summary>
		Plant = 41,

		/// <summary>
		/// [Shift Date] - Date
		/// </summary>
		ShiftDate = 42,

		/// <summary>
		/// [Start Time] - Date/Time
		/// </summary>
		StartTime = 44,

		/// <summary>
		/// [Use Old Hourly Rate] - Checkbox
		/// </summary>
		UseOldHourlyRate = 46,

		/// <summary>
		/// [Week End Date] - Formula Date
		/// </summary>
		WeekEndDate = 47,

		/// <summary>
		/// [Week End of Adjustment Paid] - Date
		/// </summary>
		WeekEndOfAdjustmentPaid = 48,

		/// <summary>
		/// [Source RID] - Numeric
		/// </summary>
		SourceRid = 49,

		/// <summary>
		/// [Layoff Pay] - Formula Checkbox
		/// </summary>
		LayoffPay = 50,

		/// <summary>
		/// [Sick Leave Requested] - Numeric
		/// </summary>
		SickLeaveRequested = 51,
	}
}
