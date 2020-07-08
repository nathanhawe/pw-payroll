using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Payroll.IntegrationTest
{
	internal class MockPslTrackingDailyRepo : Payroll.Data.QuickBase.IPslTrackingDailyRepo
	{
		public List<PaidSickLeave> Input { get; set; }
		public List<PaidSickLeave> Output { get; set; }

		public MockPslTrackingDailyRepo(List<PaidSickLeave> input)
		{
			Input = input ?? throw new ArgumentNullException(nameof(input));
		}

		public IEnumerable<PaidSickLeave> Get(DateTime startDate, DateTime endDate, string company)
		{
			return Input;
		}

		public XElement Save(IEnumerable<PaidSickLeave> paidSickLeaves)
		{
			Output = paidSickLeaves.ToList();
			return null;
		}
	}
}
