using System;
using System.ComponentModel.DataAnnotations;

namespace Payroll.Models
{
	public class MinimumWageViewModel
	{
		[Required]
		public DateTime EffectiveDate { get; set; }

		[Required]
		[Range(0, 50)]
		public decimal Wage { get; set; }
	}
}
