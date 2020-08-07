namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Plant Summaries table in Quick Base.
	/// </summary>
	public enum PlantSummariesField
	{
		/// <summary>
		/// Field is not mapped
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// [Record ID#] - Built in Quick Base field
		/// </summary>
		RecordId = 3,

		/// <summary>
		/// [Employee Number] - Text foreign key to Employee Master table.
		/// </summary>
		EmployeeNumber = 6,

		/// <summary>
		/// [Total Hours] - Numeric
		/// </summary>
		TotalHours = 10,

		/// <summary>
		/// [Total Gross] - Currency
		/// </summary>
		TotalGross = 11,

		/// <summary>
		/// [Week End Date] - Date
		/// </summary>
		WeekEndDate = 12,

		/// <summary>
		/// [Lay Off Check] = Checkbox indicates this is a layoff check
		/// </summary>
		LayoffCheck = 82,

		/// <summary>
		/// [Layoff Run ID] - Numeric indicates the lay off run used to create this summary.
		/// </summary>
		LayoffRunId = 86,

		/// <summary>
		/// [LC 600 Hours] - Numeric
		/// </summary>
		LC600Hours = 95,

		/// <summary>
		/// [Batch ID] - Numeric
		/// </summary>
		BatchId = 97,
	}
}
