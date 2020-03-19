using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class CrewBossWage : Record
	{
		public DateTime EffectiveDate { get; set; }
		public decimal Wage { get; set; }
		public int WorkerCountThreshold { get; set; }
	}
}
