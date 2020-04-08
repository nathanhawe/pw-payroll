using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class PlantSummary : Record
	{
		public int BatchId { get; set; }
		public int LayoffId { get; set; }
		public string EmployeeId { get; set; }
		public DateTime WeekEndDate { get; set; }
		public decimal TotalHours { get; set; }
		public decimal TotalGross { get; set; }
	}
}
