using Microsoft.Extensions.Logging;
using QuickBase.Api.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace QuickBase.Api
{
	public class QuickBaseConnection : IQuickBaseConnection
	{
		private readonly ILogger<QuickBaseConnection> _logger;

		public string Realm { get; set; }
		public string UserToken { get; set; }
		

		public QuickBaseConnection(string realm, string userToken, ILogger<QuickBaseConnection> logger)
		{
			Realm = realm ?? throw new ArgumentNullException(nameof(realm));
			UserToken = userToken ?? throw new ArgumentNullException(nameof(userToken));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

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
			bool queryIsName = false)
		{
			var uri = GetBaseUriForId(tableId);
			var payload = new Payloads.DoQueryPayload(
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

			return Post(uri, payload);
		}
		
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
			string udata = null)
		{
			var uri = GetBaseUriForId(tableId);
			var payload = new Payloads.DoQueryPayload(
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

			return Post(uri, payload);
		}

		public XElement DoQueryCount(string tableId, string query, string udata = null)
		{
			var uri = GetBaseUriForId(tableId);
			var payload = new Payloads.DoQueryCountPayload(UserToken, query, udata);

			return Post(uri, payload);
		}

		public XElement ImportFromCsv(
			string tableId, 
			string cdata, 
			string clist, 
			string outputClist = null, 
			bool percentageAsString = false, 
			bool skipFirstRow = false, 
			int mergeFieldId = 0, 
			bool useUtcTime = false, 
			string udata = null)
		{
			var uri = GetBaseUriForId(tableId);
			var payload = new Payloads.ImportFromCsvPayload(
				UserToken,
				cdata,
				clist,
				outputClist,
				percentageAsString,
				skipFirstRow,
				mergeFieldId,
				useUtcTime,
				udata);

			return Post(uri, payload);
		}

		private Uri GetBaseUriForId(string id) => new Uri($"https://{Realm}.quickbase.com/db/{id}");

		private XElement Post(Uri uri, Payloads.Payload payLoad)
		{
			_logger.Log(LogLevel.Information, "Posting to {Uri} - {Payload}", uri, payLoad);
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

			// Issue request
			var response = (HttpWebResponse)request.GetResponse();

			// Parse response
			var responseText = ReadResponseToText(response);
			response.Close();

			// todo: Handle non-200 responses
			// Console.WriteLine(((HttpWebResponse)response).StatusDescription);

			var responseXml = XElement.Parse(responseText);

			CheckForErrors(responseXml);

			return responseXml;
		}

		private string GetQuickBaseActionHeader(Constants.QuickBaseAction quickBaseAction)
		{
			return $"QUICKBASE-ACTION:{quickBaseAction}";
		}

		private string ReadResponseToText(HttpWebResponse response)
		{
			string responseFromServer;
			using var dataStream = response.GetResponseStream();
			var reader = new StreamReader(dataStream);
			responseFromServer = reader.ReadToEnd();
			reader.Close();

			return responseFromServer;
		}

		private void CheckForErrors(XElement document)
		{
			var errorCode = document?.Element("errcode")?.Value;
			var errorText = document?.Element("errtext")?.Value ?? string.Empty;

			if (string.IsNullOrWhiteSpace(errorCode)) throw new QuickBaseException("Unable to read errorcode value.");
			if (errorCode != "0") throw new QuickBaseException(errorCode, errorText);
		}
	}
}
