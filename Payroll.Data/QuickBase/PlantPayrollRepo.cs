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
	/// Repository that exposes query and persistence methods against the Plant Payroll table in Quick Base.
	/// </summary>
	public class PlantPayrollRepo : QuickBaseRepo<PlantPayLine>, IPlantPayrollRepo
	{
		public PlantPayrollRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Queries the Plant Payroll table in Quick Base for all records with the provided <c>weekEndDate</c>.  If <c>layoffId</c>
		/// is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		public IEnumerable<PlantPayLine> Get(DateTime weekEndDate, int layoffId)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PlantPayrollField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)PlantPayrollField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)PlantPayrollField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var clist = GetDoQueryClist();
			var slist = $"{(int)PlantPayrollField.RecordId}";

			return base.Get(QuickBaseTable.PlantPayroll, query, clist, slist, ConvertToPlantPayLines);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Plant Payroll table in Quickbase for the provided list of <c>PlantPayLine</c>s.
		/// Records with <c>QuickBaseRecordId</c> values greater than 0 will be updated while those with a value of 0 will be added new.
		/// </summary>
		/// <param name="plantPayLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<PlantPayLine> plantPayLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in plantPayLines)
			{
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{(line.Plant > 0 ? line.Plant.ToString() : "")},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{line.PayType},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.GrossFromIncentive},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"{line.TotalGross},");
				sb.Append($"{line.HourlyRateOverride}");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.PlantPayroll,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}
			

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Plant Payroll table in Quick Base into 
		/// a collection of <c>PlantPayLine</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<PlantPayLine> ConvertToPlantPayLines(XElement doQuery)
		{
			var PlantPayLines = new List<PlantPayLine>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new PlantPayLine
				{
					QuickBaseRecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)PlantPayrollField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)PlantPayrollField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)PlantPayrollField.Plant: temp.Plant = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollField.EmployeeNumber: temp.EmployeeId = field.Value; break;
						case (int)PlantPayrollField.LaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollField.HoursWorked: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.PayType: temp.PayType = field.Value; break;
						case (int)PlantPayrollField.Pieces: temp.Pieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.HourlyRate: temp.HourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.GrossFromHours: temp.GrossFromHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.GrossFromPieces: temp.GrossFromPieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.GrossFromIncentive: temp.GrossFromIncentive = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.OtherGross: temp.OtherGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.AlternativeWorkWeek: 
							temp.AlternativeWorkWeek = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == AlternativeWorkWeekValue.FourTen.ToLower()) ? true : false);
							break;
						case (int)PlantPayrollField.HourlyRateOverride: temp.HourlyRateOverride = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.EmployeeHourlyRate: temp.EmployeeHourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.H2A: temp.IsH2A = ParseBooleanFromCheckbox(field.Value); break;
						case (int)PlantPayrollField.IncentiveDisqualified: temp.IsIncentiveDisqualified = ParseBooleanFromCheckbox(field.Value); break;
						case (int)PlantPayrollField.NonPrimaViolation:
							temp.HasNonPrimaViolation = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == NonPrimaViolationValue.Yes.ToLower()) ? true : false);
							break;
						case (int)PlantPayrollField.UseIncreasedRate: temp.UseIncreasedRate = ParseBooleanFromCheckbox(field.Value); break;
						case (int)PlantPayrollField.NonPrimaRate: temp.NonPrimaRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.PrimaRate: temp.PrimaRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollField.IncreasedRate: temp.IncreasedRate = ParseDecimal(field.Value) ?? 0; break;

					}
				}
				PlantPayLines.Add(temp);
			}

			return PlantPayLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Plant Payroll table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollField.LayoffRunId}.");
			sb.Append($"{(int)PlantPayrollField.WeekEndDate}.");
			sb.Append($"{(int)PlantPayrollField.ShiftDate}.");
			sb.Append($"{(int)PlantPayrollField.Plant}.");
			sb.Append($"{(int)PlantPayrollField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollField.HoursWorked}.");
			sb.Append($"{(int)PlantPayrollField.PayType}.");
			sb.Append($"{(int)PlantPayrollField.Pieces}.");
			sb.Append($"{(int)PlantPayrollField.HourlyRate}.");
			sb.Append($"{(int)PlantPayrollField.GrossFromHours}.");
			sb.Append($"{(int)PlantPayrollField.GrossFromPieces}.");
			sb.Append($"{(int)PlantPayrollField.GrossFromIncentive}.");
			sb.Append($"{(int)PlantPayrollField.OtherGross}.");
			sb.Append($"{(int)PlantPayrollField.TotalGross}.");
			sb.Append($"{(int)PlantPayrollField.AlternativeWorkWeek}.");
			sb.Append($"{(int)PlantPayrollField.HourlyRateOverride}.");
			sb.Append($"{(int)PlantPayrollField.EmployeeHourlyRate}.");
			sb.Append($"{(int)PlantPayrollField.H2A}.");
			sb.Append($"{(int)PlantPayrollField.IncentiveDisqualified}.");
			sb.Append($"{(int)PlantPayrollField.NonPrimaViolation}.");
			sb.Append($"{(int)PlantPayrollField.UseIncreasedRate}.");
			sb.Append($"{(int)PlantPayrollField.NonPrimaRate}.");
			sb.Append($"{(int)PlantPayrollField.PrimaRate}.");
			sb.Append($"{(int)PlantPayrollField.IncreasedRate}");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Plant Payroll table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollField.RecordId}.");
			sb.Append($"{(int)PlantPayrollField.LayoffRunId}.");
			sb.Append($"{(int)PlantPayrollField.ShiftDate}.");
			sb.Append($"{(int)PlantPayrollField.Plant}.");
			sb.Append($"{(int)PlantPayrollField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollField.HoursWorked}.");
			sb.Append($"{(int)PlantPayrollField.PayType}.");
			sb.Append($"{(int)PlantPayrollField.HourlyRate}.");
			sb.Append($"{(int)PlantPayrollField.GrossFromHours}.");
			sb.Append($"{(int)PlantPayrollField.GrossFromPieces}.");
			sb.Append($"{(int)PlantPayrollField.GrossFromIncentive}.");
			sb.Append($"{(int)PlantPayrollField.OtherGross}.");
			sb.Append($"{(int)PlantPayrollField.TotalGross}.");
			sb.Append($"{(int)PlantPayrollField.HourlyRateOverride}.");

			return sb.ToString();
		}
	}
}
