using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class WeeklySummary
	{
		public string EmployeeId { get; set; }
		public int Crew { get; set; }
		public bool FiveEight { get; set; }
		public DateTime WeekEndDate { get; set; }
		public decimal MinimumWage { get; set; }
		public decimal TotalHours { get; set; }
		public decimal NonProductiveTime { get; set; }
		public decimal TotalGross { get; set; }
		public decimal EffectiveHourlyRate { get; set; }
		public decimal TotalOverTimeHours { get; set; }
		public decimal TotalDoubleTimeHours { get; set; }
		public decimal NonProductiveGross { get; set; }
	}
}
