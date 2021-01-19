using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockRanchSummariesRepo : Payroll.Data.QuickBase.IRanchSummariesRepo
	{
		public IEnumerable<RanchSummary> Get(DateTime weekEndDate)
		{
			throw new NotImplementedException();
		}

		public void Save(IEnumerable<RanchSummary> ranchSummaries)
		{
			throw new NotImplementedException();
		}
	}
}