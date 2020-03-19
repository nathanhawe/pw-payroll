using System;

namespace Payroll.Domain
{
	public class Batch : Record
	{
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string State { get; set; }
		public bool IsComplete { get; set; }
		public string Owner { get; set; }
	}
}
