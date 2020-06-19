using Microsoft.AspNetCore.Authorization;

namespace Payroll.Infrastructure.Authorization
{
	public class SubjectMustMatchUserRequirement : IAuthorizationRequirement
	{
		public SubjectMustMatchUserRequirement() { }
	}
}
