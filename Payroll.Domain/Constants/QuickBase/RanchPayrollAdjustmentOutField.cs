namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Ranch Payroll Adjustment Out table in Quick Base.
	/// </summary>
	public enum RanchPayrollAdjustmentOutField
	{
		Unknown = 0,
		
		/// <summary>
		/// [Employee Number] - Text
		/// </summary>
		EmployeeNumber = 6,

		/// <summary>
		/// [Crew] - Numeric
		/// </summary>
		Crew = 14,

		/// <summary>
		/// [Labor Code] = 16
		/// </summary>
		LaborCode = 16,

		/// <summary>
		///  [Layoff Run ID] - Numeric
		/// </summary>
		LayoffRunId = 18,

		/// <summary>
		/// [Week End of Adjustment Paid] - Date indicating which payroll cycle the adjustment is being paid
		/// </summary>
		WeekEndOfAdjustmentPaid = 21,

		/// <summary>
		/// [Related Block] - Numeric
		/// </summary>
		RelatedBlock = 22,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 26,

		/// <summary>
		/// [End Time] - Numeric (but should be changed to be Time of Day)
		/// </summary>
		EndTime = 28,

		/// <summary>
		/// [FiveEight] - Checkbox indicating office work order
		/// </summary>
		FiveEight = 29,

		/// <summary>
		/// [Gross from Hours] - Currency
		/// </summary>
		GrossFromHours = 30,

		/// <summary>
		/// [Gross from Pieces] - Currency
		/// </summary>
		GrossFromPieces = 31,

		/// <summary>
		/// [Hourly Rate] - Currency
		/// </summary>
		HourlyRate = 32,

		/// <summary>
		/// [Hours Worked] - Numeric
		/// </summary>
		HoursWorked = 33,

		/// <summary>
		/// [Old Hourly Rate] - Currency
		/// </summary>
		OldHourlyRate = 34,

		/// <summary>
		/// [Original Or New] - Text
		/// </summary>
		OriginalOrNew = 35,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric
		/// </summary>
		OtDtWotHours = 36,

		/// <summary>
		/// [OT DT WOT Rate] - Currency
		/// </summary>
		OtDtWotRate = 37,

		/// <summary>
		/// [Other Gross] - Currency
		/// </summary>
		OtherGross = 38,

		/// <summary>
		/// [Pay Type] - Text
		/// </summary>
		PayType = 39,

		/// <summary>
		/// [Piece Rate] - Currency
		/// </summary>
		PieceRate = 40,

		/// <summary>
		/// [Pieces] - Numeric
		/// </summary>
		Pieces = 41,

		/// <summary>
		/// [Shift Date] - Date
		/// </summary>
		ShiftDate = 42,

		/// <summary>
		/// [Start Time] - Numeric (but should be changed to Time of Day)
		/// </summary>
		StartTime = 44,

		/// <summary>
		/// [Use Old Hourly Rate] - Checkbox
		/// </summary>
		UseOldHourlyRate = 46,

		/// <summary>
		/// [Source RID] - Numeric indicating the original Ranch Payroll Adjustment record ID if any
		/// </summary>
		SourceRid = 49,

		/// <summary>
		/// [Layoff Pay] - Formula Checkbox indicating if the record is part of a layoff pay run
		/// </summary>
		LayoffPay = 50,
	}
}
