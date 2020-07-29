using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IPlantPayrollRepo
	{
		public IEnumerable<PlantPayLine> Get(DateTime weekEndDate, int layoffId);
		public IEnumerable<PlantPayLine> GetForSummaries(DateTime weekEndDate, int layoffId);
		public XElement Save(IEnumerable<PlantPayLine> plantPayLines);
	}
}
