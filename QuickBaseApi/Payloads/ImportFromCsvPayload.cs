using QuickBase.Api.Constants;
using System.Xml.Linq;

namespace QuickBase.Api.Payloads
{
	internal class ImportFromCsvPayload : Payload
	{
		public string Cdata { get; set; }
		public string Clist { get; set; }
		public string OutputClist { get; set; }
		public bool PercentageAsString { get; set; }
		public bool SkipFirstRow { get; set; }
		public int MergeFieldId { get; set; }
		public bool UseUtcTime { get; set; }


		public ImportFromCsvPayload(
			string userToken,
			string cdata,
			string clist,
			string outputClist,
			bool percentageAsString,
			bool skipFirstRow,
			int mergeFieldId,
			bool useUtcTime,
			string udata)
			: base(userToken, udata)
		{
			Cdata = cdata;
			Clist = clist;
			OutputClist = outputClist;
			PercentageAsString = percentageAsString;
			SkipFirstRow = skipFirstRow;
			MergeFieldId = mergeFieldId;
			UseUtcTime = useUtcTime;
		}

		public override QuickBaseAction Action => Constants.QuickBaseAction.API_ImportFromCSV;

		internal override string GetXmlString()
		{
			var qdbapi = GetBaseQdbapi();

			// Add records_csv
			qdbapi.Add(new XElement("records_csv", new XCData(Cdata)));

			// Add clist
			if (!string.IsNullOrWhiteSpace(Clist)) qdbapi.Add(new XElement("clist", Clist));

			// Add optional clist_output
			if (!string.IsNullOrWhiteSpace(OutputClist)) qdbapi.Add(new XElement("clist_output", OutputClist));

			// Add decimalPercent
			qdbapi.Add(new XElement("decimalPercent", (PercentageAsString ? "0" : "1")));

			// Add skipfirst
			qdbapi.Add(new XElement("skipfirst", (SkipFirstRow ? "1" : "0")));

			// Add mergeFieldId
			if(MergeFieldId > 0) qdbapi.Add(new XElement("mergeFieldId", MergeFieldId));

			// Add msInUTC
			qdbapi.Add(new XElement("msInUTC", (UseUtcTime ? "1" : "0")));

			return qdbapi.ToString();
		}
	}
}