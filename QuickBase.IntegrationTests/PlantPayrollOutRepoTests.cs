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
	public class PlantPayrollRepoOutTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private PlantPayrollOutRepo _repo;

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
			_repo = new PlantPayrollOutRepo(_quickBaseConn);
		}

		

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV()
		{
			var layoffId = 0;
			var PlantPayLines = new List<PlantPayLine>
			{
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, StartTime = new DateTime(2019, 1, 1, 7, 30, 0), EndTime = new DateTime(2019, 1, 1, 15, 45, 0), SickLeaveRequested = 0M},
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.00M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.01M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.02M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.03M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.04M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.05M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.06M },
				new PlantPayLine{ QuickBaseRecordId = 7287999, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.OverTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 10M, TotalGross = 0M, HourlyRateOverride = 15M, SickLeaveRequested = 5.07M },
				new PlantPayLine{ QuickBaseRecordId = 7288000, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "778973", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 11M, TotalGross = 0M, HourlyRateOverride = 15.01M, SickLeaveRequested = 5.08M },
				new PlantPayLine{ QuickBaseRecordId = 7288001, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 12M, TotalGross = 0M, HourlyRateOverride = 15.05M, SickLeaveRequested = 5.09M },
				new PlantPayLine{ QuickBaseRecordId = 7288002, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 13M, TotalGross = 0M, HourlyRateOverride = 15.99M, SickLeaveRequested = 5.10M },
				new PlantPayLine{ QuickBaseRecordId = 7288003, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 14.99M, TotalGross = 0M, HourlyRateOverride = 16.03M, SickLeaveRequested = 5.11M },
				new PlantPayLine{ QuickBaseRecordId = 7288004, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.00M, TotalGross = 0M, HourlyRateOverride = 17.08M, SickLeaveRequested = 5.12M },
				new PlantPayLine{ QuickBaseRecordId = 7288005, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.01M, TotalGross = 0M, HourlyRateOverride = 17.98M, SickLeaveRequested = 5.13M },
				new PlantPayLine{ QuickBaseRecordId = 7288006, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 503, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.02M, TotalGross = 0M, HourlyRateOverride = 17.99M, SickLeaveRequested = 5.14M }
			};

			var response = _repo.Save(PlantPayLines);
			Console.WriteLine(response);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_WithLayoff()
		{
			var layoffId = 98;
			var PlantPayLines = new List<PlantPayLine>
			{
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, StartTime = new DateTime(2019, 1, 1, 7, 30, 0), EndTime = new DateTime(2019, 1, 1, 15, 45, 0), SickLeaveRequested = 0M},
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.00M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.01M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.02M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.03M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.04M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.05M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, SickLeaveRequested = 5.06M },
				new PlantPayLine{ QuickBaseRecordId = 7287999, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.OverTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 10M, TotalGross = 0M, HourlyRateOverride = 15M, SickLeaveRequested = 5.07M },
				new PlantPayLine{ QuickBaseRecordId = 7288000, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "778973", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 11M, TotalGross = 0M, HourlyRateOverride = 15.01M, SickLeaveRequested = 5.08M },
				new PlantPayLine{ QuickBaseRecordId = 7288001, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 12M, TotalGross = 0M, HourlyRateOverride = 15.05M, SickLeaveRequested = 5.09M },
				new PlantPayLine{ QuickBaseRecordId = 7288002, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 122, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 13M, TotalGross = 0M, HourlyRateOverride = 15.99M, SickLeaveRequested = 5.10M },
				new PlantPayLine{ QuickBaseRecordId = 7288003, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 14.99M, TotalGross = 0M, HourlyRateOverride = 16.03M, SickLeaveRequested = 5.11M },
				new PlantPayLine{ QuickBaseRecordId = 7288004, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.00M, TotalGross = 0M, HourlyRateOverride = 17.08M, SickLeaveRequested = 5.12M },
				new PlantPayLine{ QuickBaseRecordId = 7288005, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 114, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.01M, TotalGross = 0M, HourlyRateOverride = 17.98M, SickLeaveRequested = 5.13M },
				new PlantPayLine{ QuickBaseRecordId = 7288006, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 503, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.02M, TotalGross = 0M, HourlyRateOverride = 17.99M, SickLeaveRequested = 5.14M }
			};

			var response = _repo.Save(PlantPayLines);
			Console.WriteLine(response);
		}


		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_CommasInStrings()
		{
			var layoffId = 0;
			var PlantPayLines = new List<PlantPayLine>
			{
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M, StartTime = new DateTime(2019, 1, 1, 7, 30, 0), EndTime = new DateTime(2019, 1, 1, 15, 45, 0)},
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 0, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7287999, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 10M, TotalGross = 0M, HourlyRateOverride = 15M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288000, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "778973,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 11M, TotalGross = 0M, HourlyRateOverride = 15.01M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288001, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 12M, TotalGross = 0M, HourlyRateOverride = 15.05M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288002, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C,", LaborCode = 122, HoursWorked = 9M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 13M, TotalGross = 0M, HourlyRateOverride = 15.99M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288003, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 14.99M, TotalGross = 0M, HourlyRateOverride = 16.03M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288004, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.00M, TotalGross = 0M, HourlyRateOverride = 17.08M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288005, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C,", LaborCode = 114, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.01M, TotalGross = 0M, HourlyRateOverride = 17.98M },
				new PlantPayLine{ BoxStyleDescription = "Testing,the,use,of,commas.", QuickBaseRecordId = 7288006, LayoffId = layoffId, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C,", LaborCode = 503, HoursWorked = 10M, PayType = "42-Comma,", HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.02M, TotalGross = 0M, HourlyRateOverride = 17.99M }
			};

			var response = _repo.Save(PlantPayLines);
			Console.WriteLine(response);
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
