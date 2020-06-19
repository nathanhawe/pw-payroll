using Payroll.Data;
using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Service that handles interaction with the Applition User Profiles
	/// </summary>
	public class ApplicationUserProfileService : IApplicationUserProfileService
	{
		private readonly PayrollContext _context;

		public ApplicationUserProfileService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Adds a <c>ApplicationUserProfile</c> to the database context.
		/// </summary>
		/// <param name="applicationUserProfile"></param>
		public void AddApplicationUserProfile(ApplicationUserProfile applicationUserProfile)
		{
			_context.ApplicationUserProfiles.Add(applicationUserProfile);
			_context.SaveChanges();
		}

		/// <summary>
		/// Returns true if an <c>ApplicationUserProfile</c> exists for provided subject.
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public bool ApplicationUserProfileExists(string subject)
		{
			return _context.ApplicationUserProfiles.Any(a => a.Subject == subject);
		}

		/// <summary>
		/// Returns the <c>ApplicationUserProfile</c> for provided subject.  This method return null
		/// if now profile is found.
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public ApplicationUserProfile GetApplicationUserProfile(string subject)
		{
			return _context.ApplicationUserProfiles.Where(x => x.Subject == subject).FirstOrDefault();
		}
	}
}
