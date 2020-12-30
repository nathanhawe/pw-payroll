using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IApplicationUserProfileService
	{
		public ApplicationUserProfile GetApplicationUserProfileFromSubject(string subject);
		public ApplicationUserProfile GetApplicationUserProfileFromId(string id);
		public List<ApplicationUserProfile> GetApplicationUserProfiles(int offset, int limit, bool orderByDescending);
		public void AddApplicationUserProfile(ApplicationUserProfile applicationUserProfile);
		public ApplicationUserProfile UpdateApplicationUserProfile(string id, ApplicationUserProfile applicationUserProfile);
		public bool ApplicationUserProfileExists(string subject);
		public int GetTotalCount();
	}
}
