using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
	internal class MockApplicationUserProfileService : Payroll.Service.Interface.IApplicationUserProfileService
	{
		public List<ApplicationUserProfile> ApplicationUserProfiles { get; set; } = new List<ApplicationUserProfile>();
		
		public void AddApplicationUserProfile(ApplicationUserProfile applicationUserProfile)
		{
			throw new NotImplementedException();
		}

		public bool ApplicationUserProfileExists(string subject)
		{
			throw new NotImplementedException();
		}

		public ApplicationUserProfile GetApplicationUserProfileFromId(string id)
		{
			throw new NotImplementedException();
		}

		public ApplicationUserProfile GetApplicationUserProfileFromSubject(string subject)
		{
			return ApplicationUserProfiles.Where(x => x.Subject == subject).FirstOrDefault();
		}

		public List<ApplicationUserProfile> GetApplicationUserProfiles(int offset, int limit, bool orderByDescending)
		{
			throw new NotImplementedException();
		}

		public int GetTotalCount()
		{
			throw new NotImplementedException();
		}

		public ApplicationUserProfile UpdateApplicationUserProfile(string id, ApplicationUserProfile applicationUserProfile)
		{
			throw new NotImplementedException();
		}
	}
}
