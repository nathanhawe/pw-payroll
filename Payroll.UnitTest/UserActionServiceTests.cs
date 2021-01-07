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
		public void AddsUserActionFromId()
		{
			var dbName = "AddsUserActionFromId";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var testUser = "Tester";
			var testDescription = "This is a test description.";
			var knownSubject = "ac99ba8e-343f-43a9-8af7-70781abec008";
			var knownId = System.Guid.Parse("57b7afdc-5b15-480d-e132-08d8a79379c7");

			var mockLogger = new Mocks.MockLogger<Payroll.Service.UserActionService>();
			var mockApplicationUserService = new Mocks.MockApplicationUserProfileService();
			mockApplicationUserService.ApplicationUserProfiles.Add(new Domain.ApplicationUserProfile
			{
				Id = knownId,
				Subject = knownSubject,
				AccessLevel = Payroll.Domain.Constants.AccessLevel.Viewer.ToString(),
				Name = "Tester McTesterface",
				Email = "t.mctesterface@prima.com",
			});

			using var context = new PayrollContext(options);
			var userActionService = new UserActionService(context, mockApplicationUserService, mockLogger);

			if (context.UserActions.Count() > 0) Assert.Inconclusive("Expected no existing UserActions when test starts.");
			userActionService.AddAction(testUser, testDescription);

			Assert.AreEqual(1, context.UserActions.Count());
			Assert.AreEqual(1, context.UserActions.Where(x => x.User == testUser && x.Action == testDescription).Count());
		}

		[TestMethod]
		public void AddsUserActionFromSubject()
		{
			var dbName = "AddsUserActionFromSubject";
			var options = new DbContextOptionsBuilder<PayrollContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
			var testDescription = "This is a test description.";
			var knownSubject = "ac99ba8e-343f-43a9-8af7-70781abec008";
			var knownId = System.Guid.Parse("57b7afdc-5b15-480d-e132-08d8a79379c7");

			var mockLogger = new Mocks.MockLogger<Payroll.Service.UserActionService>();
			var mockApplicationUserService = new Mocks.MockApplicationUserProfileService();
			mockApplicationUserService.ApplicationUserProfiles.Add(new Domain.ApplicationUserProfile
			{
				Id = knownId,
				Subject = knownSubject,
				AccessLevel = Payroll.Domain.Constants.AccessLevel.Viewer.ToString(),
				Name = "Tester McTesterface",
				Email = "t.mctesterface@prima.com",
			});

			using var context = new PayrollContext(options);
			var userActionService = new UserActionService(context, mockApplicationUserService, mockLogger);

			if (context.UserActions.Count() > 0) Assert.Inconclusive("Expected no existing UserActions when test starts.");
			userActionService.AddActionForSubject(knownSubject, testDescription);

			Assert.AreEqual(1, context.UserActions.Count());
			Assert.AreEqual(1, context.UserActions.Where(x => x.User == knownId.ToString() && x.Action == testDescription).Count());
		}
	}
}
