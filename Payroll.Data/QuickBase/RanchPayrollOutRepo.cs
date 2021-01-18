using Payroll.Domain;
using Payroll.Domain.Constants.QuickBase;
using QuickBase.Api;
using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
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
		/// Queries the Ranch Payroll Out table in Quick Base for all records with the provided <c>weekEndDate</c>.  If <c>layoffId</c>
		/// is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.  This method uses CLIST and SLIST values specifically for
		/// the creation of ranch summaries.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		public IEnumerable<RanchPayLine> GetForSummaries(DateTime weekEndDate, int layoffId)
		{
			var clist = GetDoQueryClistForSummaries();
			var slist = $"{(int)RanchPayrollOutField.EmployeeNumber}";

			return Get(weekEndDate, layoffId, clist, slist);
		}

		private IEnumerable<RanchPayLine> Get(DateTime weekEndDate, int layoffId, string clist, string slist)
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

			return base.Get(QuickBaseTable.RanchPayrollOut, query, clist, slist, ConvertToRanchPayLines);
		}

		/// <summary>
		/// Creates new API_ImportFromCSV requests to the Ranch Payroll Out table in Quickbase for the provided list of <c>RanchPayLine</c>s.
		/// </summary>
		/// <param name="ranchPayLines"></param>
		/// <returns></returns>
		public void Save(IEnumerable<RanchPayLine> ranchPayLines)
		{
			for (int i = 0; i <= ranchPayLines.Count(); i += PostBatchSize)
			{
				ImportFromCsv(ranchPayLines.Skip(i).Take(PostBatchSize));
			}
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

		private XElement ImportFromCsv(IEnumerable<RanchPayLine> ranchPayLines)
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
				sb.Append($"{(line.IsLayoffTagFirstOfTwoInWeek ? "1" : "0") },");

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
		/// Converts an XElement object representing an API_DoQuery response from the Ranch Payroll Out table in Quick Base into 
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
						case (int)RanchPayrollOutField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)RanchPayrollOutField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)RanchPayrollOutField.Crew: temp.Crew = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollOutField.LastCrew: temp.LastCrew = ParseInt(field.Value) ?? 0; break;
						case (int)RanchPayrollOutField.HoursWorked: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollOutField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchPayrollOutField.LaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
					}
				}
				ranchPayLines.Add(temp);
			}

			return ranchPayLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Ranch Payroll Out table in Quick Base with
		/// a specific emphasis on fields necessary for the creation of ranch summaries.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClistForSummaries()
		{
			var sb = new StringBuilder();

			sb.Append($"{(int)RanchPayrollOutField.EmployeeNumber}.");
			sb.Append($"{(int)RanchPayrollOutField.WeekEndDate}.");
			sb.Append($"{(int)RanchPayrollOutField.Crew}.");
			sb.Append($"{(int)RanchPayrollOutField.LastCrew}.");
			sb.Append($"{(int)RanchPayrollOutField.HoursWorked}.");
			sb.Append($"{(int)RanchPayrollOutField.TotalGross}.");
			sb.Append($"{(int)RanchPayrollOutField.LaborCode}.");

			return sb.ToString();
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
			sb.Append($"{(int)RanchPayrollOutField.LayoffTagFirstOfTwoInWeek}.");

			return sb.ToString();
		}
	}
}
