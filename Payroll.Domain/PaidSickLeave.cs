using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class PaidSickLeave : Record
	{
		public int BatchId { get; set; }
		public string EmployeeId { get; set; }
		public DateTime ShiftDate { get; set; }
		public string Company { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Hours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Gross { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal NinetyDayHours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal NinetyDayGross { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HoursUsed { get; set; }
	}
}
