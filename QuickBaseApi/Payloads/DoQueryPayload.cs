using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace QuickBase.Api.Payloads
{
	internal class DoQueryPayload : Payload
	{
		public string Query { get; set; }
		public string QueryName { get; set; }
		public int Qid { get; set; }
		public string Clist { get; set; }
		public string Slist { get; set; }
		public string Options { get; set; }
		public string Fmt { get; set; }
		public bool ReturnPercentageAsString { get; set; }
		public bool IncludeRecordIds { get; set; }
		public bool? UseFieldIds { get; set; }

		public DoQueryPayload(
			string userToken, 
			string query, 
			string clist,
			string slist,
			string options,
			string fmt,
			bool returnPercentageAsString,
			bool includeRecordIds,
			bool? useFieldIds = null,
			string udata = null, 
			bool queryIsName = false)
			: base(userToken, udata)
		{
			if (queryIsName)
			{
				QueryName = query;
			}
			else
			{
				Query = query;
			}

			Clist = clist;
			Slist = slist;
			Options = options;
			Fmt = fmt;
			ReturnPercentageAsString = returnPercentageAsString;
			IncludeRecordIds = includeRecordIds;
			UseFieldIds = useFieldIds;
		}

		public DoQueryPayload(
			string userToken,
			int qid,
			string clist,
			string slist,
			string options,
			string fmt,
			bool returnPercentageAsString,
			bool includeRecordIds,
			bool? useFieldIds = null,
			string udata = null)
			: base(userToken, udata)
		{
			Qid = qid;
			Clist = clist;
			Slist = slist;
			Options = options;
			Fmt = fmt;
			ReturnPercentageAsString = returnPercentageAsString;
			IncludeRecordIds = includeRecordIds;
			UseFieldIds = useFieldIds;
		}

		public override QuickBaseAction Action => Constants.QuickBaseAction.API_DoQuery;

		internal override string GetXmlString()
		{
			var qdbapi = GetBaseQdbapi();

			// Add query, qid, or qname
			if (!string.IsNullOrWhiteSpace(Query))
			{
				qdbapi.Add(new XElement("query", Query));
			}
			else if (!string.IsNullOrWhiteSpace(QueryName))
			{
				qdbapi.Add(new XElement("qname", QueryName));
			}
			else if (Qid > 0)
			{
				qdbapi.Add(new XElement("qid", Qid));
			}

			// Add clist
			if (!string.IsNullOrWhiteSpace(Clist)) qdbapi.Add(new XElement("clist", Clist));

			// Add slist
			if (!string.IsNullOrWhiteSpace(Slist)) qdbapi.Add(new XElement("slist", Slist));

			// Add fmt
			if (!string.IsNullOrWhiteSpace(Fmt)) qdbapi.Add(new XElement("fmt", Fmt));

			// Add returnpercentage
			qdbapi.Add(new XElement("returnpercentage", (ReturnPercentageAsString ? "1" : "0")));

			// Add options
			if (!string.IsNullOrWhiteSpace(Options)) qdbapi.Add(new XElement("options", Options));

			// Add includeRids
			qdbapi.Add(new XElement("includeRids", (IncludeRecordIds ? "1" : "0")));

			// Add useFids
			if (UseFieldIds.HasValue) qdbapi.Add(new XElement("useFids", (UseFieldIds.Value ? "1" : "0")));

			return qdbapi.ToString();
		}
	}
}
