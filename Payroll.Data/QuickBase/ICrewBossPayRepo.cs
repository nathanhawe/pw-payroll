using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface ICrewBossPayRepo
	{
		public IEnumerable<CrewBossPayLine> Get(DateTime weekEndDate, int layoffId);
		public XElement Save(IEnumerable<CrewBossPayLine> crewBossPayLines);
	}
}
