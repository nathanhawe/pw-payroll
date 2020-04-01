using Payroll.Domain;
using Payroll.Domain.Constants.QuickBase;
using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public class CrewBossPayRepo
	{
		private readonly IQuickBaseConnection _quickBaseConn;
		private readonly int _batchSize = 1000;

		public CrewBossPayRepo(IQuickBaseConnection quickBaseConnection)
		{
			_quickBaseConn = quickBaseConnection ?? throw new ArgumentNullException(nameof(quickBaseConnection));
		}

		public IEnumerable<CrewBossPayLine> Get(DateTime weekEndDate)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");
			var query = $"{{{(int)CrewBossPayField.WeekEndDate}.IR.'{formattedDate}'}}";
			return Get(query);
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
			for(int i = 0; i < count; i += _batchSize)
			{
				doQuery = _quickBaseConn.DoQuery(
					QuickBaseTable.CrewBossPay,
					query,
					clist,
					slist,
					options: $"num-{_batchSize}.skp-{(_batchSize * i)}.sortorder-A",
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

		public void Save(IEnumerable<CrewBossPayLine> crewBossPayLines)
		{
			throw new NotImplementedException();
		}
	}
}
