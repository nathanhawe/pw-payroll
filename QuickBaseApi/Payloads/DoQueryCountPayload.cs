using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace QuickBase.Api.Payloads
{
	internal class DoQueryCountPayload : Payload
	{
		public string Query { get; set; }

		public DoQueryCountPayload(string userToken, string query, string udata = null)
			: base(userToken, udata)
		{
			Query = query ?? throw new ArgumentNullException(nameof(query));
		}

		public override QuickBaseAction Action => Constants.QuickBaseAction.API_DoQueryCount;

		public override string ToString()
		{
			return $"DoQueryCountPayload - action:'{Action}', udata:'{Udata}', query:'{Query}'";
		}

		internal override string GetXmlString()
		{
			var qdbapi = GetBaseQdbapi();
			qdbapi.Add(new XElement("query", Query));
			return qdbapi.ToString();
		}
	}
}
