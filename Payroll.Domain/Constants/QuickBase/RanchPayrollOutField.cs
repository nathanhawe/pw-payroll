namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Ranch Payroll Out table in Quick Base.
	/// </summary>
	public enum RanchPayrollOutField
	{
		Unknown = 0,
		/// <summary>
		/// [Employee Number] - Text
		/// </summary>
		EmployeeNumber = 6,

		/// <summary>
		/// [Related Block] - Numeric
		/// </summary>
		RelatedBlock = 27,

		/// <summary>
		/// [Crew] - Numeric
		/// </summary>
		Crew = 40,

		/// <summary>
		/// [Labor Code] - Numeric
		/// </summary>
		LaborCode =	43,

		/// <summary>
		/// [Layoff Run ID] - Numeric
		/// </summary>
		LayoffRunId = 45,

		/// <summary>
		/// [Week End Date] - Formula Date
		/// </summary>
		WeekEndDate = 51,

		/// <summary>
		/// [Shift Date] - Date
		/// </summary>
		ShiftDate = 52,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 66,

		/// <summary>
		/// [End Time] - Time of Day
		/// </summary>
		EndTime = 68,

		/// <summary>
		/// [FiveEight] - Checkbox
		/// </summary>
		FiveEight = 69,

		/// <summary>
		/// [Gross from Hours] - Currency
		/// </summary>
		GrossFromHours = 70,

		/// <summary>
		/// [Gross from Pieces] - Currency
		/// </summary>
		GrossFromPieces = 71,

		/// <summary>
		/// [Hourly Rate] - Currency
		/// </summary>
		HourlyRate = 73,

		/// <summary>
		/// [Hourly Rate Override] - Currency
		/// </summary>
		HourlyRateOverride = 74,

		/// <summary>
		/// [Layoff Pay] - Formula Checkbox
		/// </summary>
		LayoffPay = 75,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric
		/// </summary>
		OtDtWotHours = 76,

		/// <summary>
		/// [OT DT WOT Rate] - Currency
		/// </summary>
		OtDtWotRate = 77,

		/// <summary>
		/// [Hours Worked] - Numeric
		/// </summary>
		HoursWorked = 78,

		/// <summary>
		/// [Other Gross] - Currency
		/// </summary>
		OtherGross = 79,

		/// <summary>
		///  [Pay Type] - Text
		/// </summary>
		PayType = 80,

		/// <summary>
		/// [Piece Rate] - Numeric
		/// </summary>
		PieceRate = 83,

		/// <summary>
		/// [Pieces] - Numeric
		/// </summary>
		Pieces = 84,

		/// <summary>
		/// [Start Time] - Time of Day
		/// </summary>
		StartTime = 86,

		/// <summary>
		/// [Source RID] - Numeric
		/// </summary>
		SourceRid = 93,
	}
}
