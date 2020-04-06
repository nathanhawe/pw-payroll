using Payroll.Domain;
using Payroll.Domain.Constants.QuickBase;
using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	/// <summary>
	/// Repository that exposes query and persistence methods against the Crew Boss Pay table in Quick Base.
	/// </summary>
	public class CrewBossPayRepo
	{
		private readonly IQuickBaseConnection _quickBaseConn;
		public int BatchSize { get; } = 1000;

		public CrewBossPayRepo(IQuickBaseConnection quickBaseConnection)
		{
			_quickBaseConn = quickBaseConnection ?? throw new ArgumentNullException(nameof(quickBaseConnection));
		}

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
			var query = $"{{{(int)CrewBossPayField.WeekEndDate}.IR.'{formattedDate}'}}";
			if(layoffId > 0)
			{
				query += $"AND{{{(int)CrewBossPayField.LayoffRunId}.EX.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)CrewBossPayField.LayoffPay}.EX.0}}";
			}

			return Get(query);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the CrewBossPay table in Quickbase for the provided list of <c>CrewBossPayLine</c>s.
		/// Records with <c>QuickBaseRecordId</c> values greater than 0 will be updated while those with a value of 0 will be added new.
		/// </summary>
		/// <param name="crewBossPayLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<CrewBossPayLine> crewBossPayLines)
		{
			var clist = $"{(int)CrewBossPayField.RecordId}.{(int)CrewBossPayField.LayoffRunId}.{(int)CrewBossPayField.ShiftDate}.{(int)CrewBossPayField.Crew}.{(int)CrewBossPayField.CBEmployeeNumber}.{(int)CrewBossPayField.CountOfWorkers}.{(int)CrewBossPayField.HoursWorkedByCB}.{(int)CrewBossPayField.HourlyRate}";

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in crewBossPayLines)
			{
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{line.Crew},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{line.WorkerCount},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{line.HourlyRate}\n");
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

		// ToDo: Refactor this method so that it can handle request too large, and server busy errors.
		private IEnumerable<CrewBossPayLine> Get(string query)
		{
			var crewBossPayLines = new List<CrewBossPayLine>();

			// Get the total query size so batching can be tracked
			XElement doQueryCount = _quickBaseConn.DoQueryCount(QuickBaseTable.CrewBossPay, query);
			if (!int.TryParse(doQueryCount.Element("numMatches")?.Value, out int count)) count = 0;

			// Download records in batches
			var clist = $"{(int)CrewBossPayField.CBEmployeeNumber}.{(int)CrewBossPayField.CBPayMethod}.{(int)CrewBossPayField.Crew}.{(int)CrewBossPayField.HoursWorkedByCB}.{(int)CrewBossPayField.LayoffRunId}.{(int)CrewBossPayField.ShiftDate}.{(int)CrewBossPayField.WeekEndDate}";
			var slist = $"{(int)CrewBossPayField.RecordId}";

			XElement doQuery;
			for(int i = 0; i < count; i += BatchSize)
			{
				doQuery = _quickBaseConn.DoQuery(
					QuickBaseTable.CrewBossPay,
					query,
					clist,
					slist,
					options: $"num-{BatchSize}.skp-{(BatchSize * i)}.sortorder-A",
					includeRecordIds: true,
					useFieldIds: true);

				// Convert XElement response to new domain objects
				crewBossPayLines.AddRange(ConvertToCrewBossPayLines(doQuery));
			}

			// Compare list with query count?
			// Return list
			return crewBossPayLines;
		}
		
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
						case (int)CrewBossPayField.CBEmployeeNumber: temp.EmployeeId = field.Value; break;
						case (int)CrewBossPayField.CBPayMethod: temp.PayMethod = field.Value; break;
						//case (int)CrewBossPayField.CountOfWorkers: temp.WorkerCount = ParseInt(field.Value) ?? 0; break;
						case (int)CrewBossPayField.Crew: temp.Crew = ParseInt(field.Value) ?? 0; break;
						case (int)CrewBossPayField.HoursWorkedByCB: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)CrewBossPayField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)CrewBossPayField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)CrewBossPayField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
					}
				}
				crewBossPayLines.Add(temp);
			}

			return crewBossPayLines;
		}

		private DateTime ParseDate(string value)
		{
			// Quick Base dates are returned as milliseconds since UNIX time in UTC.
			if (!long.TryParse(value, out long milliseconds)) milliseconds = 0;
			var offset = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
			return offset.UtcDateTime;
		}

		private decimal? ParseDecimal(string value)
		{
			if (!decimal.TryParse(value, out decimal convertedValue)) return null;
			return convertedValue;
		}

		private int? ParseInt(string value)
		{
			if (!int.TryParse(value, out int convertedValue)) return null;
			return convertedValue;
		}		
	}
}
