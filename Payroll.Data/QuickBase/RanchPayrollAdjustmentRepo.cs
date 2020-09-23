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
	/// Repository that exposes query and persistence methods against the Ranch Payroll Adjustment table in Quick Base.
	/// </summary>
	public class RanchPayrollAdjustmentRepo : QuickBaseRepo<RanchAdjustmentLine>, IRanchPayrollAdjustmentRepo
	{
		public RanchPayrollAdjustmentRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Queries the Ranch Payroll Adjustment table in Quick Base for all records with the provided <c>weekEndOfAdjustmentPaidDate</c>.  
		/// If <c>layoffId</c> is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.
		/// </summary>
		/// <param name="weekEndOfAdjustmentPaidDate"></param>
		/// <param name="layoffId"></param>
		public IEnumerable<RanchAdjustmentLine> Get(DateTime weekEndOfAdjustmentPaidDate, int layoffId)
		{
			var formattedDate = weekEndOfAdjustmentPaidDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)RanchPayrollAdjustmentField.WeekEndOfAdjustmentPaid}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)RanchPayrollAdjustmentField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)RanchPayrollAdjustmentField.Layoff}.{ComparisonOperator.EX}.0}}";
			}

			var clist = GetDoQueryClist();
			var slist = $"{(int)RanchPayrollAdjustmentField.RecordId}";

			return base.Get(QuickBaseTable.RanchPayrollAdjustment, query, clist, slist, ConvertToRanchAdjustmentLines);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Ranch Payroll Adjustment table in Quickbase for the provided list of <c>RanchAdjustmentLine</c>s.
		/// Records with <c>QuickBaseRecordId</c> values greater than 0 will be updated while those with a value of 0 will be added new.
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
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{(line.Crew > 0 ? line.Crew.ToString() : "")},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{(line.BlockId > 0 ? line.BlockId.ToString() : "")},");
				sb.Append($"{line.PayType},");
				sb.Append($"{line.Pieces},");
				sb.Append($"{line.PieceRate},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"{line.WeekEndOfAdjustmentPaid:MM-dd-yyyy},");
				sb.Append($"{(line.IsOriginal ? OriginalOrNewValue.Original : OriginalOrNewValue.New)},");
				sb.Append($"{line.OldHourlyRate},");
				sb.Append($"{(line.UseOldHourlyRate ? "1" : "0")},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.TotalGross},");
				sb.Append($"{line.BatchId},");
				sb.Append($"{line.OtDtWotHours},");
				sb.Append($"{line.OtDtWotRate},");
				sb.Append($"{line.FiveEight}");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.RanchPayrollAdjustment,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}
			

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Ranch Payroll Adjustment table in Quick Base into 
		/// a collection of <c>RanchAdjustmentLine</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<RanchAdjustmentLine> ConvertToRanchAdjustmentLines(XElement doQuery)
		{
			var ranchAdjustmentLines = new List<RanchAdjustmentLine>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new RanchAdjustmentLine
				{
					QuickBaseRecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)RanchPayrollAdjustmentField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)RanchPayrollAdjustmentField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)RanchPayrollAdjustmentField.Crew: temp.Crew = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)RanchPayrollAdjustmentField.LaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.RelatedBlock: temp.BlockId = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.HoursWorked: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.PayType: temp.PayType = field.Value; break;
						case (int)RanchPayrollAdjustmentField.Pieces: temp.Pieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.PieceRate: temp.PieceRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.HourlyRate: temp.HourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.GrossFromHours: temp.GrossFromHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.GrossFromPieces: temp.GrossFromPieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.OtherGross: temp.OtherGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.AlternativeWorkWeek: 
							temp.AlternativeWorkWeek = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == AlternativeWorkWeekValue.FourTen.ToLower()) ? true : false);
							break;
						case (int)RanchPayrollAdjustmentField.FiveEight: temp.FiveEight = ParseBooleanFromCheckbox(field.Value); break;
						case (int)RanchPayrollAdjustmentField.EmployeeHourlyRate: temp.EmployeeHourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.WeekEndOfAdjustmentPaid: temp.WeekEndOfAdjustmentPaid = ParseDate(field.Value); break;
						case (int)RanchPayrollAdjustmentField.OriginalOrNew:
							temp.IsOriginal = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == OriginalOrNewValue.Original.ToLower() ? true : false));
							break;
						case (int)RanchPayrollAdjustmentField.OldHourlyRate: temp.OldHourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollAdjustmentField.UseOldHourlyRate: temp.UseOldHourlyRate = ParseBooleanFromCheckbox(field.Value); break;
					}
				}
				ranchAdjustmentLines.Add(temp);
			}

			return ranchAdjustmentLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Ranch Payroll Adjustment table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchPayrollAdjustmentField.LayoffRunId}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.WeekEndDate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.ShiftDate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.Crew}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.LaborCode}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.RelatedBlock}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.HoursWorked}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.PayType}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.Pieces}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.PieceRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OtherGross}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.AlternativeWorkWeek}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.EmployeeHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.WeekEndOfAdjustmentPaid}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OriginalOrNew}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OldHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.UseOldHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.FiveEight}.");


			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Ranch Payroll Adjustment table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchPayrollAdjustmentField.RecordId}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.LayoffRunId}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.ShiftDate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.Crew}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.LaborCode}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.RelatedBlock}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.PayType}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.Pieces}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.PieceRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OtherGross}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.WeekEndOfAdjustmentPaid}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OriginalOrNew}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OldHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.UseOldHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.CalculatedHourlyRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.CalculatedGrossFromHours}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.CalculatedGrossFromPieces}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.CalculatedTotalGross}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.BatchId}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OtDtWotHours}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.OtDtWotRate}.");
			sb.Append($"{(int)RanchPayrollAdjustmentField.FiveEight}.");

			return sb.ToString();
		}
	}
}
