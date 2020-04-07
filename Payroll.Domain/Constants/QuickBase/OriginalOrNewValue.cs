using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants.QuickBase
{
	/// <summary>
	/// Valid values for the OriginalOrNew field in the Payroll: Ranch Payroll Adjustments and Payroll: Plant Payroll Adjustments tables in Quick Base.
	/// </summary>
	public static class OriginalOrNewValue
	{
		public const string Original = "Original";
		public const string New = "New";
	}
}
