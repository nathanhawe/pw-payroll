using System.Xml.Linq;

namespace QuickBase.Api
{
	public interface IQuickBaseConnection
	{
		public XElement DoQuery(
			string applicationId,
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
			string applicationId,
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
		
	}
}
