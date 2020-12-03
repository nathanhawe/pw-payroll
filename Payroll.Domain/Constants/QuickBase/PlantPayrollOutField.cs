namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Plant Payroll Out table in Quick Base.
	/// </summary>
	public enum PlantPayrollOutField
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
		LaborCode = 32,

		/// <summary>
		/// [Layoff Run ID] - Numeric
		/// </summary>
		LayoffRunId = 35,

		/// <summary>
		/// [Week End Date] - Formula Date
		/// </summary>
		WeekEndDate = 40,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 53,

		/// <summary>
		/// [Box Style] - Numeric
		/// </summary>
		BoxStyle = 56,

		/// <summary>
		/// [Box Style Description] - Text
		/// </summary>
		BoxStyleDescription = 57,

		/// <summary>
		/// [End Time] - Date/Time
		/// </summary>
		EndTime = 59,

		/// <summary>
		/// [Gross from Hours] - Currency
		/// </summary>
		GrossFromHours = 63,

		/// <summary>
		/// [Gross from Incentive] - Currency
		/// </summary>
		GrossFromIncentive = 64,

		/// <summary>
		/// [Gross from Pieces] - Currency
		/// </summary>
		GrossFromPieces = 65,

		/// <summary>
		/// [H-2A Hours Offered] - Numeric
		/// </summary>
		H2AHoursOffered = 66,

		/// <summary>
		/// [Hourly Rate] - Currency
		/// </summary>
		HourlyRate = 67,

		/// <summary>
		/// [Hourly Rate Override] - Currency
		/// </summary>
		HourlyRateOverride = 68,

		/// <summary>
		/// [Hours Worked] - Numeric
		/// </summary>
		HoursWorked = 69,

		/// <summary>
		/// [Incentive Disqualified] - Checkbox
		/// </summary>
		IncentiveDisqualified = 70,

		/// <summary>
		/// [Increased Rate] - Checkbox
		/// </summary>
		UseIncreasedRate = 71,

		/// <summary>
		/// [IncreasedRate] - Currency
		/// </summary>
		IncreasedRate = 72,

		/// <summary>
		/// [Layoff Pay] - Formula Checkbox indicates the payline is for a layoff run
		/// </summary>
		LayoffPay = 73,

		/// <summary>
		/// [NonPrimaRate] - Currency
		/// </summary>
		NonPrimaRate = 74,

		/// <summary>
		/// [NonPrimaViolation] - Text
		/// </summary>
		NonPrimaViolation = 75,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric
		/// </summary>
		OtDtWotHours = 76,

		/// <summary>
		/// [OT DT WOT Rate] - Currency
		/// </summary>
		OtDtWotRate = 77,

		/// <summary>
		/// [Other Gross] - Currency
		/// </summary>
		OtherGross = 78,

		/// <summary>
		/// [Pay Type] - Text (Multiple Choice)
		/// </summary>
		PayType = 79,

		/// <summary>
		/// [Pieces] - Numeric
		/// </summary>
		Pieces = 84,

		/// <summary>
		/// [Plant] - Numeric
		/// </summary>
		Plant = 85,

		/// <summary>
		/// [PrimaRate] - Currency
		/// </summary>
		PrimaRate = 86,

		/// <summary>
		/// [Shift Date] - Date
		/// </summary>
		ShiftDate = 92,

		/// <summary>
		/// [Start Time] - Date/Time
		/// </summary>
		StartTime = 94,

		/// <summary>
		/// [Source RID] - Numeric
		/// </summary>
		SourceRid = 96,

	}
}
