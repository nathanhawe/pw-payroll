using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class RanchSummary : Record
	{
		public int BatchId { get; set; }
		public int LayoffId { get; set; }
		public string EmployeeId { get; set; }
		public DateTime WeekEndDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalHours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalGross { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal CulturalHours { get; set; }

		public int LastCrew { get; set; }
		
		[Column(TypeName = "decimal(18,2)")]
		public decimal CovidHours { get; set; }
	}
}
