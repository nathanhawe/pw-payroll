using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IPlantPayrollOutRepo
	{
		public IEnumerable<PlantPayLine> GetForSummaries(DateTime weekEndDate, int layoffId);
		public void Save(IEnumerable<PlantPayLine> plantPayLines);
		public XElement Delete(DateTime weekEndDate, int layoffId);
	}
}
