using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants
{
	public enum AccessLevel
	{
		UNKNOWN = 0,
		Viewer,
		BatchCreator,
		Administrator
	}
}
