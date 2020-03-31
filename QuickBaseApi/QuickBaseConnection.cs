using System;
using System.IO;
using System.Net;
using System.Text;

namespace QuickBase.Api
{
	public class QuickBaseConnection
	{
		public string Realm { get; set; }
		public string UserToken { get; set; }

		public QuickBaseConnection(string realm, string userToken)
		{
			Realm = realm ?? throw new ArgumentNullException(nameof(realm));
			UserToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
		}

		public string DoQuery(
			string applicationId,
			string query = null,
			string clist = null,
			string slist = null,
			string options = null,
			string format = "structured",
			bool returnPercentageAsString = false,
			bool includeRecordIds = true,
			bool? useFieldIds = null,
			string udata = null,
			bool queryIsName = false)
		{
			var uri = GetBaseUriForId(applicationId);
			var payLoad = new Payloads.DoQueryPayload(
				UserToken,
				query,
				clist,
				slist,
				options,
				format,
				returnPercentageAsString,
				includeRecordIds,
				useFieldIds,
				udata,
				queryIsName);

			var response = Post(uri, payLoad);

			// todo: Handle non-200 responses
			//Console.WriteLine(((HttpWebResponse)response).StatusDescription);

			string responseFromServer;
			using (var dataStream = response.GetResponseStream())
			{
				var reader = new StreamReader(dataStream);
				responseFromServer = reader.ReadToEnd();
			}
			response.Close();

			return responseFromServer;
		}

		public string DoQuery(
			string applicationId,
			int qid,
			string clist = null,
			string slist = null,
			string options = null,
			string format = "structured",
			bool returnPercentageAsString = false,
			bool includeRecordIds = true,
			bool? useFieldIds = null,
			string udata = null)
		{
			var uri = GetBaseUriForId(applicationId);
			var payLoad = new Payloads.DoQueryPayload(
				UserToken,
				qid,
				clist,
				slist,
				options,
				format,
				returnPercentageAsString,
				includeRecordIds,
				useFieldIds,
				udata);

			var response = Post(uri, payLoad);

			// todo: Handle non-200 responses
			//Console.WriteLine(((HttpWebResponse)response).StatusDescription);

			string responseFromServer;
			using (var dataStream = response.GetResponseStream())
			{
				var reader = new StreamReader(dataStream);
				responseFromServer = reader.ReadToEnd();
			}
			response.Close();

			return responseFromServer;
		}

		public string DoQueryCount(string tableId, string query, string udata = null)
		{
			var uri = GetBaseUriForId(tableId);
			var payLoad = new Payloads.DoQueryCountPayload(UserToken, query, udata);
			var response = Post(uri, payLoad);

			// todo: Handle non-200 responses
			//Console.WriteLine(((HttpWebResponse)response).StatusDescription);

			string responseFromServer;
			using (var dataStream = response.GetResponseStream())
			{
				var reader = new StreamReader(dataStream);
				responseFromServer = reader.ReadToEnd();
			}
			response.Close();

			return responseFromServer;
		}

		private Uri GetBaseUriForId(string id) => new Uri($"https://{Realm}.quickbase.com/db/{id}");

		private HttpWebResponse Post(Uri uri, Payloads.Payload payLoad)
		{
			var request = WebRequest.Create(uri);
			var data = payLoad.GetXmlString();
			byte[] byteArray = Encoding.UTF8.GetBytes(data);

			request.Method = "POST";
			request.ContentType = "application/xml";
			request.ContentLength = byteArray.Length;
			request.Headers.Add(GetQuickBaseActionHeader(payLoad.Action));

			// Write data into request
			var dataStream = request.GetRequestStream();
			dataStream.Write(byteArray, 0, byteArray.Length);
			dataStream.Close();

			var response = request.GetResponse();

			return (HttpWebResponse)response;
		}

		private string GetQuickBaseActionHeader(Constants.QuickBaseAction quickBaseAction)
		{
			return quickBaseAction switch
			{
				Constants.QuickBaseAction.API_DoQuery => "QUICKBASE-ACTION:API_DoQuery",
				Constants.QuickBaseAction.API_DoQueryCount => "QUICKBASE-ACTION:API_DoQueryCount",
				_ => "",
			};
		}
	}
}
