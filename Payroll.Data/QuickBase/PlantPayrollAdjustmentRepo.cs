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
	/// Repository that exposes query and persistence methods against the Plant Payroll Adjustment table in Quick Base.
	/// </summary>
	public class PlantPayrollAdjustmentRepo : QuickBaseRepo<PlantAdjustmentLine>, IPlantPayrollAdjustmentRepo
	{
		public PlantPayrollAdjustmentRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Queries the Plant Payroll Adjustment table in Quick Base for all records with the provided <c>weekEndOfAdjustmentPaid</c>.  If <c>layoffId</c>
		/// is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.
		/// </summary>
		/// <param name="weekEndOfAdjustmentPaid"></param>
		/// <param name="layoffId"></param>
		public IEnumerable<PlantAdjustmentLine> Get(DateTime weekEndOfAdjustmentPaid, int layoffId)
		{
			var formattedDate = weekEndOfAdjustmentPaid.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PlantPayrollAdjustmentField.WeekEndOfAdjustmentPaid}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)PlantPayrollAdjustmentField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)PlantPayrollAdjustmentField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var clist = GetDoQueryClist();
			var slist = $"{(int)PlantPayrollAdjustmentField.RecordId}";

			return base.Get(QuickBaseTable.PlantPayrollAdjustment, query, clist, slist, ConvertToPlantAdjustmentLines);
		}

		/// <summary>
		/// Creates a new API_ImportFromCSV request to the Plant Payroll Adjustment table in Quickbase for the provided list of <c>PlantAdjustmentLine</c>s.
		/// Records with <c>QuickBaseRecordId</c> values greater than 0 will be updated while those with a value of 0 will be added new.
		/// </summary>
		/// <param name="plantAdjustmentLines"></param>
		/// <returns></returns>
		public XElement Save(IEnumerable<PlantAdjustmentLine> plantAdjustmentLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in plantAdjustmentLines)
			{
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{(line.Plant > 0 ? line.Plant.ToString() : "")},");
				sb.Append($"{line.EmployeeId},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{line.PayType},");
				sb.Append($"{line.WeekEndOfAdjustmentPaid:MM-dd-yyyy},");
				sb.Append($"{(line.IsOriginal ? OriginalOrNewValue.Original : OriginalOrNewValue.New)},");
				sb.Append($"{line.OldHourlyRate},");
				sb.Append($"{(line.UseOldHourlyRate ? "1" : "0")},");
				sb.Append($"{line.OtDtWotHours},");
				sb.Append($"{line.OtDtWotRate},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"{line.TotalGross},");
				sb.Append($"{line.BatchId}");
				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.PlantPayrollAdjustment,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}
			

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Plant Payroll Adjustment table in Quick Base into 
		/// a collection of <c>PlantAdjustmentLine</c> objects.
		/// </summary>
		/// <param name="doQuery"></param>
		/// <returns></returns>
		private IEnumerable<PlantAdjustmentLine> ConvertToPlantAdjustmentLines(XElement doQuery)
		{
			var PlantAdjustmentLines = new List<PlantAdjustmentLine>();
			var records = doQuery.Elements("record");

			foreach (var record in records)
			{
				var recordId = ParseInt(record.Attribute("rid")?.Value) ?? 0;
				var temp = new PlantAdjustmentLine
				{
					QuickBaseRecordId = recordId
				};

				var fields = record.Elements("f");
				foreach (var field in fields)
				{
					var fieldId = ParseInt(field.Attribute("id")?.Value) ?? 0;

					switch (fieldId)
					{
						case (int)PlantPayrollAdjustmentField.LayoffRunId: temp.LayoffId = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)PlantPayrollAdjustmentField.ShiftDate: temp.ShiftDate = ParseDate(field.Value); break;
						case (int)PlantPayrollAdjustmentField.Plant: temp.Plant = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)PlantPayrollAdjustmentField.LaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.HoursWorked: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.PayType: temp.PayType = field.Value; break;
						case (int)PlantPayrollAdjustmentField.Pieces: temp.Pieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.PieceRate: temp.PieceRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.HourlyRate: temp.HourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.GrossFromHours: temp.GrossFromHours = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.GrossFromPieces: temp.GrossFromPieces = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.GrossFromIncentive: temp.GrossFromIncentive = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.OtherGross: temp.OtherGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.AlternativeWorkWeek: 
							temp.AlternativeWorkWeek = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == AlternativeWorkWeekValue.FourTen.ToLower()) ? true : false);
							break;
						case (int)PlantPayrollAdjustmentField.EmployeeHourlyRate: temp.EmployeeHourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.H2A: temp.IsH2A = ParseBooleanFromCheckbox(field.Value); break;
						case (int)PlantPayrollAdjustmentField.WeekEndOfAdjustmentPaid: temp.WeekEndOfAdjustmentPaid = ParseDate(field.Value); break;
						case (int)PlantPayrollAdjustmentField.OriginalOrNew:
							temp.IsOriginal = ((!string.IsNullOrWhiteSpace(field.Value) && field.Value.Trim().ToLower() == OriginalOrNewValue.Original.ToLower() ? true : false));
							break;
						case (int)PlantPayrollAdjustmentField.OldHourlyRate: temp.OldHourlyRate = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.UseOldHourlyRate: temp.UseOldHourlyRate = ParseBooleanFromCheckbox(field.Value); break;
						case (int)PlantPayrollAdjustmentField.CountOfRanchersWorkingPlants: temp.UseCrewLaborRateForMinimumAssurance = (ParseInt(field.Value) ?? 0) > 0; break;
						case (int)PlantPayrollAdjustmentField.BoxStyle: temp.BoxStyle = ParseInt(field.Value) ?? 0;break;
						case (int)PlantPayrollAdjustmentField.BoxStyleDescription: temp.BoxStyleDescription = field.Value; break;
						case (int)PlantPayrollAdjustmentField.EndTime: temp.EndTime = (!string.IsNullOrWhiteSpace(field.Value) ? ParseDate(field.Value) : (DateTime?)null); break;
						case (int)PlantPayrollAdjustmentField.H2AHoursOffered: temp.H2AHoursOffered = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollAdjustmentField.IncentiveDisqualified: temp.IsIncentiveDisqualified = ParseBooleanFromCheckbox(field.Value); break;
						case (int)PlantPayrollAdjustmentField.StartTime: temp.StartTime = (!string.IsNullOrWhiteSpace(field.Value) ? ParseDate(field.Value) : (DateTime?)null); break;

					}
				}
				PlantAdjustmentLines.Add(temp);
			}

			return PlantAdjustmentLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Plant Payroll Adjustment table in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollAdjustmentField.LayoffRunId}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.WeekEndDate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.ShiftDate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.Plant}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.HoursWorked}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.PayType}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.Pieces}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.PieceRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.GrossFromIncentive}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OtherGross}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.AlternativeWorkWeek}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.EmployeeHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.H2A}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.WeekEndOfAdjustmentPaid}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OriginalOrNew}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OldHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.UseOldHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.CountOfRanchersWorkingPlants}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.BoxStyle}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.BoxStyleDescription}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.EndTime}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.H2AHoursOffered}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.IncentiveDisqualified}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.StartTime}.");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Plant Payroll Adjustment table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollAdjustmentField.RecordId}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.LayoffRunId}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.ShiftDate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.Plant}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.PayType}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.WeekEndOfAdjustmentPaid}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OriginalOrNew}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OldHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.UseOldHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OtDtWotHours}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OtDtWotRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.CalculatedHourlyRate}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.CalculatedGrossFromHours}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.CalculatedGrossFromPieces}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.OtherGross}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.CalculatedTotalGross}.");
			sb.Append($"{(int)PlantPayrollAdjustmentField.BatchId}.");

			return sb.ToString();
		}
	}
}
