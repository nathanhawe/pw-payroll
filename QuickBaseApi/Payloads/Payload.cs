using System;
using System.Xml.Linq;

namespace QuickBase.Api.Payloads
{
	internal abstract class Payload
	{
		public string UserToken { get; set; }
		public string Udata { get; set; }
		public abstract Constants.QuickBaseAction Action { get; }

		public Payload(string userToken, string udata = null)
		{
			UserToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
			Udata = udata;
		}

		internal abstract string GetXmlString();

		protected XElement GetBaseQdbapi()
		{
			return
				new XElement("qdbapi",
					new XElement("usertoken", UserToken),
					new XElement("udata", Udata)
					);
		}
	}
}
