using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Data;
using Payroll.Service;
using System.Linq;

namespace Payroll.UnitTest
{
	[TestClass]
	public class UserActionServiceTests
	{
		[TestMethod]
		public void AddsUserAction()
		{
			var dbName = "AddsUserAction";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var testUser = "Tester";
			var testDescription = "This is a test description.";

			using var context = new PayrollContext(options);
			var userActionService = new UserActionService(context);

			if (context.UserActions.Count() > 0) Assert.Inconclusive("Expected no existing UserActions when test starts.");
			userActionService.AddAction(testUser, testDescription);

			Assert.AreEqual(1, context.UserActions.Count());
			Assert.AreEqual(1, context.UserActions.Where(x => x.User == testUser && x.Action == testDescription).Count());
		}
	}
}
