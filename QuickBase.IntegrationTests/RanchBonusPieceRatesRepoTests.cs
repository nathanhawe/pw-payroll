using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using QuickBase.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickBase.IntegrationTests
{
	[TestClass]
	public class RanchBonusPieceRatesRepoTests
	{
		private IConfigurationRoot _configuration;
		private QuickBaseConnection _quickBaseConn;
		private RanchBonusPieceRatesRepo _repo;

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
			_repo = new RanchBonusPieceRatesRepo(_quickBaseConn);
		}

		[TestMethod]
		public void DoQuery_All()
		{
			var temp = _repo.Get();
			Print(temp);
		}
				

		private void Print(IEnumerable<RanchBonusPieceRate> lines)
		{
			Console.WriteLine($"There are '{lines.Count()}' RanchBonusPieceRates:");
			foreach(var line in lines)
			{
				Print(line);
			}
		}

		private void Print(RanchBonusPieceRate line)
		{
			Console.WriteLine($"Record #{line.QuickBaseRecordId}");
			var properties = typeof(RanchBonusPieceRate).GetProperties();
			foreach(var property in properties)
			{
				Console.Write($"     {property.Name}: '{property.GetValue(line)}'");
			}
		}
	}
}
