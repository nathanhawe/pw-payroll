using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IRanchPayrollAdjustmentRepo
	{
		public IEnumerable<RanchAdjustmentLine> Get(DateTime weekEndOfAdjustmentPaid, int layoffId);
		public XElement Save(IEnumerable<RanchAdjustmentLine> ranchAdjustmentLines);
	}
}
}
