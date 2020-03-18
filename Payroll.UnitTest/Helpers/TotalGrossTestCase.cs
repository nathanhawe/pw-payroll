using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Helpers
{
	public class TotalGrossTestCase
	{
		public int Id { get; set; }
		public decimal GrossFromHours { get; set; }
		public decimal GrossFromPieces { get; set; }
		public decimal OtherGross { get; set; }
		public decimal GrossFromIncentive { get; set; }
		public decimal ExpectedTotalGross { get; set; }
	}
}
