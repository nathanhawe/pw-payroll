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
	public class RanchPayrollAdjustmentOutRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private RanchPayrollAdjustmentOutRepo _repo;

		[TestInitialize]
		public void Setup()
		{
			if (_configuration == null)
			{
				_configuration = ConfigurationHelper.GetIConfigurationRoot();
			}

			var realm = _configuration["QuickBase:Realm"];
			var userToken = _configuration["QuickBase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();

			_quickBaseConn = new QuickBaseConnection(realm, userToken, logger);
			_repo = new RanchPayrollAdjustmentOutRepo(_quickBaseConn);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV_NoLayoff()
		{
			var adjustmentWeekEndDate = new DateTime(2019, 1, 1);
			var layoffId = 0;

			var ranchAdjustmentLines = new List<RanchAdjustmentLine>
			{
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, StartTime = "44147.000694444", EndTime = "44147.213888889", SickLeaveRequested = 0M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.25M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.26M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.27M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.28M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.29M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.30M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.31M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193538, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.32M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193539, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 110M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.33M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193540, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 0, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 120M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.34M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193541, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 130, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.35M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193542, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 140M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.36M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193543, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 150M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.37M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193544, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 0, EmployeeId = "4870C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 160M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.38M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193545, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 170M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.39M }
			};
			_repo.PostBatchSize = 3;
			_repo.Save(ranchAdjustmentLines);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV_Layoff()
		{
			var adjustmentWeekEndDate = new DateTime(2019, 1, 1);
			var layoffId = 980;

			var ranchAdjustmentLines = new List<RanchAdjustmentLine>
			{
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, StartTime = "44147.000694444", EndTime = "44147.213888889", SickLeaveRequested = 0M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.25M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.26M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.27M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.28M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.29M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.30M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.31M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193538, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.32M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193539, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 110M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.33M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193540, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 0, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 120M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.34M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193541, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 130, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false, SickLeaveRequested = 6.35M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193542, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 140M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.36M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193543, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 150M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.37M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193544, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 0, EmployeeId = "4870C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 160M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.38M },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193545, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = PayType.Pieces, Pieces = 170M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true, SickLeaveRequested = 6.39M }
			};

			_repo.Save(ranchAdjustmentLines);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV_CommasInStrings()
		{
			var adjustmentWeekEndDate = new DateTime(2019, 1, 1);
			var layoffId = 0;

			var ranchAdjustmentLines = new List<RanchAdjustmentLine>
			{
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "25,18C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, StartTime = "44147,000694444", EndTime = "44147,213888889" },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "29,37D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false, StartTime = "44147.000694444", EndTime = "44147.213888889"  },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "48,70C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "48,67C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "25,18C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "29,37D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "48,70C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "48,67C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193538, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "25,18C", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193539, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "29,37D", LaborCode = 0, BlockId = 998, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 110M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193540, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "48,70C", LaborCode = 0, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 120M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193541, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "48,67C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 130, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = false, OldHourlyRate = 10M, UseOldHourlyRate = false },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193542, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "25,18C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 140M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193543, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "29,37D", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 150M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193544, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 0, EmployeeId = "48,70C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 160M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true },
				new RanchAdjustmentLine{ QuickBaseRecordId = 193545, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "48,67C", LaborCode = 208, BlockId = 999, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 170M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, WeekEndOfAdjustmentPaid = adjustmentWeekEndDate, IsOriginal = true, OldHourlyRate = 10M, UseOldHourlyRate = true }
			};

			_repo.Save(ranchAdjustmentLines);
		}

		[TestMethod]
		[Ignore]
		public void Delete()
		{
			var adjustmentWeekEndDate = new DateTime(2019, 1, 1);
			var layoffId = 0;

			var response = _repo.Delete(adjustmentWeekEndDate, layoffId);
			Console.WriteLine(response);
		}


	}
}
