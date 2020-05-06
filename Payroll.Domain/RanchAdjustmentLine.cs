using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class RanchAdjustmentLine : Record
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
		public DateTime WeekEndOfAdjustmentPaid { get; set; }
		public bool IsOriginal { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OldHourlyRate { get; set; }
		public bool UseOldHourlyRate { get; set; }
	}
}
