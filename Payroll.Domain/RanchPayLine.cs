using System;
using System.ComponentModel.DataAnnotations.Schema;

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
		public int LastCrew { get; set; }
		public string EmployeeId { get; set; }
		public int LaborCode { get; set; }
		public int BlockId { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HoursWorked { get; set; }
		public string PayType { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Pieces { get; set; }

		[Column(TypeName = "decimal(18,16)")]
		public decimal PieceRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OtDtWotRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OtDtWotHours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal GrossFromHours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal GrossFromPieces { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OtherGross { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalGross { get; set; }
		public bool AlternativeWorkWeek { get; set; }
		public bool FiveEight { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyRateOverride { get; set; }
		
		[Column(TypeName = "decimal(18,2)")]
		public decimal EmployeeHourlyRate { get; set; }

		public string StartTime { get; set; }
		public string EndTime { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal SickLeaveRequested { get; set; }

		[NotMapped]
		public bool SpecialAdjustmentApproved { get; set; }
	}
}
