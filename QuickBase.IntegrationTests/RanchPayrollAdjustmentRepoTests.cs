using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using Payroll.Domain.Constants;
using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickBase.IntegrationTests
{
	[TestClass]
	public class RanchPayrollAdjustmentRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private RanchPayrollAdjustmentRepo _repo;

		[TestInitialize]
		public void Setup()
		{
			if (_configuration == null)
			{
				_configuration = ConfigurationHelper.GetIConfigurationRoot();
			}

			var realm = _configuration["QuickBase:Realm"];
			var userToken = _configuration["QuickBase:UserToken"];

			_quickBaseConn = new QuickBaseConnection(realm, userToken);
			_repo = new RanchPayrollAdjustmentRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_NoLayoff()
		{
			var temp = _repo.Get(new DateTime(2020, 3, 1), 0);
			Print(temp);
		}

		[TestMethod]
		public void DoQuery_WithLayoff()
		{
			var temp = _repo.Get(new DateTime(2020, 3, 1), 922);
			Print(temp);
		}

		[TestMethod, Ignore]
		public void ImportFromCSV()
		{
			var ranchAdjustmentLines = new List<RanchAdjustmentLine>
			{
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false }
			};

			var response = _repo.Save(ranchAdjustmentLines);
			Console.WriteLine(response);
		}

		[TestMethod, Ignore]
		public void ImportFromCSV_Update()
		{
			var ranchAdjustmentLines = new List<RanchAdjustmentLine>
			{
				new RanchAdjustmentLine{ QuickBaseRecordId = 193538, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193539, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 110M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193540, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 0, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 120M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193541, LayoffId = 920, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 130, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193542, LayoffId = 923, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 140M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193543, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 150M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193544, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 0, EmployeeId = "4870C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 160M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193545, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 170M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 1), IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true }
			};

			var response = _repo.Save(ranchAdjustmentLines);
			Console.WriteLine(response);
		}


		private void Print(IEnumerable<RanchAdjustmentLine> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Ranch Adjustment Lines:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(RanchAdjustmentLine line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(RanchAdjustmentLine).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
