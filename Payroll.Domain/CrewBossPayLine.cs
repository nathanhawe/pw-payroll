using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Domain
{
	public class CrewBossPayLine : Record
	{
		public int BatchId { get; set; }
		public int LayoffId { get; set; }
		public int QuickBaseRecordId { get; set; }
		public DateTime WeekEndDate { get; set; }
		public DateTime ShiftDate { get; set; }
		public int Crew { get; set; }
		public string EmployeeId { get; set; }
		public string PayMethod { get; set; }
		public int WorkerCount { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HoursWorked { get; set; }
		
		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Gross { get; set; }
		public bool FiveEight { get; set; }

		public bool HighHeatSupplement { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HighHeatSupplementTotalHoursCap { get; set; }
	}
}
