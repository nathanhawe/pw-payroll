using Payroll.Domain;
using System.Collections.Generic;

namespace Payroll.Data.QuickBase
{
	public interface IRanchBonusPieceRatesRepo
	{
		public IEnumerable<RanchBonusPieceRate> Get();
	}
}
