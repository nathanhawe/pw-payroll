using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IRanchPayrollRepo
	{
		public IEnumerable<RanchPayLine> Get(DateTime weekEndDate, int layoffId);
		public XElement Save(IEnumerable<RanchPayLine> ranchPayLines);
		public XElement SaveWithHoursWorked(IEnumerable<RanchPayLine> ranchPayLines);
	}
}
