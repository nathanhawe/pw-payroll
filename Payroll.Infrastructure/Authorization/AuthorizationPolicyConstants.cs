using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.Authorization
{
	public static class AuthorizationPolicyConstants
	{
		public const string SubjectMustMatchUser = "SubjectMustMatchUser";
		public const string MustBeViewingUser = "MustBeViewingUser";
		public const string MustBeBatchCreatingUser = "MustBeBatchCreatingUser";
		public const string MustBeAdministrationUser = "MustBeAdministrationUser";
	}
}
