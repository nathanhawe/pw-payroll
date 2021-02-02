using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class SouthCrewBossWage : Record
	{
		public DateTime EffectiveDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Wage { get; set; }
		public int WorkerCountThreshold { get; set; }
	}
}
