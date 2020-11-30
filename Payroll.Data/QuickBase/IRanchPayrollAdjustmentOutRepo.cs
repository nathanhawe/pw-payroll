using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IRanchPayrollAdjustmentOutRepo
	{
		public XElement Save(IEnumerable<RanchAdjustmentLine> ranchPayLines);
		public XElement Delete(DateTime weekEndOfAdjustmentPaidDate, int layoffId);
	}
}
