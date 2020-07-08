using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockPlantPayrollAdjustmentRepo : Payroll.Data.QuickBase.IPlantPayrollAdjustmentRepo
	{
		public List<PlantAdjustmentLine> Input { get; set; }
		public List<PlantAdjustmentLine> Output { get; set; }

		public MockPlantPayrollAdjustmentRepo(List<PlantAdjustmentLine> input)
		{
			Input = input ?? throw new ArgumentNullException(nameof(input));
		}
		public IEnumerable<PlantAdjustmentLine> Get(DateTime weekEndOfAdjustmentPaid, int layoffId)
		{
			return Input;
		}

		public XElement Save(IEnumerable<PlantAdjustmentLine> plantAdjustmentLines)
		{
			Output = plantAdjustmentLines.ToList();
			return null;
		}
	}
}