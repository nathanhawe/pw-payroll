using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
    public class PlantPayLine : Record
    {
		public int BatchId { get; set; }
		public int LayoffId { get; set; }
		public int QuickBaseRecordId { get; set; }
		public DateTime WeekEndDate { get; set; }
		public DateTime ShiftDate { get; set; }
		public int Plant { get; set; }
		public string EmployeeId { get; set; }
		public int LaborCode { get; set; }
		public decimal HoursWorked { get; set; }
		public string PayType { get; set; }
		public decimal Pieces { get; set; }
		public decimal PieceRate { get; set; }
		public decimal HourlyRate { get; set; }
		public decimal OtDtWotRate { get; set; }
		public decimal OtDtWotHours { get; set; }
		public decimal GrossFromHours { get; set; }
		public decimal GrossFromPieces { get; set; }
		public decimal OtherGross { get; set; }
		public decimal TotalGross { get; set; }
		public bool AlternativeWorkWeek { get; set; }
		public decimal HourlyRateOverride { get; set; }
		public decimal EmployeeHourlyRate { get; set; }
	}
}
