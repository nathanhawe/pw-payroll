using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockPlantPayrollAdjustmentOutRepo : Data.QuickBase.IPlantPayrollAdjustmentOutRepo
	{
		public List<PlantAdjustmentLine> Input { get; set; }
		public List<PlantAdjustmentLine> Output { get; set; }

		public MockPlantPayrollAdjustmentOutRepo(List<PlantAdjustmentLine> input)
		{
			Input = input ?? throw new ArgumentNullException(nameof(input));
		}

		public XElement Delete(DateTime weekEndDate, int layoffId)
		{
			return null;
		}

		public XElement Save(IEnumerable<PlantAdjustmentLine> plantAdjustmentLines)
		{
			Output = plantAdjustmentLines.ToList();
			return null;
		}
	}
}
