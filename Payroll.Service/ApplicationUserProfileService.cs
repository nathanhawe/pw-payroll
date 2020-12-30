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
		/// Returns all <c>ApplicationUserProfile</c>s with pagination.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="limit"></param>
		/// <param name="orderByDescending"></param>
		/// <returns></returns>
		public List<ApplicationUserProfile> GetApplicationUserProfiles(int offset, int limit, bool orderByDescending)
		{
			if (offset < 0) offset = 0;

			var query = _context.ApplicationUserProfiles.AsQueryable();
			if (orderByDescending)
			{
				query = query.OrderByDescending(o => o.Name);
			}
			else
			{
				query = query.OrderBy(o => o.Name);
			}

			return query
				.Skip(offset * limit)
				.Take(limit)
				.ToList();
		}

		/// <summary>
		/// Returns the <c>ApplicationUserProfile</c> for the provided subject.  This method returns null
		/// if no profile is found.
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public ApplicationUserProfile GetApplicationUserProfileFromSubject(string subject)
		{
			return _context.ApplicationUserProfiles.Where(x => x.Subject == subject).FirstOrDefault();
		}

		/// <summary>
		/// Return the <c>ApplicationUserProfile</c> for the provided ID.  This method returns null
		/// if no profile is found.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ApplicationUserProfile GetApplicationUserProfileFromId(string id)
		{
			var guidId = Guid.Parse(id);
			return _context.ApplicationUserProfiles.Where(x => x.Id == guidId).FirstOrDefault();
		}

		/// <summary>
		/// Returns the count of all <c>ApplicationUserProfile</c> records in the database.
		/// </summary>
		/// <returns></returns>
		public int GetTotalCount()
		{
			return _context.ApplicationUserProfiles.Count();
		}

		/// <summary>
		/// Updates the <c>ApplicationUserProfile</c> with the provided ID.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="applicationUserProfile"></param>
		/// <returns></returns>
		public ApplicationUserProfile UpdateApplicationUserProfile(
			string id, 
			ApplicationUserProfile applicationUserProfile)
		{
			if (applicationUserProfile == null) throw new ArgumentNullException(nameof(applicationUserProfile));
			var guidId = Guid.Parse(id);

			var existingProfile = _context.ApplicationUserProfiles
				.Where(x => x.Id == guidId)
				.FirstOrDefault();

			if (existingProfile == null) throw new Exception($"ApplicationUserProfile with ID '{id}' was not found.");

			existingProfile.Name = applicationUserProfile.Name;
			existingProfile.Email = applicationUserProfile.Email;
			existingProfile.AccessLevel = applicationUserProfile.AccessLevel;
			_context.SaveChanges();

			return existingProfile;
		}
	}
}
