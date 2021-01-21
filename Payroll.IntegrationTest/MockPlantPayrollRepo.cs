using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockPlantPayrollRepo : Payroll.Data.QuickBase.IPlantPayrollRepo
	{
		public List<PlantPayLine> Input { get; set; }
		public List<PlantPayLine> Output { get; set; }

		public MockPlantPayrollRepo(List<PlantPayLine> input)
		{
			Input = input ?? throw new ArgumentNullException(nameof(input));
		}

		public IEnumerable<PlantPayLine> Get(DateTime weekEndDate, int layoffId)
		{
			return Input;
		}

		public XElement Save(IEnumerable<PlantPayLine> plantPayLines)
		{
			Output = plantPayLines.ToList();
			return null;
		}

		public IEnumerable<PlantPayLine> GetForSummaries(DateTime weekEndDate, int layoffId)
		{
			throw new NotImplementedException();
		}

		public void Lock(IEnumerable<PlantPayLine> plantPayLines)
		{
			throw new NotImplementedException();
		}

		public void Unlock(IEnumerable<PlantPayLine> plantPayLines)
		{
			throw new NotImplementedException();
		}
	}
}