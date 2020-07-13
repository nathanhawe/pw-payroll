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
	/// Repository that exposes query and persistence methods against the Ranch Summaries table in Quick Base.
	/// </summary>
	public class RanchSummariesRepo : QuickBaseRepo<RanchSummary>, IRanchSummariesRepo
	{
		public RanchSummariesRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection) { }

		/// <summary>
		/// Queries the Ranch Summaries table in Quick Base for all records with the provided <c>weekEndDate</c>.
		/// </summary>
		/// <param name="weekEndDate"></param>
		public IEnumerable<RanchSummary> Get(DateTime weekEndDate)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)RanchSummariesField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";

			var clist = GetDoQueryClist();
			var slist = $"{(int)RanchSummariesField.RecordId}";

			return base.Get(QuickBaseTable.RanchSummaries, query, clist, slist, ConvertToRanchSummaries);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Ranch Summaries table in Quickbase to create new records for the provided list of <c>RanchSummary</c>s.
		/// </summary>
		/// <param name="ranchSummaries"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<RanchSummary> ranchSummaries)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string.  This must match the order of fields as returned by GetImportFromCsvClist().
			var sb = new StringBuilder();
			foreach (var line in ranchSummaries)
			{
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{line.WeekEndDate:MM-dd-yyyy},");
				sb.Append($"{line.TotalHours},");
				sb.Append($"{line.TotalGross},");
				sb.Append($"{line.CulturalHours},");
				sb.Append($"{(line.LastCrew > 0 ? line.LastCrew.ToString() : "")},");
				sb.Append($"{line.CovidHours},");
				sb.Append($"{line.BatchId},");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.RanchSummaries,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Ranch Summaries table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchSummariesField.LayoffRunId}.");
			sb.Append($"{(int)RanchSummariesField.EmployeeNumber}.");
			sb.Append($"{(int)RanchSummariesField.WeekEndDate}.");
			sb.Append($"{(int)RanchSummariesField.TotalHours}.");
			sb.Append($"{(int)RanchSummariesField.TotalGross}.");
			sb.Append($"{(int)RanchSummariesField.CulturalHours}.");
			sb.Append($"{(int)RanchSummariesField.LastCrew}.");
			sb.Append($"{(int)RanchSummariesField.LC600Hours}.");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Ranch Summaries table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchSummariesField.LayoffRunId}.");
			sb.Append($"{(int)RanchSummariesField.EmployeeNumber}.");
			sb.Append($"{(int)RanchSummariesField.WeekEndDate}.");
			sb.Append($"{(int)RanchSummariesField.TotalHours}.");
			sb.Append($"{(int)RanchSummariesField.TotalGross}.");
			sb.Append($"{(int)RanchSummariesField.CulturalHours}.");
			sb.Append($"{(int)RanchSummariesField.LastCrew}.");
			sb.Append($"{(int)RanchSummariesField.LC600Hours}.");
			sb.Append($"{(int)RanchSummariesField.BatchId}.");

			return sb.ToString();
		}

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Ranch Summaries table in Quick Base into 
		/// a collection of <c>RanchSummary</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<RanchSummary> ConvertToRanchSummaries(XElement doQuery)
		{
			var ranchSummaries = new List<RanchSummary>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new RanchSummary();				

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)RanchSummariesField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)RanchSummariesField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)RanchSummariesField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)RanchSummariesField.TotalHours: temp.TotalHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchSummariesField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchSummariesField.CulturalHours: temp.CulturalHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchSummariesField.LastCrew: temp.LastCrew = ParseInt(field.Value) ?? 0; break;
						case (int)RanchSummariesField.LC600Hours: temp.CovidHours = ParseDecimal(field.Value) ?? 0; break;
					}
				}
				ranchSummaries.Add(temp);
			}

			return ranchSummaries;
		}
	}
}
