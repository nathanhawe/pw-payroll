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
	/// Repository that exposes query and persistence methods against the Ranch Payroll Adjustment Out table in Quick Base.
	/// </summary>
	public class RanchPayrollAdjustmentOutRepo : QuickBaseRepo<RanchAdjustmentLine>, IRanchPayrollAdjustmentOutRepo
	{
		public RanchPayrollAdjustmentOutRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Ranch Payroll Adjustment table in Quickbase for the provided list of <c>RanchAdjustmentLine</c>s.
		/// </summary>
		/// <param name="ranchAdjustmentLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<RanchAdjustmentLine> ranchAdjustmentLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in ranchAdjustmentLines)
			{
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{(line.Crew > 0 ? line.Crew.ToString() : "")},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.WeekEndOfAdjustmentPaid:MM-dd-yyyy},");
				sb.Append($"{(line.BlockId > 0 ? line.BlockId.ToString() : "")},");
				sb.Append($"{line.BatchId},");
				sb.Append($"{line.EndTime},");
				sb.Append($"{line.FiveEight},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{line.OldHourlyRate},");
				sb.Append($"{(line.IsOriginal ? OriginalOrNewValue.Original : OriginalOrNewValue.New)},");
				sb.Append($"{line.OtDtWotHours},");
				sb.Append($"{line.OtDtWotRate},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"{line.PayType},");
				sb.Append($"{line.PieceRate},");
				sb.Append($"{line.Pieces},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{line.StartTime},");
				sb.Append($"{(line.UseOldHourlyRate ? "1" : "0")},");
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.RanchPayrollAdjustmentOut,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		/// <summary>
		/// Creates a new API_PurgeRecords request to the Ranch Payroll Adjustment Out table in Quick Base based on the 
		/// provided <c>weekEndOfAdjustmentPaidDate</c> and <c>layoffId</c>.
		/// </summary>
		/// <param name="weekEndOfAdjustmentPaidDate"></param>
		/// <param name="layoffId"></param>
		/// <returns></returns>
		public XElement Delete(DateTime weekEndOfAdjustmentPaidDate, int layoffId)
		{
			var formattedDate = weekEndOfAdjustmentPaidDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)RanchPayrollAdjustmentOutField.WeekEndOfAdjustmentPaid}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)RanchPayrollAdjustmentOutField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)RanchPayrollAdjustmentOutField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var deleteResponse = _quickBaseConn.PurgeRecords(
				QuickBaseTable.RanchPayrollAdjustmentOut,
				query);

			return deleteResponse;
		}


		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Ranch Payroll Adjustment Out table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.Crew}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.LaborCode}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.LayoffRunId}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.WeekEndOfAdjustmentPaid}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.RelatedBlock}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.BatchId}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.EndTime}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.FiveEight}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.GrossFromHours}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.GrossFromPieces}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.HourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.HoursWorked}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.OldHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.OriginalOrNew}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.OtDtWotHours}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.OtDtWotRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.OtherGross}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.PayType}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.PieceRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.Pieces}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.ShiftDate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.StartTime}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.UseOldHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentOutField.SourceRid}.");

			return sb.ToString();
		}
	}
}
