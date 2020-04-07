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
	/// Repository that exposes query and persistence methods against the Ranch Payroll table in Quick Base.
	/// </summary>
	public class RanchPayrollRepo : QuickBaseRepo<RanchPayLine>
	{
		public RanchPayrollRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Queries the Ranch Payroll table in Quick Base for all records with the provided <c>weekEndDate</c>.  If <c>layoffId</c>
		/// is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		public IEnumerable<RanchPayLine> Get(DateTime weekEndDate, int layoffId)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)RanchPayrollField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)RanchPayrollField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)RanchPayrollField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var clist = GetDoQueryClist();
			var slist = $"{(int)RanchPayrollField.RecordId}";

			return base.Get(QuickBaseTable.RanchPayroll, query, clist, slist, ConvertToRanchPayLines);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Ranch Payroll table in Quickbase for the provided list of <c>RanchPayLine</c>s.
		/// Records with <c>QuickBaseRecordId</c> values greater than 0 will be updated while those with a value of 0 will be added new.
		/// </summary>
		/// <param name="ranchPayLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<RanchPayLine> ranchPayLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in ranchPayLines)
			{
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{(line.Crew > 0 ? line.Crew.ToString() : "")},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{(line.BlockId > 0 ? line.BlockId.ToString() : "")},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{line.PayType},");
				sb.Append($"{line.Pieces},");
				sb.Append($"{line.PieceRate},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"{line.TotalGross},");
				sb.Append($"{(line.FiveEight ? "1" : "0")},");
				sb.Append($"{line.HourlyRateOverride}");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.RanchPayroll,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}
			

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Ranch Payroll table in Quick Base into 
		/// a collection of <c>RanchPayLine</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<RanchPayLine> ConvertToRanchPayLines(XElement doQuery)
		{
			var ranchPayLines = new List<RanchPayLine>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new RanchPayLine
				{
					QuickBaseRecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)RanchPayrollField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)RanchPayrollField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)RanchPayrollField.Crew: temp.Crew = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollField.LastCrew: temp.LastCrew = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollField.EmployeeNumber: temp.EmployeeId = field.Value; break;
						case (int)RanchPayrollField.LaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollField.RelatedBlock: temp.BlockId = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollField.HoursWorked: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.PayType: temp.PayType = field.Value; break;
						case (int)RanchPayrollField.Pieces: temp.Pieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.PieceRate: temp.PieceRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.HourlyRate: temp.HourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.GrossFromHours: temp.GrossFromHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.GrossFromPieces: temp.GrossFromPieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.OtherGross: temp.OtherGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.AlternativeWorkWeek: 
							temp.AlternativeWorkWeek = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == AlternativeWorkWeekValue.FourTen.ToLower()) ? true : false);
							break;
						case (int)RanchPayrollField.FiveEight: temp.FiveEight = ParseBooleanFromCheckbox(field.Value); break;
						case (int)RanchPayrollField.HourlyRateOverride: temp.HourlyRateOverride = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollField.EmployeeHourlyRate: temp.EmployeeHourlyRate = ParseDecimal(field.Value) ?? 0; break;
					}
				}
				ranchPayLines.Add(temp);
			}

			return ranchPayLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Ranch Payroll table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchPayrollField.LayoffRunId}.");
			sb.Append($"{(int)RanchPayrollField.WeekEndDate}.");
			sb.Append($"{(int)RanchPayrollField.ShiftDate}.");
			sb.Append($"{(int)RanchPayrollField.Crew}.");
			sb.Append($"{(int)RanchPayrollField.LastCrew}.");
			sb.Append($"{(int)RanchPayrollField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollField.LaborCode}.");
			sb.Append($"{(int)RanchPayrollField.RelatedBlock}.");
			sb.Append($"{(int)RanchPayrollField.HoursWorked}.");
			sb.Append($"{(int)RanchPayrollField.PayType}.");
			sb.Append($"{(int)RanchPayrollField.Pieces}.");
			sb.Append($"{(int)RanchPayrollField.PieceRate}.");
			sb.Append($"{(int)RanchPayrollField.HourlyRate}.");
			sb.Append($"{(int)RanchPayrollField.GrossFromHours}.");
			sb.Append($"{(int)RanchPayrollField.GrossFromPieces}.");
			sb.Append($"{(int)RanchPayrollField.OtherGross}.");
			sb.Append($"{(int)RanchPayrollField.TotalGross}.");
			sb.Append($"{(int)RanchPayrollField.AlternativeWorkWeek}.");
			sb.Append($"{(int)RanchPayrollField.FiveEight}.");
			sb.Append($"{(int)RanchPayrollField.HourlyRateOverride}.");
			sb.Append($"{(int)RanchPayrollField.EmployeeHourlyRate}");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Ranch Payroll table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchPayrollField.RecordId}.");
			sb.Append($"{(int)RanchPayrollField.LayoffRunId}.");
			sb.Append($"{(int)RanchPayrollField.ShiftDate}.");
			sb.Append($"{(int)RanchPayrollField.Crew}.");
			sb.Append($"{(int)RanchPayrollField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollField.LaborCode}.");
			sb.Append($"{(int)RanchPayrollField.RelatedBlock}.");
			sb.Append($"{(int)RanchPayrollField.HoursWorked}.");
			sb.Append($"{(int)RanchPayrollField.PayType}.");
			sb.Append($"{(int)RanchPayrollField.Pieces}.");
			sb.Append($"{(int)RanchPayrollField.PieceRate}.");
			sb.Append($"{(int)RanchPayrollField.HourlyRate}.");
			sb.Append($"{(int)RanchPayrollField.GrossFromHours}.");
			sb.Append($"{(int)RanchPayrollField.GrossFromPieces}.");
			sb.Append($"{(int)RanchPayrollField.OtherGross}.");
			sb.Append($"{(int)RanchPayrollField.TotalGross}.");
			sb.Append($"{(int)RanchPayrollField.FiveEight}.");
			sb.Append($"{(int)RanchPayrollField.HourlyRateOverride}");

			return sb.ToString();
		}
	}
}
