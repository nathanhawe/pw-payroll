namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Plant Payroll Adjustment table in Quick Base.
	/// </summary>
	public enum PlantPayrollAdjustmentField
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
		/// [Lay Off Pay] - Formula checkbox that indicates the layoff run Id value is greater than 0.
		/// This field is necessary because [Lay Off Run ID] is optional and it's not possible to query
		/// for null field values in reports or through the API.
		/// </summary>
		LayoffPay = 82,

		/// <summary>
		/// [Lay Off Run ID] - Foreign key to the layoff runs table.
		/// </summary>
		LayoffRunId = 78,

		/// <summary>
		/// [Week End Date] - Formula date field that gives the date of the Sunday on or after [Shift Date].
		/// </summary>
		WeekEndDate = 17,

		/// <summary>
		/// [Shift Date] - Date
		/// </summary>
		ShiftDate = 15,

		/// <summary>
		/// [Plant] - Numeric
		/// </summary>
		Plant = 19,

		/// <summary>
		/// [Employee Number] - Text foreign key to the Employee Master table.
		/// </summary>
		EmployeeNumber = 6,

		/// <summary>
		/// [Labor Code] - Numeric foreign key to Master Pull Table: Labor Code Master
		/// </summary>
		LaborCode = 12,

		/// <summary>
		/// [Hours Worked] - Numeric
		/// </summary>
		HoursWorked = 22,

		/// <summary>
		/// [OT DT WOT Hours] - Numeric
		/// </summary>
		OtDtWotHours = 23,

		/// <summary>
		/// [Pay Type] - Text (multiple choice)
		/// </summary>
		PayType = 24,

		/// <summary>
		/// [Pieces] - Numeric
		/// </summary>
		Pieces = 27,

		/// <summary>
		/// [Piece Rate] - Currency indicates the rate paid for the pieces in this adjustment record.  Note: this
		/// is different from Plant Payroll in that the creator of the adjustment record needs to determine the
		/// rate paid instead of the application making that determination.
		/// </summary>
		PieceRate = 28,

		/// <summary>
		/// [OT DT WOT Rate] - Currency
		/// </summary>
		OtDtWotRate = 29,

		/// <summary>
		/// [Hourly Rate] - Formula currency
		/// </summary>
		HourlyRate = 39,

		/// <summary>
		/// [Gross from Hours] - Formula currency
		/// </summary>
		GrossFromHours = 31,

		/// <summary>
		/// [Gross from Pieces] - Formula currency
		/// </summary>
		GrossFromPieces = 30,

		/// <summary>
		/// [Gross from Incentive] - Currency
		/// </summary>
		GrossFromIncentive = 56,

		/// <summary>
		/// [Other Gross] - Currency
		/// </summary>
		OtherGross = 32,

		/// <summary>
		/// [Total Gross] - Formula currency
		/// </summary>
		TotalGross = 33,

		/// <summary>
		/// [Alternative Work Week] - Reference text from Employee Master that can indicate an alternative work week.
		/// </summary>
		AlternativeWorkWeek = 11,

		/// <summary>
		/// [Employee Hourly Rate] - Reference currency from Employee Master that indicates a per-employee rate to factor in to rate selection.
		/// </summary>
		EmployeeHourlyRate = 41,

		/// <summary>
		/// [H-2A] - Reference checkbox from Employee Master that indicates an H-2A employee.
		/// </summary>
		H2A = 84,

		/// <summary>
		/// [Week End of Adjustment Paid] - Date that indicates which week ending the adjustment is being paid in.
		/// </summary>
		WeekEndOfAdjustmentPaid = 36,

		/// <summary>
		/// [Original or New] - Text (multiple choice) indicates whether the adjustment line reflects the original or new value.
		/// </summary>
		OriginalOrNew = 35,

		/// <summary>
		/// [Old Hourly Rate] - Currency indicates the original hourly rate.  Can also be used as [Hourly Rate Override] in other 
		/// tables to force a specific rate.
		/// </summary>
		OldHourlyRate = 48,

		/// <summary>
		/// [Use Old Hourly Rate] - Checkbox indicates that the value of [Old Hourly Rate] should be used when calculating the effective
		/// hourly rate.
		/// </summary>
		UseOldHourlyRate = 49,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 88,

		/// <summary>
		/// [Calculated Hourly Rate] - Currency
		/// </summary>
		CalculatedHourlyRate = 89,

		/// <summary>
		/// [Calculated Gross From Hours] - Currency
		/// </summary>
		CalculatedGrossFromHours = 90,

		/// <summary>
		/// [Calculated Gross From Pieces] - Currency
		/// </summary>
		CalculatedGrossFromPieces = 91,

		/// <summary>
		/// [Calculated Total Gross] - Currency
		/// </summary>
		CalculatedTotalGross = 92,
	}
}
