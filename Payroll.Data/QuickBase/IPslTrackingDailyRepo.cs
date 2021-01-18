using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IPslTrackingDailyRepo
	{
		public IEnumerable<PaidSickLeave> Get(DateTime startDate, DateTime endDate, string company);
		public void Save(IEnumerable<PaidSickLeave> paidSickLeaves);
	}
}
