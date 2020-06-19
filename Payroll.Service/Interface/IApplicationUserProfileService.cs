using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IApplicationUserProfileService
	{
		public ApplicationUserProfile GetApplicationUserProfile(string subject);
		public bool ApplicationUserProfileExists(string subject);
		public void AddApplicationUserProfile(ApplicationUserProfile applicationUserProfile);
	}
}
