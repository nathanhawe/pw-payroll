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
	public class PslTrackingDailyRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private PslTrackingDailyRepo _repo;

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
			_repo = new PslTrackingDailyRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery()
		{
			var temp = _repo.Get(new DateTime(2019, 6, 1), new DateTime(2019, 9, 30), Company.Plants);
			Print(temp);
		}

		[TestMethod, Ignore("This test can create Quick Base records when run.")]
		public void ImportFromCSV()
		{
			var paidSickLeaves = new List<PaidSickLeave>
			{
				new PaidSickLeave{ EmployeeId = "2519C", ShiftDate = new DateTime(2016, 1, 1), Company = Company.Plants, Hours = 9.5M, Gross = 1000.99M, NinetyDayHours = 1000.99M, NinetyDayGross = 11231.42M, HoursUsed = 0 },
				new PaidSickLeave{ EmployeeId = "2937D", ShiftDate = new DateTime(2016, 1, 1), Company = Company.Ranches, Hours = 9.5M, Gross = 1000.99M, NinetyDayHours = 1000.99M, NinetyDayGross = 11231.42M, HoursUsed = 0 },
				new PaidSickLeave{ EmployeeId = "4870C", ShiftDate = new DateTime(2016, 1, 1), Company = Company.Plants, Hours = 9.5M, Gross = 1000.99M, NinetyDayHours = 1000.99M, NinetyDayGross = 11231.42M, HoursUsed = 0 },
				new PaidSickLeave{ EmployeeId = "4867C", ShiftDate = new DateTime(2016, 1, 1), Company = Company.Plants, Hours = 9.5M, Gross = 1000.99M, NinetyDayHours = 1000.99M, NinetyDayGross = 11231.42M, HoursUsed = 0 }
			};

			var response = _repo.Save(paidSickLeaves);
			Console.WriteLine(response);
		}

		private void Print(IEnumerable<PaidSickLeave> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' Paid Sick Leaves:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(PaidSickLeave line)
		{
			Console.WriteLine($"---");
			var properties = typeof(PaidSickLeave).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
