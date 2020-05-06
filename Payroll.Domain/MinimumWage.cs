using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class MinimumWage : Record
	{
		[Column(TypeName = "decimal(18,2)")]
		public decimal Wage { get; set; }
		public DateTime EffectiveDate { get; set; }
	}
}
