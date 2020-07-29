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
	public class RanchPayrollRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private RanchPayrollRepo _repo;

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
			_repo = new RanchPayrollRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_NoLayoff()
		{
			var temp = _repo.Get(new DateTime(2020, 5, 24), 0);
			Print(temp);
		}

		[TestMethod]
		public void DoQuery_WithLayoff()
		{
			var temp = _repo.Get(new DateTime(2020, 7, 19), 1143);
			Print(temp);
		}

		[TestMethod]
		public void DoQuery_ForSummaries_NoLayoff()
		{
			var temp = _repo.GetForSummaries(new DateTime(2020, 7, 19), 0);
			Print(temp);
		}

		[TestMethod]
		public void DoQuery_ForSummaries_WithLayoff()
		{
			var temp = _repo.GetForSummaries(new DateTime(2020, 7, 19), 1143);
			Print(temp);
		}

		[TestMethod, Ignore]
		public void ImportFromCSV()
		{
			var ranchPayLines = new List<RanchPayLine>
			{
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M },
				new RanchPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.Regular, Pieces = 0M, PieceRate = 0M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 0M }
			};

			var response = _repo.Save(ranchPayLines);
			Console.WriteLine(response);
		}

		[TestMethod, Ignore]
		public void ImportFromCSV_Update()
		{
			var ranchPayLines = new List<RanchPayLine>
			{
				new RanchPayLine{ QuickBaseRecordId = 10359029, LayoffId = 920, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2518C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M },
				new RanchPayLine{ QuickBaseRecordId = 10359030, LayoffId = 920, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "2937D", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M },
				new RanchPayLine{ QuickBaseRecordId = 10359031, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4870C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M },
				new RanchPayLine{ QuickBaseRecordId = 10359032, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Crew = 1002, EmployeeId = "4867C", LaborCode = 207, BlockId = 0, HoursWorked = 9M, PayType = PayType.Pieces, Pieces = 100M, PieceRate = .0125M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = false, HourlyRateOverride = 10M },
				new RanchPayLine{ QuickBaseRecordId = 10359033, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2518C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 110M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 10M },
				new RanchPayLine{ QuickBaseRecordId = 10359034, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "2937D", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 120M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 11M },
				new RanchPayLine{ QuickBaseRecordId = 10359035, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4870C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 130M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 12M },
				new RanchPayLine{ QuickBaseRecordId = 10359036, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Crew = 1002, EmployeeId = "4867C", LaborCode = 208, BlockId = 0, HoursWorked = 10M, PayType = PayType.HourlyPlusPieces, Pieces = 140M, PieceRate = .0133M, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, OtherGross = 0M, TotalGross = 0M, FiveEight = true, HourlyRateOverride = 12.57M }
			};

			var response = _repo.Save(ranchPayLines);
			Console.WriteLine(response);
		}


		private void Print(IEnumerable<RanchPayLine> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Ranch Pay Lines:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(RanchPayLine line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(RanchPayLine).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
