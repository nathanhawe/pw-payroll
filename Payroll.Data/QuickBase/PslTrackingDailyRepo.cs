using Payroll.Domain;
using Payroll.Domain.Constants.QuickBase;
using QuickBase.Api;
using QuickBase.Api.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	/// <summary>
	/// Repository that exposes query and persistence methods against the PSL Tracking Daily table in Quick Base.
	/// </summary>
	public class PslTrackingDailyRepo : QuickBaseRepo<PaidSickLeave>, IPslTrackingDailyRepo
	{
		public PslTrackingDailyRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection) { }

		/// <summary>
		/// Queries the PSL Tracking Daily table in Quick Base for all records with Shift Dates between <c>startDate</c> and <c>endDate</c> inclusive
		/// for the specified <c>Company</c>.
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public IEnumerable<PaidSickLeave> Get(DateTime startDate, DateTime endDate, string company)
		{
			var formattedStartDate = startDate.ToString("MM-dd-yyyy");
			var formattedEndDate = endDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PslTrackingDailyField.ShiftDate}.{ComparisonOperator.OAF}.'{formattedStartDate}'}}";
			query += $"AND{{{(int)PslTrackingDailyField.ShiftDate}.{ComparisonOperator.OBF}.'{formattedEndDate}'}}";
			query += $"AND{{{(int)PslTrackingDailyField.Company}.{ComparisonOperator.EX}.'{company}'}}";
			
			var clist = GetDoQueryClist();
			var slist = $"{(int)PslTrackingDailyField.EmployeeDate}";

			return base.Get(QuickBaseTable.PslTrackingDaily, query, clist, slist, ConvertToPaidSickLeaves);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the PSL Tracking Daily table in Quickbase based on the provided list of <c>PaidSickLeave</c>s.
		/// If a record exists for the employee and shift date it will be updated, otherwise a new record is created.
		/// </summary>
		/// <param name="paidSickLeaves"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<PaidSickLeave> paidSickLeaves)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in paidSickLeaves)
			{
				sb.Append($"\"{line.EmployeeId} {line.ShiftDate:M/d/yyyy} {line.Company}\",");
				sb.Append($"\"{line.EmployeeId}\",");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{line.Company},");
				sb.Append($"{line.Hours},");
				sb.Append($"{line.Gross},");
				sb.Append($"{line.NinetyDayHours},");
				sb.Append($"{line.NinetyDayGross},");
				sb.Append($"{(line.HoursUsed != 0 ? line.HoursUsed.ToString() : "")}");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.PslTrackingDaily,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the PSL Tracking Daily table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PslTrackingDailyField.EmployeeDate}.");
			sb.Append($"{(int)PslTrackingDailyField.EmployeeNumber}.");
			sb.Append($"{(int)PslTrackingDailyField.ShiftDate}.");
			sb.Append($"{(int)PslTrackingDailyField.Company}.");
			sb.Append($"{(int)PslTrackingDailyField.Hours}.");
			sb.Append($"{(int)PslTrackingDailyField.Gross}.");
			sb.Append($"{(int)PslTrackingDailyField.NinetyDayHours}.");
			sb.Append($"{(int)PslTrackingDailyField.NinetyDayGross}.");
			sb.Append($"{(int)PslTrackingDailyField.PSLUsed}");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the PSL Tracking Daily table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			return GetDoQueryClist();
		}

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the PSL Tracking Daily table in Quick Base into 
		/// a collection of <c>PaidSickLeave</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<PaidSickLeave> ConvertToPaidSickLeaves(XElement doQuery)
		{
			var paidSickLeaves = new List<PaidSickLeave>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new PaidSickLeave();				

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)PslTrackingDailyField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)PslTrackingDailyField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)PslTrackingDailyField.Company: temp.Company = field.Value ?? ""; break;
						case (int)PslTrackingDailyField.Hours: temp.Hours = ParseDecimal(field.Value) ?? 0; break;
						case (int)PslTrackingDailyField.Gross: temp.Gross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PslTrackingDailyField.NinetyDayHours: temp.NinetyDayHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)PslTrackingDailyField.NinetyDayGross: temp.NinetyDayGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PslTrackingDailyField.PSLUsed: temp.HoursUsed = ParseDecimal(field.Value) ?? 0; break;
					}
				}
				paidSickLeaves.Add(temp);
			}

			return paidSickLeaves;
		}
	}
}
