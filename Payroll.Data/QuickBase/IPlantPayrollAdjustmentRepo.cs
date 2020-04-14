using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IPlantPayrollAdjustmentRepo
	{
		public IEnumerable<PlantAdjustmentLine> Get(DateTime weekEndOfAdjustmentPaid, int layoffId);
		public XElement Save(IEnumerable<PlantAdjustmentLine> plantAdjustmentLines);
	}
}

