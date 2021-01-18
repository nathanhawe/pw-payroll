using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public abstract class QuickBaseRepo<T>
	{
		protected readonly IQuickBaseConnection _quickBaseConn;
		public int GetBatchSize { get; set; } = 10000;
		public int PostBatchSize { get; set; } = 5000;

		public QuickBaseRepo(IQuickBaseConnection quickBaseConnection)
		{
			_quickBaseConn = quickBaseConnection ?? throw new ArgumentNullException(nameof(quickBaseConnection));
		}

		// ToDo: Refactor this method so that it can handle request too large, and server busy errors.
		protected IEnumerable<T> Get(
			string tableId,
			string query, 
			string clist,
			string slist,
			Func<XElement, IEnumerable<T>> convertMethod)
		{
			var responseList = new List<T>();

			// Get the total query size so batching can be tracked
			XElement doQueryCount = _quickBaseConn.DoQueryCount(tableId, query);
			if (!int.TryParse(doQueryCount.Element("numMatches")?.Value, out int count)) count = 0;

			// Download records in batches
			XElement doQuery;
			for (int i = 0; i < count; i += GetBatchSize)
			{
				doQuery = _quickBaseConn.DoQuery(
					tableId,
					query,
					clist,
					slist,
					options: $"num-{GetBatchSize}.skp-{(i)}.sortorder-A",
					includeRecordIds: true,
					useFieldIds: true);

				// Convert XElement response to new domain objects
				responseList.AddRange(convertMethod(doQuery));
			}

			// Compare list with query count?
			// Return list
			return responseList;
		}


		protected DateTime ParseDate(string value)
		{
			// Quick Base dates are returned as milliseconds since UNIX time in UTC.
			if (!long.TryParse(value, out long milliseconds)) milliseconds = 0;
			var offset = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
			return offset.UtcDateTime;
		}

		protected decimal? ParseDecimal(string value)
		{
			if (!decimal.TryParse(value, out decimal convertedValue)) return null;
			return convertedValue;
		}

		protected int? ParseInt(string value)
		{
			if (!int.TryParse(value, out int convertedValue)) return null;
			return convertedValue;
		}

		protected bool ParseBooleanFromCheckbox(string value)
		{
			if (!string.IsNullOrWhiteSpace(value) && (value.Trim() == "1" || value.Trim().ToLower() == "yes")) return true;
			return false;
		}

		protected string ParseTimeOfDay(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) return null;

			// Quickbase returns milliseconds since 12:00AM to represent time of day but expects
			// HH:MM (AM|PM) for posting back.
			if (!long.TryParse(value, out long milliseconds)) milliseconds = 0;
			var totalMinutes = milliseconds / 60000;
			var hourPart = totalMinutes / 60;
			var minutePart = totalMinutes % 60;

			return $"{(hourPart < 10 ? "0":"")}{hourPart}:{(minutePart < 10 ? "0":"")}{minutePart}";
		}
	}
}
