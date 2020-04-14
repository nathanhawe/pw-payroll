using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IRanchSummariesRepo
	{
		public IEnumerable<RanchSummary> Get(DateTime weekEndDate);
		public XElement Save(IEnumerable<RanchSummary> ranchSummaries);
	}
}
