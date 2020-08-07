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
	/// Repository that exposes query and persistence methods against the Plant Summaries table in Quick Base.
	/// </summary>
	public class PlantSummariesRepo : QuickBaseRepo<PlantSummary>, IPlantSummariesRepo
	{
		public PlantSummariesRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection) { }

		/// <summary>
		/// Queries the Plant Summaries table in Quick Base for all records with the provided <c>weekEndDate</c>.
		/// </summary>
		/// <param name="weekEndDate"></param>
		public IEnumerable<PlantSummary> Get(DateTime weekEndDate)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PlantSummariesField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";

			var clist = GetDoQueryClist();
			var slist = $"{(int)PlantSummariesField.RecordId}";

			return base.Get(QuickBaseTable.PlantSummaries, query, clist, slist, ConvertToPlantSummaries);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Plant Summaries table in Quickbase to create new records for the provided list of <c>PlantSummary</c>s.
		/// </summary>
		/// <param name="plantSummaries"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<PlantSummary> plantSummaries)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string. Fields must match the order from GetImportFromCsvClist().
			var sb = new StringBuilder();
			foreach (var line in plantSummaries)
			{
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{line.WeekEndDate:MM-dd-yyyy},");
				sb.Append($"{line.TotalHours},");
				sb.Append($"{line.TotalGross},");
				sb.Append($"{line.CovidHours},");
				sb.Append($"{line.BatchId},");
				sb.Append($"{(line.LayoffId > 0 ? 1 : 0)}");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.PlantSummaries,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Plant Summaries table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantSummariesField.LayoffRunId}.");
			sb.Append($"{(int)PlantSummariesField.EmployeeNumber}.");
			sb.Append($"{(int)PlantSummariesField.WeekEndDate}.");
			sb.Append($"{(int)PlantSummariesField.TotalHours}.");
			sb.Append($"{(int)PlantSummariesField.TotalGross}.");
			sb.Append($"{(int)PlantSummariesField.LC600Hours}.");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Plant Summaries table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantSummariesField.LayoffRunId}.");
			sb.Append($"{(int)PlantSummariesField.EmployeeNumber}.");
			sb.Append($"{(int)PlantSummariesField.WeekEndDate}.");
			sb.Append($"{(int)PlantSummariesField.TotalHours}.");
			sb.Append($"{(int)PlantSummariesField.TotalGross}.");
			sb.Append($"{(int)PlantSummariesField.LC600Hours}.");
			sb.Append($"{(int)PlantSummariesField.BatchId}.");
			sb.Append($"{(int)PlantSummariesField.LayoffCheck}.");

			return sb.ToString();
		}

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Plant Summaries table in Quick Base into 
		/// a collection of <c>PlantSummary</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<PlantSummary> ConvertToPlantSummaries(XElement doQuery)
		{
			var PlantSummaries = new List<PlantSummary>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new PlantSummary();				

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)PlantSummariesField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)PlantSummariesField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)PlantSummariesField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)PlantSummariesField.TotalHours: temp.TotalHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantSummariesField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantSummariesField.LC600Hours: temp.CovidHours = ParseDecimal(field.Value) ?? 0; break;
					}
				}
				PlantSummaries.Add(temp);
			}

			return PlantSummaries;
		}
	}
}
