﻿using Payroll.Domain;
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
	/// Repository that exposes query and persistence methods against the Plant Payroll Out table in Quick Base.
	/// </summary>
	public class PlantPayrollOutRepo : QuickBaseRepo<PlantPayLine>, IPlantPayrollOutRepo
	{
		public PlantPayrollOutRepo(IQuickBaseConnection quickBaseConnection)
			: base(quickBaseConnection)	{ }


		/// <summary>
		/// Queries the Plant Payroll Out table in Quick Base for all records with the provided <c>weekEndDate</c>.  If <c>layoffId</c>
		/// is greater than 0, records are filtered to only those with a matching [LayOffRunId].  If <c>layoffId</c> is equal
		/// to 0, only records without a [LayOffRunId] are returned.  This method uses CLIST and SLIST values specifically for
		/// the creation of plant summaries.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		public IEnumerable<PlantPayLine> GetForSummaries(DateTime weekEndDate, int layoffId)
		{
			var clist = GetDoQueryClistForSummaries();
			var slist = $"{(int)PlantPayrollOutField.EmployeeNumber}";

			return Get(weekEndDate, layoffId, clist, slist);
		}

		/// <summary>
		/// Creates new API_ImportFromCSV requests to the Plant Payroll Out table in Quickbase for the provided list of <c>PlantPayLine</c>s.
		/// </summary>
		/// <param name="plantPayLines"></param>
		/// <returns></returns>
		public void Save(IEnumerable<PlantPayLine> plantPayLines)
		{
			for (int i = 0; i <= plantPayLines.Count(); i += PostBatchSize)
			{
				ImportFromCsv(plantPayLines.Skip(i).Take(PostBatchSize));
			}
		}

		/// <summary>
		/// Creates a new API_PurgeRecords request to the Plant Payroll Out table in Quick Base based on the provided <c>weekEndDate</c> and <c>layoffId</c>.
		/// </summary>
		/// <param name="weekEndDate"></param>
		/// <param name="layoffId"></param>
		/// <returns></returns>
		public XElement Delete(DateTime weekEndDate, int layoffId)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PlantPayrollOutField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)PlantPayrollOutField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)PlantPayrollOutField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			var deleteResponse = _quickBaseConn.PurgeRecords(
				QuickBaseTable.PlantPayrollOut,
				query);

			return deleteResponse;
		}

		private XElement ImportFromCsv(IEnumerable<PlantPayLine> plantPayLines)
		{
			var clist = GetImportFromCsvClist();

			// Build the CDATA string
			var sb = new StringBuilder();
			foreach (var line in plantPayLines)
			{
				sb.Append($"{line.BatchId},");
				sb.Append($"{line.BoxStyle},");
				sb.Append($"\"{line.BoxStyleDescription}\",");
				sb.Append($"\"{line.EmployeeId}\",");
				sb.Append($"{line.EndTime:MM-dd-yyyy H:mm},");
				sb.Append($"{line.GrossFromHours},");
				sb.Append($"{line.GrossFromIncentive},");
				sb.Append($"{line.GrossFromPieces},");
				sb.Append($"{line.H2AHoursOffered},");
				sb.Append($"{line.HourlyRate},");
				sb.Append($"{line.HourlyRateOverride},");
				sb.Append($"{line.HoursWorked},");
				sb.Append($"{(line.IsIncentiveDisqualified ? "1" : "0")},");
				sb.Append($"{(line.UseIncreasedRate ? "1" : "0")},");
				sb.Append($"{line.IncreasedRate},");
				sb.Append($"{(line.LaborCode > 0 ? line.LaborCode.ToString() : "")},");
				sb.Append($"{(line.LayoffId > 0 ? line.LayoffId.ToString() : "")},");
				sb.Append($"{line.NonPrimaRate},");
				sb.Append($"{(line.HasNonPrimaViolation ? NonPrimaViolationValue.Yes : "")},");
				sb.Append($"{line.OtDtWotHours},");
				sb.Append($"{line.OtDtWotRate},");
				sb.Append($"{line.OtherGross},");
				sb.Append($"\"{line.PayType}\",");
				sb.Append($"{line.Pieces},");
				sb.Append($"{(line.Plant > 0 ? line.Plant.ToString() : "")},");
				sb.Append($"{line.PrimaRate},");
				sb.Append($"{line.ShiftDate:MM-dd-yyyy},");
				sb.Append($"{line.StartTime:MM-dd-yyyy H:mm},");
				sb.Append($"{(line.QuickBaseRecordId > 0 ? line.QuickBaseRecordId.ToString() : "")},");
				sb.Append($"{line.SickLeaveRequested},");
				sb.Append($"{(line.PackerNumber > 0 ? line.PackerNumber.ToString() : "")},");
				sb.Append($"\"{line.Packline}\",");

				sb.Append("\n");
			}

			// Create the request
			var importResponse = _quickBaseConn.ImportFromCsv(
				QuickBaseTable.PlantPayrollOut,
				sb.ToString(),
				clist,
				percentageAsString: false,
				skipFirstRow: false);

			return importResponse;
		}

		private IEnumerable<PlantPayLine> Get(DateTime weekEndDate, int layoffId, string clist, string slist)
		{
			var formattedDate = weekEndDate.ToString("MM-dd-yyyy");

			var query = $"{{{(int)PlantPayrollOutField.WeekEndDate}.{ComparisonOperator.IR}.'{formattedDate}'}}";
			if (layoffId > 0)
			{
				query += $"AND{{{(int)PlantPayrollOutField.LayoffRunId}.{ComparisonOperator.EX}.{layoffId}}}";
			}
			else
			{
				query += $"AND{{{(int)PlantPayrollOutField.LayoffPay}.{ComparisonOperator.EX}.0}}";
			}

			return base.Get(QuickBaseTable.PlantPayrollOut, query, clist, slist, ConvertToPlantPayLines);
		}

		/// <summary>
		/// Converts an XElement object representing an API_DoQuery response from the Plant Payroll Out table in Quick Base into 
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
						case (int)PlantPayrollOutField.EmployeeNumber: temp.EmployeeId = field.Value.ToUpper(); break;
						case (int)PlantPayrollOutField.WeekEndDate: temp.WeekEndDate = ParseDate(field.Value); break;
						case (int)PlantPayrollOutField.LaborCode: temp.LaborCode = ParseInt(field.Value) ?? 0; break;
						case (int)PlantPayrollOutField.HoursWorked: temp.HoursWorked = ParseDecimal(field.Value) ?? 0; break;
						case (int)PlantPayrollOutField.TotalGross: temp.TotalGross = ParseDecimal(field.Value) ?? 0; break;
					}
				}
				PlantPayLines.Add(temp);
			}

			return PlantPayLines;
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_DoQuery calls to the Plant Payroll Out table in Quick Base with a
		/// specific emphasis on fields necessary for the creation of plant summaries.
		/// </summary>
		/// <returns></returns>
		private string GetDoQueryClistForSummaries()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollOutField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollOutField.WeekEndDate}.");
			sb.Append($"{(int)PlantPayrollOutField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollOutField.HoursWorked}.");
			sb.Append($"{(int)PlantPayrollOutField.TotalGross}.");

			return sb.ToString();
		}

		/// <summary>
		/// Returns a properly formatted clist string for API_ImportFromCSV calls to the Plant Payroll Out table in Quick Base.
		/// A clist is required to properly map values to fields in Quick Base.
		/// </summary>
		/// <returns></returns>
		private string GetImportFromCsvClist()
		{
			var sb = new StringBuilder();
			sb.Append($"{(int)PlantPayrollOutField.BatchId}.");
			sb.Append($"{(int)PlantPayrollOutField.BoxStyle}.");
			sb.Append($"{(int)PlantPayrollOutField.BoxStyleDescription}.");
			sb.Append($"{(int)PlantPayrollOutField.EmployeeNumber}.");
			sb.Append($"{(int)PlantPayrollOutField.EndTime}.");
			sb.Append($"{(int)PlantPayrollOutField.GrossFromHours}.");
			sb.Append($"{(int)PlantPayrollOutField.GrossFromIncentive}.");
			sb.Append($"{(int)PlantPayrollOutField.GrossFromPieces}.");
			sb.Append($"{(int)PlantPayrollOutField.H2AHoursOffered}.");
			sb.Append($"{(int)PlantPayrollOutField.HourlyRate}.");
			sb.Append($"{(int)PlantPayrollOutField.HourlyRateOverride}.");
			sb.Append($"{(int)PlantPayrollOutField.HoursWorked}.");
			sb.Append($"{(int)PlantPayrollOutField.IncentiveDisqualified}.");
			sb.Append($"{(int)PlantPayrollOutField.UseIncreasedRate}.");
			sb.Append($"{(int)PlantPayrollOutField.IncreasedRate}.");
			sb.Append($"{(int)PlantPayrollOutField.LaborCode}.");
			sb.Append($"{(int)PlantPayrollOutField.LayoffRunId}.");
			sb.Append($"{(int)PlantPayrollOutField.NonPrimaRate}.");
			sb.Append($"{(int)PlantPayrollOutField.NonPrimaViolation}.");
			sb.Append($"{(int)PlantPayrollOutField.OtDtWotHours}.");
			sb.Append($"{(int)PlantPayrollOutField.OtDtWotRate}.");
			sb.Append($"{(int)PlantPayrollOutField.OtherGross}.");
			sb.Append($"{(int)PlantPayrollOutField.PayType}.");
			sb.Append($"{(int)PlantPayrollOutField.Pieces}.");
			sb.Append($"{(int)PlantPayrollOutField.Plant}.");
			sb.Append($"{(int)PlantPayrollOutField.PrimaRate}.");
			sb.Append($"{(int)PlantPayrollOutField.ShiftDate}.");
			sb.Append($"{(int)PlantPayrollOutField.StartTime}.");
			sb.Append($"{(int)PlantPayrollOutField.SourceRid}.");
			sb.Append($"{(int)PlantPayrollOutField.SickLeaveRequested}.");
			sb.Append($"{(int)PlantPayrollOutField.PackerNumber}.");
			sb.Append($"{(int)PlantPayrollOutField.Packline}.");

			return sb.ToString();
		}
	}
}
