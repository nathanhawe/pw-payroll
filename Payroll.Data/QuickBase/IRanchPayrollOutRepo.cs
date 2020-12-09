using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IRanchPayrollOutRepo
	{
		public IEnumerable<RanchPayLine> GetForSummaries(DateTime weekEndDate, int layoffId);
		public XElement Save(IEnumerable<RanchPayLine> ranchPayLines);
		public XElement Delete(DateTime weekEndDate, int layoffId);
	}
}
