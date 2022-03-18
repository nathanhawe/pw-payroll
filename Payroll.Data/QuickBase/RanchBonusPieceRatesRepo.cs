using Payroll.Domain;
using Payroll.Domain.Constants.QuickBase;
using QuickBase.Api;
using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	/// <summary>
	/// Repository that exposes query and persistence methods against the Ranch Bonus Piece Rates table in Quick Base.
	/// </summary>
	public class RanchBonusPieceRatesRepo : QuickBaseRepo<RanchBonusPieceRate>, IRanchBonusPieceRatesRepo
	{

		public RanchBonusPieceRatesRepo(IQuickBaseConnection quickBaseConnection)
			:base(quickBaseConnection) { }

		/// <summary>
		/// Queries the Ranch Bonus Piece Rates table in Quick Base for all records.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RanchBonusPieceRate> Get()
		{
			
			var query = $"{{{(int)RanchBonusPieceRateField.RecordId}.{ComparisonOperator.GT}.'0'}}";
			var clist = GetDoQueryClist();
			var slist = $"{(int)RanchBonusPieceRateField.RecordId}";

			return Get(QuickBaseTable.RanchBonusPieceRates, query, clist, slist, ConvertToRanchBonusPieceRate);
		}
		

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Ranch Bonus Piece Rate table in Quick Base into 
		/// a collection of <c>RanchBonusPieceRate</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<RanchBonusPieceRate> ConvertToRanchBonusPieceRate(XElement doQuery)
		{
			var bonusPieceRates = new List<RanchBonusPieceRate>();
			var records = doQuery.Elements("record");

			foreach(var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new RanchBonusPieceRate 
				{ 
					QuickBaseRecordId = recordId 
				};

				var fields = record.Elements("f");
				foreach(var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;
					
					switch (fieldId)
					{
						case (int)RanchBonusPieceRateField.RelatedBlock: temp.BlockId = ParseInt(field.Value) ?? 0; break;
						case (int)RanchBonusPieceRateField.RelatedLaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
						case (int)RanchBonusPieceRateField.EffectiveDate: temp.EffectiveDate = ParseDate(field.Value); break;
						case (int)RanchBonusPieceRateField.PerHourThreshold: temp.PerHourThreshold = ParseDecimal(field.Value) ?? 0; break;
						case (int)RanchBonusPieceRateField.PerTreeBonus: temp.PerTreeBonus = ParseDecimal(field.Value) ?? 0; break;
					}
				}
				bonusPieceRates.Add(temp);
			}

			return bonusPieceRates;
		}

		/// <summary>
		/// Returns a properly formatted clist string for the API_DoQuery call to the Ranch Bonus Piece Rates table in Quickbase.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)RanchBonusPieceRateField.RelatedBlock}.");
			sb.Append($"{(int)RanchBonusPieceRateField.RelatedLaborCode}.");
			sb.Append($"{(int)RanchBonusPieceRateField.EffectiveDate}.");
			sb.Append($"{(int)RanchBonusPieceRateField.PerHourThreshold}.");
			sb.Append($"{(int)RanchBonusPieceRateField.PerTreeBonus}.");

			return sb.ToString();
		}
	}
}