using System;

namespace Payroll.Domain
{
	public class RanchPayLine : Record
	{
		public int BatchId { get; set; }
		public int LayoffId { get; set; }
		public int QuickBaseRecordId { get; set; }
		public DateTime WeekEndDate { get; set; }
		public DateTime ShiftDate { get; set; }
		public int Crew { get; set; }
		public string EmployeeId { get; set; }
		public int LaborCode { get; set; }
		public int BlockId { get; set; }
		public decimal HoursWorked { get; set; }
		public string PayType { get; set; }
		public decimal Pieces { get; set; }
		public decimal PieceRate { get; set; }
		public decimal HourlyRate  {get; set;}
		public decimal OtDtWotRate { get; set; }
		public decimal OtDtWotHours { get; set; }
		public decimal GrossFromHours { get; set; }
		public decimal GrossFromPieces { get; set; }
		public decimal OtherGross { get; set; }
		public bool AlternativeWorkWeek { get; set; }
		public bool FiveEight { get; set; }
	}
}
