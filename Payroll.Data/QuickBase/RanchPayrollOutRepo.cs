using Payroll.Domain;
using Payroll.Domain.Constants.QuickBase;
using QuickBase.Api;
using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	/// <summary>
	/// Repository that exposes query and persistence methods against the Ranch Payroll Out table in Quick Base.  The Ranch Payroll Out table
	/// is meant to contain the output from calculating the ranch payroll lines in Ranch Payroll.
	/// </summary>
	public class RanchPayrollOutRepo : QuickBaseRepo<RanchPayLine>, IRanchPayrollOutRepo
	{
		public RanchPayrollOutRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Ranch Payroll Out table in Quickbase for the provided list of <c>RanchPayLine</c>s.
		/// </summary>
		/// <param name="ranchPayLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<RanchPayLine> ranchPayLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string. This must match the field order returned by GetImportFromCsvList().
			var sb = new StringBuilder();
			foreach (var line in ranchPayLines)
			{
				sb.Append($"\"{line.EmployeeId}\",");
				sb.Append($"{(line.BlockId > 0 ? line.BlockId.ToString() : "")},");
				sb.Append($"{(line.Crew > 0 ? line.Crew.ToString() : "")},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{line.BatchId},");
				sb.Append($"\"{line.EndTime}\",");
				sb.Append($"{(line.FiveEight ? "1" : "0")},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{(line.HourlyRateOverride > 0 ? line.HourlyRateOverride.ToString() : "")},");
				sb.Append($"{line.OtDtWotHours},");
				sb.Append($"{line.OtDtWotRate},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"\"{line.PayType}\",");
				sb.Append($"{line.PieceRate},");
				sb.Append($"{line.Pieces},");
				sb.Append($"\"{line.StartTime}\",");
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{line.SickLeaveRequested},");

				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.RanchPayrollOut,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		/// <summary>
		/// Creates a new API_PurgeRecords request to the Ranch Payroll Out table in Quick Base based on the provided <c>weekEndDate</c> and <c>layoffId</c>.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		/// <returns></returns>
		public XElement Delete(DateTime weekEndDate, int layoffId)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)RanchPayrollOutField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)RanchPayrollOutField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)RanchPayrollOutField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var deleteResponse = _quickBaseConn.PurgeRecords(
				QuickBaseTable.RanchPayrollOut,
				query);

			return deleteResponse;
		}


		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Ranch Payroll Out table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchPayrollOutField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollOutField.RelatedBlock}.");
			sb.Append($"{(int)RanchPayrollOutField.Crew}.");
			sb.Append($"{(int)RanchPayrollOutField.LaborCode}.");
			sb.Append($"{(int)RanchPayrollOutField.LayoffRunId}.");
			sb.Append($"{(int)RanchPayrollOutField.ShiftDate}.");
			sb.Append($"{(int)RanchPayrollOutField.BatchId}.");
			sb.Append($"{(int)RanchPayrollOutField.EndTime}.");
			sb.Append($"{(int)RanchPayrollOutField.FiveEight}.");
			sb.Append($"{(int)RanchPayrollOutField.GrossFromHours}.");
			sb.Append($"{(int)RanchPayrollOutField.GrossFromPieces}.");
			sb.Append($"{(int)RanchPayrollOutField.HourlyRate}.");
			sb.Append($"{(int)RanchPayrollOutField.HourlyRateOverride}.");
			sb.Append($"{(int)RanchPayrollOutField.OtDtWotHours}.");
			sb.Append($"{(int)RanchPayrollOutField.OtDtWotRate}.");
			sb.Append($"{(int)RanchPayrollOutField.HoursWorked}.");
			sb.Append($"{(int)RanchPayrollOutField.OtherGross}.");
			sb.Append($"{(int)RanchPayrollOutField.PayType}.");
			sb.Append($"{(int)RanchPayrollOutField.PieceRate}.");
			sb.Append($"{(int)RanchPayrollOutField.Pieces}.");
			sb.Append($"{(int)RanchPayrollOutField.StartTime}.");
			sb.Append($"{(int)RanchPayrollOutField.SourceRid}.");
			sb.Append($"{(int)RanchPayrollOutField.SickLeaveRequested}.");

			return sb.ToString();
		}
	}
}
