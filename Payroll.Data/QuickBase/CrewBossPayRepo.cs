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
	/// Repository that exposes query and persistence methods against the Crew Boss Pay table in Quick Base.
	/// </summary>
	public class CrewBossPayRepo : QuickBaseRepo<CrewBossPayLine>, ICrewBossPayRepo
	{

		public CrewBossPayRepo(IQuickBaseConnection quickBaseConnection)
			:base(quickBaseConnection) { }

		/// <summary>
		/// Queries the CrewBossPay table in Quick Base for all records with the provided <c>weekEndDate</c>.  If <c>layoffId</c>
		/// is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		/// <returns></returns>
		public IEnumerable<CrewBossPayLine> Get(DateTime weekEndDate, int layoffId)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");
			var query = $"{{{(int)CrewBossPayField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if(layoffId > 0)
			{
				query += $"AND{{{(int)CrewBossPayField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)CrewBossPayField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var clist = GetDoQueryClist();
			var slist = $"{(int)CrewBossPayField.RecordId}";

			return Get(QuickBaseTable.CrewBossPay, query, clist, slist, ConvertToCrewBossPayLines);
		}
				

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the CrewBossPay table in Quickbase for the provided list of <c>CrewBossPayLine</c>s.
		/// Records with <c>QuickBaseRecordId</c> values greater than 0 will be updated while those with a value of 0 will be added new.
		/// </summary>
		/// <param name="crewBossPayLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<CrewBossPayLine> crewBossPayLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string. This must match the order of fields returned from GetImportFromCsvClist().
			var sb = new StringBuilder();
			foreach (var line in crewBossPayLines)
			{
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{line.WorkerCount},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.Gross},");
				sb.Append($"{line.BatchId}\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.CrewBossPay,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}
		
		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Crew Boss Pay table in Quick Base into 
		/// a collection of <c>CrewBossPayLine</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<CrewBossPayLine> ConvertToCrewBossPayLines(XElement doQuery)
		{
			var crewBossPayLines = new List<CrewBossPayLine>();
			var records = doQuery.Elements("record");

			foreach(var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new CrewBossPayLine 
				{ 
					QuickBaseRecordId = recordId 
				};

				var fields = record.Elements("f");
				foreach(var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;
					
					switch (fieldId)
					{
						case (int)CrewBossPayField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)CrewBossPayField.CBPayMethod: temp.PayMethod = field.Value; break;
						//case (int)CrewBossPayField.CountOfWorkers: temp.WorkerCount = ParseInt(field.Value) ?? 0; break;
						case (int)CrewBossPayField.Crew: temp.Crew = ParseInt(field.Value) ?? 0; break;
						case (int)CrewBossPayField.HoursWorkedByCB: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)CrewBossPayField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)CrewBossPayField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)CrewBossPayField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)CrewBossPayField.FiveEight: temp.FiveEight = ParseBooleanFromCheckbox(field.Value); break;
					}
				}
				crewBossPayLines.Add(temp);
			}

			return crewBossPayLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for the API_DoQuery call to the Crew Boss Pay table in Quickbase.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)CrewBossPayField.EmployeeNumber}.");
			sb.Append($"{(int)CrewBossPayField.CBPayMethod}.");
			sb.Append($"{(int)CrewBossPayField.Crew}.");
			sb.Append($"{(int)CrewBossPayField.HoursWorkedByCB}.");
			sb.Append($"{(int)CrewBossPayField.LayoffRunId}.");
			sb.Append($"{(int)CrewBossPayField.ShiftDate}.");
			sb.Append($"{(int)CrewBossPayField.WeekEndDate}.");
			sb.Append($"{(int)CrewBossPayField.FiveEight}.");

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
			sb.Append($"{(int)CrewBossPayField.RecordId}.");
			sb.Append($"{(int)CrewBossPayField.CountOfWorkers}.");
			sb.Append($"{(int)CrewBossPayField.CalculatedHourlyRate}.");
			sb.Append($"{(int)CrewBossPayField.CalculatedGross}.");
			sb.Append($"{(int)CrewBossPayField.BatchId}");
			return sb.ToString();
		}
	}
}