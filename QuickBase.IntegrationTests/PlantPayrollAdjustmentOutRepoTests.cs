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
	public class PlantPayrollAdjustmentOutRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private PlantPayrollAdjustmentOutRepo _repo;

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
			_repo = new PlantPayrollAdjustmentOutRepo(_quickBaseConn);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV()
		{
			var layoffId = 0;
			
			var PlantAdjustmentLines = new List<PlantAdjustmentLine>
			{
				new PlantAdjustmentLine{ BoxStyle = 12, BoxStyleDescription = "Testing12", H2AHoursOffered = 8M, IsIncentiveDisqualified = false, StartTime = new DateTime(2019, 1, 1, 6, 30, 0), EndTime = new DateTime(2019, 1, 1, 15, 30, 0), QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.96M, UseOldHourlyRate = true, SickLeaveRequested = 0 },
				new PlantAdjustmentLine{ BoxStyle = 11, BoxStyleDescription = "Testing11", H2AHoursOffered = 13.25M, IsIncentiveDisqualified = true, StartTime = new DateTime(2019, 1, 1, 6, 30, 0), EndTime = new DateTime(2019, 1, 1, 20, 45, 0), QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.97M, UseOldHourlyRate = true, SickLeaveRequested = 4M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.98M, UseOldHourlyRate = true, SickLeaveRequested = 4.01M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.99M, UseOldHourlyRate = true, SickLeaveRequested = 4.02M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.03M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.04M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.05M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.06M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849853, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.OverTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .91M, OtherGross = 10M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.96M, UseOldHourlyRate = true, SickLeaveRequested = 4.07M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849854, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "778973", LaborCode = 123, HoursWorked = 9M, PayType = PayType.DoubleTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .92M, OtherGross = 10M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.97M, UseOldHourlyRate = true, SickLeaveRequested = 4.08M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849855, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "4870C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.Pieces, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .93M, OtherGross = 20M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.98M, UseOldHourlyRate = true, SickLeaveRequested = 4.09M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849856, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "4867C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .94M, OtherGross = 20M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.99M, UseOldHourlyRate = true, SickLeaveRequested = 4.1M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849857, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .95M, OtherGross = 30M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.11M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849858, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .96M, OtherGross = 30M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.12M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849859, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .97M, OtherGross = 40M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.13M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849860, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .98M, OtherGross = 41.92M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.14M }
			};
			_repo.PostBatchSize = 3;
			_repo.Save(PlantAdjustmentLines);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_WithLayoff()
		{
			var layoffId = 98;

			var PlantAdjustmentLines = new List<PlantAdjustmentLine>
			{
				new PlantAdjustmentLine{ BoxStyle = 12, BoxStyleDescription = "Testing12", H2AHoursOffered = 8M, IsIncentiveDisqualified = false, StartTime = new DateTime(2019, 1, 1, 6, 30, 0), EndTime = new DateTime(2019, 1, 1, 15, 30, 0), QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.96M, UseOldHourlyRate = true, SickLeaveRequested = 0 },
				new PlantAdjustmentLine{ BoxStyle = 11, BoxStyleDescription = "Testing11", H2AHoursOffered = 13.25M, IsIncentiveDisqualified = true, StartTime = new DateTime(2019, 1, 1, 6, 30, 0), EndTime = new DateTime(2019, 1, 1, 20, 45, 0), QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.97M, UseOldHourlyRate = true, SickLeaveRequested = 4M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.98M, UseOldHourlyRate = true, SickLeaveRequested = 4.01M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.99M, UseOldHourlyRate = true, SickLeaveRequested = 4.02M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.03M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.04M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.05M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.06M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849853, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.OverTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .91M, OtherGross = 10M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.96M, UseOldHourlyRate = true, SickLeaveRequested = 4.07M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849854, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "778973", LaborCode = 123, HoursWorked = 9M, PayType = PayType.DoubleTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .92M, OtherGross = 10M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.97M, UseOldHourlyRate = true, SickLeaveRequested = 4.08M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849855, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "4870C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.Pieces, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .93M, OtherGross = 20M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.98M, UseOldHourlyRate = true, SickLeaveRequested = 4.09M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849856, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "4867C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .94M, OtherGross = 20M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.99M, UseOldHourlyRate = true, SickLeaveRequested = 4.1M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849857, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .95M, OtherGross = 30M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.11M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849858, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .96M, OtherGross = 30M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.12M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849859, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .97M, OtherGross = 40M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.13M },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849860, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .98M, OtherGross = 41.92M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false, SickLeaveRequested = 4.14M }
			};

			_repo.Save(PlantAdjustmentLines);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_CommaInStrings()
		{
			var PlantAdjustmentLines = new List<PlantAdjustmentLine>
			{
				new PlantAdjustmentLine{ ShiftDate = new DateTime(2019, 1, 1), EmployeeId = "1,2,3,", BoxStyleDescription = "This is a test, only a test.", PayType = "42-Commas," },
				new PlantAdjustmentLine{ ShiftDate = new DateTime(2019, 1, 2), EmployeeId = "1,2,3,", BoxStyleDescription = "This is a test, only a test.", PayType = "42-Commas," },
				new PlantAdjustmentLine{ ShiftDate = new DateTime(2019, 1, 3), EmployeeId = "1,2,3,", BoxStyleDescription = "This is a test, only a test.", PayType = "42-Commas," },
			};

			_repo.Save(PlantAdjustmentLines);
		}

		[TestMethod]
		[Ignore]
		public void Delete()
		{
			var weekEndDate = new DateTime(2019, 1, 6);
			var layoffId = 0;

			var response = _repo.Delete(weekEndDate, layoffId);
			Console.WriteLine(response);
		}


	}
}
