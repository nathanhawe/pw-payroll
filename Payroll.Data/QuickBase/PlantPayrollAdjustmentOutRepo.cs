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
	/// Repository that exposes query and persistence methods against the Plant Payroll Adjustment Out table in Quick Base.
	/// </summary>
	public class PlantPayrollAdjustmentOutRepo : QuickBaseRepo<PlantAdjustmentLine>, IPlantPayrollAdjustmentOutRepo
	{
		public PlantPayrollAdjustmentOutRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Plant Payroll Adjustment Out table in Quickbase for the provided list of <c>PlantAdjustmentLine</c>s.
		/// </summary>
		/// <param name="plantAdjustmentLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<PlantAdjustmentLine> plantAdjustmentLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in plantAdjustmentLines)
			{
				sb.Append($"\"{line.EmployeeId}\",");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.BatchId},");
				sb.Append($"{line.BoxStyle},");
				sb.Append($"\"{line.BoxStyleDescription}\",");
				sb.Append($"{line.EndTime:MM-dd-yyyy H:mm},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromIncentive},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.H2AHoursOffered},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{(line.IsIncentiveDisqualified ? "1" : "0")},");
				sb.Append($"{line.OldHourlyRate},");
				sb.Append($"{(line.IsOriginal ? OriginalOrNewValue.Original : OriginalOrNewValue.New)},");
				sb.Append($"{line.OtDtWotHours},");
				sb.Append($"{line.OtDtWotRate},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"\"{line.PayType}\",");
				sb.Append($"{line.PieceRate},");
				sb.Append($"{line.Pieces},");
				sb.Append($"{(line.Plant > 0 ? line.Plant.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{line.StartTime:MM-dd-yyyy H:mm},");
				sb.Append($"{(line.UseOldHourlyRate ? "1" : "0")},");
				sb.Append($"{line.WeekEndOfAdjustmentPaid:MM-dd-yyyy},");
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.PlantPayrollAdjustmentOut,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		/// <summary>
		/// Creates a new API_PurgeRecords request to the Plant Payroll Adjustment Out table in Quick Base based on the provided <c>weekEndDate</c> and <c>layoffId</c>.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		/// <returns></returns>
		public XElement Delete(DateTime weekEndDate, int layoffId)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PlantPayrollAdjustmentOutField.WeekEndOfAdjustmentPaid}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)PlantPayrollAdjustmentOutField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)PlantPayrollAdjustmentOutField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var deleteResponse = _quickBaseConn.PurgeRecords(
				QuickBaseTable.PlantPayrollAdjustmentOut,
				query);

			return deleteResponse;
		}


		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Plant Payroll Adjustment Out table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.LayoffRunId}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.BatchId}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.BoxStyle}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.BoxStyleDescription}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.EndTime}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.GrossFromHours}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.GrossFromIncentive}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.GrossFromPieces}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.H2AHoursOffered}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.HourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.HoursWorked}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.IncentiveDisqualified}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.OldHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.OriginalOrNew}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.OtDtWotHours}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.OtDtWotRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.OtherGross}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.PayType}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.PieceRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.Pieces}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.Plant}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.ShiftDate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.StartTime}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.UseOldHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.WeekEndOfAdjustmentPaid}.");
			sb.Append($"{(int)PlantPayrollAdjustmentOutField.SourceRid}.");

			return sb.ToString();
		}
	}
}
