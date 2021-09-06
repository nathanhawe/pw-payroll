using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class WeeklyOverTimeHours
	{
		public string EmployeeId { get; set; }
		public int Crew { get; set; }
		public DateTime WeekEndDate { get; set; }
		public decimal OverTimeHours { get; set; }
		public int BlockId { get; set; }
		public int LaborCode { get; set; }
	}
}
