using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickBase.Api;
using System;

namespace QuickBase.IntegrationTests
{
	[TestClass]
	public class DoQueryCountTests
	{
		private IConfigurationRoot Configuration;

		[TestInitialize]
		public void Setup()
		{
			if(Configuration == null)
			{
				Configuration = ConfigurationHelper.GetIConfigurationRoot();
			}
		}

		[TestMethod]
		public void DoQueryCount_CrewBossPay()
		{
			var dbid = "be7eyypdu";
			var realm = Configuration["QuickBase:Realm"];
			var userToken = Configuration["QuickBase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();

			var qb = new QuickBaseConnection(realm, userToken, logger);
			Console.WriteLine(qb.DoQueryCount(dbid, "{3.GT.0}", "DoQueryCount_CrewBossPay"));
		}

		[TestMethod]
		public void DoQuery_CrewBossPay()
		{
			var dbid = "be7eyypdu";
			var realm = Configuration["QuickBase:Realm"];
			var userToken = Configuration["QuickBase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();

			var qb = new QuickBaseConnection(realm, userToken, logger);
			Console.WriteLine(qb.DoQuery(dbid, "{9.IR.2020-03-28}", "3.6.7.8", "6", "num-2.sortorder-D.skp-2", ""));
		}

		[TestMethod]
		public void DoQuery_CrewBossPay_WithQueryName()
		{
			var dbid = "be7eyypdu";
			var realm = Configuration["QuickBase:Realm"];
			var userToken = Configuration["QuickBase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();

			var qb = new QuickBaseConnection(realm, userToken, logger);
			Console.WriteLine(qb.DoQuery(dbid, "CB worked less than 6 Hours and not on Hourly Rate", "3.6.7.8", "6", "num-5", "", queryIsName: true));
		}

		[TestMethod]
		public void DoQuery_CrewBossPay_WithQid()
		{
			var dbid = "be7eyypdu";
			var realm = Configuration["QuickBase:Realm"];
			var userToken = Configuration["QuickBase:UserToken"];
			var logger = new MockLogger<QuickBaseConnection>();

			var qb = new QuickBaseConnection(realm, userToken, logger);
			Console.WriteLine(qb.DoQuery(dbid, 6, "3.6.7.8", "6", "num-5", ""));
		}
	}
}
