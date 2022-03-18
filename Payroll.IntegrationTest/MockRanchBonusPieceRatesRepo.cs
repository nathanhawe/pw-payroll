using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.IntegrationTest
{
	internal class MockRanchBonusPieceRatesRepo : Data.QuickBase.IRanchBonusPieceRatesRepo
	{
		public List<RanchBonusPieceRate> Input { get; set; }

		public MockRanchBonusPieceRatesRepo(List<RanchBonusPieceRate> input)
		{
			Input = input ?? throw new ArgumentNullException(nameof(input));
		}

		public IEnumerable<RanchBonusPieceRate> Get()
		{
			return Input;
		}
	}
}
