using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IUserActionService
	{
		void AddAction(string userId, string actionDescription);
	}
}
