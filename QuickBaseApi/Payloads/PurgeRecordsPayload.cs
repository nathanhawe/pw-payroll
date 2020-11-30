using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace QuickBase.Api.Payloads
{
	internal class PurgeRecordsPayload : Payload
	{
		public string Query { get; set; }
		
		public PurgeRecordsPayload(
			string userToken, 
			string query, 
			string udata = null)
			: base(userToken, udata)
		{
			Query = query;
		}
				
		public override QuickBaseAction Action => Constants.QuickBaseAction.API_PurgeRecords;

		public override string ToString()
		{
			return $"PurgeRecordsPayload - action:'{Action}', udata:'{Udata}', query:'{Query}'.";
		}

		internal override string GetXmlString()
		{
			var qdbapi = GetBaseQdbapi();

			// Add query, qid, or qname
			if (!string.IsNullOrWhiteSpace(Query))
			{
				qdbapi.Add(new XElement("query", Query));
			}
			else
			{
				// Prevent an empty query from purging the table.
				qdbapi.Add(new XElement("query", "{3.EX.0}"));
			}

			return qdbapi.ToString();
		}
	}
}
