using System.Xml.Linq;

namespace QuickBase.Api
{
	public interface IQuickBaseConnection
	{
		public XElement DoQuery(
			string tableId,
			string query = null,
			string clist = null,
			string slist = null,
			string options = null,
			string format = null,
			bool returnPercentageAsString = false,
			bool includeRecordIds = false,
			bool? useFieldIds = null,
			string udata = null,
			bool queryIsName = false);

		public XElement DoQuery(
			string tableId,
			int qid,
			string clist = null,
			string slist = null,
			string options = null,
			string format = null,
			bool returnPercentageAsString = false,
			bool includeRecordIds = false,
			bool? useFieldIds = null,
			string udata = null);

		public XElement DoQueryCount(string tableId, string query, string udata = null);

		public XElement ImportFromCsv(
			string tableId,
			string cdata,
			string clist,
			string outputClist = null,
			bool percentageAsString = false,
			bool skipFirstRow = false,
			int mergeFieldId = 0,
			bool useUtcTime = false,
			string udata = null);

		public XElement PurgeRecords(
			string tableId,
			string query);
	}
}
