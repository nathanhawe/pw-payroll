using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockRanchPayrollAdjustmentOutRepo : Payroll.Data.QuickBase.IRanchPayrollAdjustmentOutRepo
	{
		public XElement Delete(DateTime weekEndOfAdjustmentPaidDate, int layoffId)
		{
			throw new NotImplementedException();
		}

		public void Save(IEnumerable<RanchAdjustmentLine> ranchPayLines)
		{
			throw new NotImplementedException();
		}
	}
}
