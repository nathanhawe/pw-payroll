using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockRanchPayrollRepo : Payroll.Data.QuickBase.IRanchPayrollRepo
	{
		public IEnumerable<RanchPayLine> Get(DateTime weekEndDate, int layoffId)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<RanchPayLine> GetForSummaries(DateTime weekEndDate, int layoffId)
		{
			throw new NotImplementedException();
		}

		public XElement Lock(IEnumerable<RanchPayLine> ranchPayLines)
		{
			throw new NotImplementedException();
		}

		public XElement Save(IEnumerable<RanchPayLine> ranchPayLines)
		{
			throw new NotImplementedException();
		}

		public XElement SaveWithHoursWorked(IEnumerable<RanchPayLine> ranchPayLines)
		{
			throw new NotImplementedException();
		}

		public XElement Unlock(IEnumerable<RanchPayLine> ranchPayLines)
		{
			throw new NotImplementedException();
		}
	}
}
