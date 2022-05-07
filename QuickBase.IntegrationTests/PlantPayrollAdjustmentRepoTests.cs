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
	public class PlantPayrollAdjustmentRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private PlantPayrollAdjustmentRepo _repo;

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
			_repo = new PlantPayrollAdjustmentRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_NoLayoff()
		{
			var temp = _repo.Get(new DateTime(2022, 5, 1), 0);
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
			var PlantAdjustmentLines = new List<PlantAdjustmentLine>
			{
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.96M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2937D", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.97M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4870C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.98M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "4867C", LaborCode = 207, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.99M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false },
				new PlantAdjustmentLine{ QuickBaseRecordId = 0, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 208, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = 0M, OtherGross = 0M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 0M, UseOldHourlyRate = false }
			};

			var response = _repo.Save(PlantAdjustmentLines);
			Console.WriteLine(response);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_Update()
		{
			var PlantAdjustmentLines = new List<PlantAdjustmentLine>
			{
				new PlantAdjustmentLine{ QuickBaseRecordId = 849853, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "2518C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.OverTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .91M, OtherGross = 10M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.96M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849854, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "778973", LaborCode = 123, HoursWorked = 9M, PayType = PayType.DoubleTime, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .92M, OtherGross = 10M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.97M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849855, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "4870C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.Pieces, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .93M, OtherGross = 20M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.98M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849856, LayoffId = 0, ShiftDate = new DateTime(2019, 1, 1), Plant = 2, EmployeeId = "4867C", LaborCode = 123, HoursWorked = 9M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .94M, OtherGross = 20M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = true, OldHourlyRate = 10.99M, UseOldHourlyRate = true },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849857, LayoffId = 96, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .95M, OtherGross = 30M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849858, LayoffId = 96, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2937D", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .96M, OtherGross = 30M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849859, LayoffId = 96, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4870C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .97M, OtherGross = 40M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false },
				new PlantAdjustmentLine{ QuickBaseRecordId = 849860, LayoffId = 96, ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "4867C", LaborCode = 123, HoursWorked = 10M, PayType = PayType.Regular, HourlyRate = 0M, GrossFromHours = 0M, GrossFromPieces = 0M, GrossFromIncentive = .98M, OtherGross = 41.92M, TotalGross = 0M, WeekEndOfAdjustmentPaid = new DateTime(2019, 1, 6), IsOriginal = false, OldHourlyRate = 0M, UseOldHourlyRate = false }
			};

			var response = _repo.Save(PlantAdjustmentLines);
			Console.WriteLine(response);
		}

		[TestMethod]
		[Ignore("This test can create new records in Quick Base.")]
		public void ImportFromCSV_CommasInString()
		{
			var PlantAdjustmentLines = new List<PlantAdjustmentLine>
			{
				new PlantAdjustmentLine{ ShiftDate = new DateTime(2019, 1, 1), Plant = 5, EmployeeId = "2518,C", PayType = "42-Comma," },
				new PlantAdjustmentLine{ ShiftDate = new DateTime(2019, 1, 2), Plant = 5, EmployeeId = "2518,C", PayType = "42-Comma," },
				new PlantAdjustmentLine{ ShiftDate = new DateTime(2019, 1, 3), Plant = 5, EmployeeId = "2518,C", PayType = "42-Comma," },
			};

			var response = _repo.Save(PlantAdjustmentLines);
			Console.WriteLine(response);
		}


		private void Print(IEnumerable<PlantAdjustmentLine> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Plant Adjustment Lines:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PlantAdjustmentLine line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(PlantAdjustmentLine).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
