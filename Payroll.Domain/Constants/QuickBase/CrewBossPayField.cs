namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Fields used in the Crew Boss Pay table in Quick Base
	/// </summary>
	public enum CrewBossPayField
	{
		Unknown = 0,
		LayoffRunId = 55,
		LayoffPay = 51,
		RecordId = 3,
		WeekEndDate = 35,
		ShiftDate = 9,
		Crew = 6,
		CBPayMethod = 12,
		CountOfWorkers = 10,
		HoursWorkedByCB = 13,
		HourlyRate = 45,
		TotalGross = 27,
		EmployeeNumber = 15,

		/// <summary>
		/// [Five Eight] - Checkbox
		/// </summary>
		FiveEight = 37,

		/// <summary>
		/// [Batch ID] - Numeric indicates the batch from time and attendance calculations that last processed this record.
		/// </summary>
		BatchId = 64,

		/// <summary>
		/// [Calculated Hourly Rate] - Currency indicates the selected hourly rate by the time and attendance calculations.
		/// </summary>
		CalculatedHourlyRate = 65,
		
		/// <summary>
		/// [Calculated Gross] - Currency indicates the final gross value for the crew boss pay line.
		/// </summary>
		CalculatedGross = 66,

		/// <summary>
		/// [High Heat Supplement] - Checkbox indicates that the record is for supplemental hours due to heat-related work stoppage.
		/// </summary>
		HighHeatSupplement = 75,

		/// <summary>
		/// [High Heat Supplement Total Hours Cap] - Numeric indicating the total number of hours guaranteed to be paid out when a high-heat supplement is required.
		/// </summary>
		HighHeatSupplementTotalHoursCap = 76,

		/// <summary>
		/// [Disqualify from Quality Bonus] - Checkbox indicates that the crew boss has been disqualified from the quality bonus.
		/// </summary>
		DisqualifyFromQualityBonus = 78,
	}
}
