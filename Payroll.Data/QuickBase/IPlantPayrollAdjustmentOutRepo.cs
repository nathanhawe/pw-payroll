using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IPlantPayrollAdjustmentOutRepo
	{
		public XElement Save(IEnumerable<PlantAdjustmentLine> plantAdjustmentLines);
		public XElement Delete(DateTime weekEndDate, int layoffId);
	}
}
