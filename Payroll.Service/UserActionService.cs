using Payroll.Data;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Class that handles the creation and logging of user actions.
	/// </summary>
	public class UserActionService : IUserActionService
	{
		private readonly PayrollContext _context;
		public UserActionService(PayrollContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Creates a new <c>UserAction</c> and inserts it into the database.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="actionDescription"></param>
		public void AddAction(string userId, string actionDescription)
		{
			_context.UserActions.Add(new Domain.UserAction
			{
				User = userId,
				Action = actionDescription,
			});
			_context.SaveChanges();
		}
	}
}
