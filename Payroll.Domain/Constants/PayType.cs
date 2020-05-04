using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants
{
	public static class PayType
	{
		public const string Regular = "1-Regular";
		public const string OverTime = "2-OT";
		public const string DoubleTime = "3-DT";
		public const string Pieces = "4-Pieces";
		public const string HourlyPlusPieces = "4.1-Hourly plus Pieces";
		public const string ProductivityOnlyPieces = "4.2-Pieces-Productivity Only - No Piece Rate";
		public const string MinimumAssurance = "5-Minimum Assurance";
		public const string MinimumAssurance_Regular = "5.1-Minimum Assurance Reg";
		public const string MinimumAssurance_OverTime = "5.2-Minimum Assurance OT";
		public const string MinimumAssurance_DoubleTime = "5.3-Minimum Assurance DT";
		public const string MinimumAssurance_WeeklyOverTime = "5.3-Minimum Assurance WOT";
		public const string Vacation = "7-Vacation";
		public const string Holiday = "7.1-Holiday";
		public const string SickLeave = "7.2-Sick Leave";
		public const string Bereavement = "7.3-Bereavement";
		public const string CBDaily = "8.1-CB Daily Rate";
		public const string CBPerWorker = "8.2-CB Per Worker Rate";
		public const string CBHourlyTrees = "8.3-CB Hourly Trees";
		public const string CBHourlyVines = "8.4-CB Hourly Vines";
		public const string CBSouthDaily = "8.5-CB South Daily";
		public const string CBSouthHourly = "8.6-CB South Hourly";
		public const string CBCommission = "9-Commission";
		public const string WeeklyOverTime = "40-Weekly OT";
		public const string Adjustment = "41-Adjustment";
		public const string SpecialAdjustment = "41.1-Adjustment";
		public const string CompTime = "48-Comp Time";
		public const string ReportingPay = "49-Reporting Pay";
		public const string ReportingPayAdjustment = "49.1-Reporting Pay Adjustment";
		public const string Bonus = "42-Bono";
	}
}
