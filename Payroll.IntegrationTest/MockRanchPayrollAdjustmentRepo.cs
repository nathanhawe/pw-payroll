using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockRanchPayrollAdjustmentRepo : Payroll.Data.QuickBase.IRanchPayrollAdjustmentRepo
	{
		public IEnumerable<RanchAdjustmentLine> Get(DateTime weekEndOfAdjustmentPaid, int layoffId)
		{
			throw new NotImplementedException();
		}

		public XElement Save(IEnumerable<RanchAdjustmentLine> ranchAdjustmentLines)
		{
			throw new NotImplementedException();
		}
	}
}
