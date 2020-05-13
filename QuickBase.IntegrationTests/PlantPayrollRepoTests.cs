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
	public class PlantPayrollRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private PlantPayrollRepo _repo;

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
			_repo = new PlantPayrollRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_NoLayoff()
		{
			var temp = _repo.Get(new DateTime(2019, 1, 6), 0);
			Print(temp);
		}

		[TestMethod]
		public void DoQuery_WithLayoff()
		{
			var temp = _repo.Get(new DateTime(2019, 1, 6), 96);
			Print(temp);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV()
		{
			var PlantPayLines = new List<PlantPayLine>
			{
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M },
				new PlantPayLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, HourlyRateOverride = 0M }
			};

			var response = _repo.Save(PlantPayLines);
			Console.WriteLine(response);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_Update()
		{
			var PlantPayLines = new List<PlantPayLine>
			{
				new PlantPayLine{ QuickBaseRecordId = 7287999, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.OverTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 10M, TotalGross = 0M, HourlyRateOverride = 15M },
				new PlantPayLine{ QuickBaseRecordId = 7288000, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "778973", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 11M, TotalGross = 0M, HourlyRateOverride = 15.01M },
				new PlantPayLine{ QuickBaseRecordId = 7288001, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 12M, TotalGross = 0M, HourlyRateOverride = 15.05M },
				new PlantPayLine{ QuickBaseRecordId = 7288002, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 13M, TotalGross = 0M, HourlyRateOverride = 15.99M },
				new PlantPayLine{ QuickBaseRecordId = 7288003, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 14.99M, TotalGross = 0M, HourlyRateOverride = 16.03M },
				new PlantPayLine{ QuickBaseRecordId = 7288004, LayoffId = 96, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.00M, TotalGross = 0M, HourlyRateOverride = 17.08M },
				new PlantPayLine{ QuickBaseRecordId = 7288005, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.01M, TotalGross = 0M, HourlyRateOverride = 17.98M },
				new PlantPayLine{ QuickBaseRecordId = 7288006, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 15.02M, TotalGross = 0M, HourlyRateOverride = 17.99M }
			};

			var response = _repo.Save(PlantPayLines);
			Console.WriteLine(response);
		}


		private void Print(IEnumerable<PlantPayLine> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Plant Pay Lines:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PlantPayLine line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(PlantPayLine).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
