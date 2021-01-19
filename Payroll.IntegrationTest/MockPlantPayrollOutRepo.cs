using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockPlantPayrollOutRepo : Data.QuickBase.IPlantPayrollOutRepo
	{
		public List<PlantPayLine> Input { get; set; }
		public List<PlantPayLine> Output { get; set; }

		public MockPlantPayrollOutRepo(List<PlantPayLine> input)
		{
			Input = input ?? throw new ArgumentNullException(nameof(input));
		}

		public XElement Delete(DateTime weekEndDate, int layoffId)
		{
			return null;
		}

		public void Save(IEnumerable<PlantPayLine> plantPayLines)
		{
			Output = plantPayLines.ToList();
		}

		public IEnumerable<PlantPayLine> GetForSummaries(DateTime weekEndDate, int layoffId)
		{
			throw new NotImplementedException();
		}
	}
}
