using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockCrewBossPayRepo : Payroll.Data.QuickBase.ICrewBossPayRepo
	{
		public IEnumerable<CrewBossPayLine> Get(DateTime weekEndDate, int layoffId)
		{
			throw new NotImplementedException();
		}

		public XElement Save(IEnumerable<CrewBossPayLine> crewBossPayLines)
		{
			throw new NotImplementedException();
		}
	}
}
