namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Ranch Payroll Adjustment table in Quick Base.
	/// </summary>
	public enum RanchPayrollAdjustmentField
	{
		Unknown = 0,
		RecordId = 3,
		ShiftDate = 7,
		WeekEndDate = 9,

		/// <summary>
		/// [Start Time] - Numeric (but should be Time of Day to match Ranch Payroll)
		/// </summary>
		StartTime = 11,

		/// <summary>
		/// [End Time] - Numeric (but should be Time of Day to match Ranch Payroll)
		/// </summary>
		EndTime = 12,

		HoursWorked = 15,
		OtDtWotHours = 16,
		PayType = 17,
		Pieces = 18,
		PieceRate = 19,
		HourlyRate = 20,
		OtDtWotRate = 21,
		GrossFromPieces = 22,
		GrossFromHours = 23,
		OtherGross = 24,
		TotalGross = 25,
		OriginalOrNew = 27,
		WeekEndOfAdjustmentPaid = 28,
		EmployeeNumber = 29,
		AlternativeWorkWeek = 32,
		EmployeeHourlyRate = 33,
		Crew = 34,
		OldHourlyRate = 51,
		UseOldHourlyRate = 52,
		LaborCode = 59,
		LayoffRunId = 86,
		Layoff = 92,


		/// <summary>
		/// [Batch ID] - Numeric indicates the BatchId value during the last calculations this record was used in.
		/// </summary>
		BatchId = 98,

		/// <summary>
		/// [Calculated Hourly Rate] - Currency indicates the hourly rate determined by the time and attendance calculations.
		/// </summary>
		CalculatedHourlyRate = 99,

		/// <summary>
		/// [Calculated Gross From Hours] - Currency indicates the final gross from hours for this adjustment pay line.
		/// </summary>
		CalculatedGrossFromHours = 100,

		/// <summary>
		/// [Calculated Gross From Pieces] - Currency indicates the final gross from pieces for this adjustment pay line.
		/// </summary>
		CalculatedGrossFromPieces = 101,

		/// <summary>
		/// [Calculated Total Gross] - Currency indicates the final total gross value for this adjustment pay line.
		/// </summary>
		CalculatedTotalGross = 102,

		/// <summary>
		/// [FiveEight] - Checkbox indicates that the pay line should be paid as an office/clerical wage order.
		/// </summary>
		FiveEight = 103,

		/// <summary>
		/// [Related Block] - ID of related block in Block List.
		/// </summary>
		RelatedBlock = 106,

	}
}
