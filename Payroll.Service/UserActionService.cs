using Microsoft.Extensions.Logging;
using Payroll.Data;
using Payroll.Service.Interface;
using System;

namespace Payroll.Service
{
	/// <summary>
	/// Class that handles the creation and logging of user actions.
	/// </summary>
	public class UserActionService : IUserActionService
	{
		private readonly PayrollContext _context;
		private readonly IApplicationUserProfileService _applicationUserProfileService;
		private readonly ILogger<UserActionService> _logger;

		public UserActionService(
			PayrollContext context, 
			IApplicationUserProfileService applicationUserProfileService, 
			ILogger<UserActionService> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_applicationUserProfileService = applicationUserProfileService ?? throw new ArgumentNullException(nameof(applicationUserProfileService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Creates a new <c>UserAction</c> and inserts it into the database.  This method will not throw an exception.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="actionDescription"></param>
		public void AddAction(string userId, string actionDescription)
		{
			try
			{
				_context.UserActions.Add(new Domain.UserAction
				{
					User = userId,
					Action = actionDescription,
				});
				_context.SaveChanges();
			}
			catch(Exception ex)
			{
				_logger.LogError("Unable to log user action for {User}:{Action} because an exception occurred. {Exception}", userId, actionDescription, ex);
			}
		}

		/// <summary>
		/// Creates a new <c>UserAction</c> and inserts it into the database for the user associated with the passed subject. This method will not throw an exception.
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="actionDescription"></param>
		public void AddActionForSubject(string subject, string actionDescription)
		{
			var userId = subject;
			try
			{
				var applicationUser = _applicationUserProfileService.GetApplicationUserProfileFromSubject(subject);
				if(applicationUser != null)
				{
					userId = applicationUser.Id.ToString();
				}
				else
				{
					actionDescription = "(User is Subject - Unable to Identify)" + actionDescription;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Unable to find user for {Subject} becuase an exception occured. {Exception}", subject, ex);
				actionDescription = "(User is Subject - Exception Occurred)" + actionDescription;
			}

			AddAction(userId, actionDescription);
		}
	}
}
