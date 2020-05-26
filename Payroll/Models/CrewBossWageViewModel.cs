using System;
using System.ComponentModel.DataAnnotations;

namespace Payroll.Models
{
	public class CrewBossWageViewModel
	{
		[Required]
		public DateTime EffectiveDate { get; set; }

		[Required]
		[Range(0, 50)]
		public decimal Wage { get; set; }

		[Required]
		[Range(0, 100)]
		public int WorkerCountThreshold { get; set; }
	}
}
