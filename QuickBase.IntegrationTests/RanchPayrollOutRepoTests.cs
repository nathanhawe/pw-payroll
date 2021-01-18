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
	public class RanchPayrollOutRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private RanchPayrollOutRepo _repo;

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
			_repo = new RanchPayrollOutRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_ForSummaries()
		{
			var temp = _repo.GetForSummaries(new DateTime(2020, 11, 29), 0);
			Print(temp);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV_NoLayoff()
		{
			var layoffId = 0;
			var ranchPayLines = new List<RanchPayLine>
			{
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.25M, IsLayoffTagFirstOfTwoInWeek = true },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.26M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.27M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.28M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.29M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.30M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.31M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.32M },
				new RanchPayLine{ QuickBaseRecordId = 10359029, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.33M, IsLayoffTagFirstOfTwoInWeek = true },
				new RanchPayLine{ QuickBaseRecordId = 10359030, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.34M },
				new RanchPayLine{ QuickBaseRecordId = 10359031, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.35M },
				new RanchPayLine{ QuickBaseRecordId = 10359032, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.36M },
				new RanchPayLine{ QuickBaseRecordId = 10359033, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 110M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 10M, SickLeaveRequested = 6.37M },
				new RanchPayLine{ QuickBaseRecordId = 10359034, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 120M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 11M, SickLeaveRequested = 6.38M },
				new RanchPayLine{ QuickBaseRecordId = 10359035, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 130M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 12M, SickLeaveRequested = 6.39M },
				new RanchPayLine{ QuickBaseRecordId = 10359036, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 140M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 12.57M, SickLeaveRequested = 6.40M }
			};

			_repo.PostBatchSize = 3;
			_repo.Save(ranchPayLines);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV_Layoff()
		{
			var layoffId = 42;
			var ranchPayLines = new List<RanchPayLine>
			{
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.25M, IsLayoffTagFirstOfTwoInWeek = true },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.26M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.27M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.28M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.29M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.30M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.31M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M, SickLeaveRequested = 6.32M },
				new RanchPayLine{ QuickBaseRecordId = 10359029, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.33M, IsLayoffTagFirstOfTwoInWeek = true },
				new RanchPayLine{ QuickBaseRecordId = 10359030, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.34M },
				new RanchPayLine{ QuickBaseRecordId = 10359031, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.35M },
				new RanchPayLine{ QuickBaseRecordId = 10359032, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M, SickLeaveRequested = 6.36M },
				new RanchPayLine{ QuickBaseRecordId = 10359033, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 110M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 10M, SickLeaveRequested = 6.37M },
				new RanchPayLine{ QuickBaseRecordId = 10359034, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 120M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 11M, SickLeaveRequested = 6.38M },
				new RanchPayLine{ QuickBaseRecordId = 10359035, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 130M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 12M, SickLeaveRequested = 6.39M },
				new RanchPayLine{ QuickBaseRecordId = 10359036, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 140M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 12.57M, SickLeaveRequested = 6.40M }
			};

			_repo.Save(ranchPayLines);
		}

		[TestMethod]
		[Ignore]
		public void ImportFromCSV_CommasInStrings()
		{
			var ranchPayLines = new List<RanchPayLine>
			{
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "25,18C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "29,37D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "48,70C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "48,67C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "25,18C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "29,37D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "48,70C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "48,67C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = "42,Comma,", Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M }
			};

			_repo.Save(ranchPayLines);
		}

		[TestMethod]
		[Ignore]
		public void Delete()
		{
			var weekEndDate = new DateTime(2019, 1, 6);
			var layoffId = 42;

			var response = _repo.Delete(weekEndDate, layoffId);
			Console.WriteLine(response);
		}

		private void Print(IEnumerable<RanchPayLine> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Ranch Pay Lines:");
			foreach (var line in lines)
			{
				Print(line);
			}
		}

		private void Print(RanchPayLine line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(RanchPayLine).GetProperties();
			foreach (var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}

	}
}
