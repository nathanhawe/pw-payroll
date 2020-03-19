using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class PaidSickLeave : Record
	{
		public int BatchId { get; set; }
		public string EmployeeId { get; set; }
		public DateTime ShiftDate { get; set; }
		public string Company { get; set; }
		public decimal Hours { get; set; }
		public decimal Gross { get; set; }
		public decimal NinetyDayHours { get; set; }
		public decimal NinetyDayGross { get; set; }
		public decimal HoursUsed { get; set; }
	}
}
