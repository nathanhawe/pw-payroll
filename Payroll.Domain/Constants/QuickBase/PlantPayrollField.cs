namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Plant Payroll table in Quick Base.
	/// </summary>
	public enum PlantPayrollField
	{
		/// <summary>
		/// Not mapped
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// [RecordID] - Built in Quick Base key field
		/// </summary>
		RecordId = 3,

		/// <summary>
		/// [Shift Date]
		/// </summary>
		ShiftDate = 7,

		/// <summary>
		/// [Plant]
		/// </summary>
		Plant = 8,
		
		/// <summary>
		/// [Hours Worked] - Numeric
		/// </summary>
		HoursWorked = 11,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric
		/// </summary>
		OtDtWotHours = 12,

		/// <summary>
		/// [Pay Type] - Text (multiple choice)
		/// </summary>
		PayType = 13,

		/// <summary>
		/// [Pieces] - Numeric
		/// </summary>
		Pieces = 16,

		/// <summary>
		/// [Hourly Rate] - Formula currency
		/// </summary>
		HourlyRate = 18,

		/// <summary>
		/// [OT DT WOT Rate] - Currency
		/// </summary>
		OtDtWotRate = 19,

		/// <summary>
		/// [Gross from Pieces] - Formula currency
		/// </summary>
		GrossFromPieces = 20,

		/// <summary>
		/// [Gross from Hours] - Formula currency
		/// </summary>
		GrossFromHours = 21,
		
		/// <summary>
		/// [Other Gross] - Currency
		/// </summary>
		OtherGross = 22,

		/// <summary>
		/// [Total Gross] - Formula currency
		/// </summary>
		TotalGross = 23,

		/// <summary>
		/// [Employee Number] - Foreign key to the Employee Master table.
		/// </summary>
		EmployeeNumber = 31,

		/// <summary>
		/// [Alternative Work Week] - Reference text from Employee Master that can indicate an alternative work week.
		/// </summary>
		AlternativeWorkWeek = 34,

		/// <summary>
		/// [Labor Code] - Foreign key to Master Pull Table: Labor Code Master
		/// </summary>
		LaborCode = 39,

		/// <summary>
		/// [Week End Date] - Formula date field that gives the date of the Sunday on or after [Shift Date].
		/// </summary>
		WeekEndDate = 44,

		/// <summary>
		/// [Employee Hourly Rate] - Reference currency from Employee Master that indicates a per-employee rate to factor in to rate selection.
		/// </summary>
		EmployeeHourlyRate = 47,

		/// <summary>
		/// [NonPrimaRate] - Currency that indicates the rate paid for pieces when a non-Prima violation has occurred.
		/// </summary>
		NonPrimaRate = 52,

		/// <summary>
		/// [PrimaRate] - Currency indicates the rate paid for pieces when there is no non-Prima violation.
		/// </summary>
		PrimaRate = 53,

		/// <summary>
		/// [IncreasedRate] - Currency indicates the rate paid for pieces when there is no non-Prima violation and [Increased Rate] is checked.
		/// </summary>
		IncreasedRate = 54,

		/// <summary>
		/// [NonPrimaViolation] - Text where "Yes" indicates that a non-Prima violation occured for this pay lines employee and commodity.
		/// </summary>
		NonPrimaViolation = 55,

		/// <summary>
		/// [Last Plant] - A related value from the Employee Master table.
		/// </summary>
		LastPlant = 57,

		/// <summary>
		/// [Gross from Incentive] - Formula currency
		/// </summary>
		GrossFromIncentive = 59,

		/// <summary>
		/// [Incentive Disqualified] - Checkbox indicates if a pay line is disqualified from an incentive that would otherwise be paid.
		/// </summary>
		IncentiveDisqualified = 60,

		/// <summary>
		/// [Audit Lock] - Multiple Choice Text indicates that a pay line has been "audit locked" and not editable by most users.
		/// </summary>
		AuditLock = 64,

		/// <summary>
		/// [Increased Rate] - Checkbox indicates that notwithstanding non-Primas the [IncreasedRate] should be used for pieces.
		/// </summary>
		UseIncreasedRate = 75,

		/// <summary>
		/// [Hourly Rate Override] - Currency used to override the hourly rate logic to a specific value.
		/// </summary>
		HourlyRateOverride = 98,

		/// <summary>
		/// [Lay Off Pay] - Formula checkbox that indicates the layoff run Id value is greater than 0.
		/// This field is necessary because [Lay Off Run ID] is optional and it's not possible to query
		/// for null field values in reports or through the API.
		/// </summary>
		LayoffPay = 153,

		/// <summary>
		/// [Lay Off Run ID] - Foreign key to the layoff runs table.
		/// </summary>
		LayoffRunId = 155,

		/// <summary>
		/// [H-2A] - Reference checkbox from Employee Master that indicates an H-2A employee.
		/// </summary>
		H2A = 163,

		/// <summary>
		/// [41.1 Approval] - Checkbox indicates that a pay line with pay type 41.1 is approved to be paid out.
		/// </summary>
		SpecialAdjustmentApproval = 182,

		/// <summary>
		/// [Non Discretionary Bonus Rate] - Currency indicates the amount of bonus pay given per hour for non-LC555 pay lines
		/// </summary>
		NonDiscretionaryBonusRate = 189,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 194,

		/// <summary>
		/// [Calculated Hourly Rate] - Currency
		/// </summary>
		CalculatedHourlyRate = 195,

		/// <summary>
		/// [Calculated Gross From Hours] - Currency
		/// </summary>
		CalculatedGrossFromHours = 196,

		/// <summary>
		/// [Calculated Gross From Pieces] - Currency
		/// </summary>
		CalculatedGrossFromPieces = 197,

		/// <summary>
		/// [Calculated Gross From Incentive] - Currency
		/// </summary>
		CalculatedGrossFromIncentive = 198,

		/// <summary>
		/// [Calculated Total Gross] - Currency
		/// </summary>
		CalculatedTotalGross = 199,

	}
}
