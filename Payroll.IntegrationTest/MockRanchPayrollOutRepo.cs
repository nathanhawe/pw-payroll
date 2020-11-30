using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockRanchPayrollOutRepo : Payroll.Data.QuickBase.IRanchPayrollOutRepo
	{
		public XElement Delete(DateTime weekEndDate, int layoffId)
		{
			throw new NotImplementedException();
		}

		public XElement Save(IEnumerable<RanchPayLine> ranchPayLines)
		{
			throw new NotImplementedException();
		}
	}
}
