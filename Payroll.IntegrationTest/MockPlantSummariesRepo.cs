using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockPlantSummariesRepo : Payroll.Data.QuickBase.IPlantSummariesRepo
	{
		public List<PlantSummary> Input { get; set; } = new List<PlantSummary>();
		public List<PlantSummary> Output { get; set; }


		public IEnumerable<PlantSummary> Get(DateTime weekEndDate)
		{
			return Input;
		}

		public void Save(IEnumerable<PlantSummary> plantSummaries)
		{
			Output = plantSummaries.ToList();
		}
	}
}