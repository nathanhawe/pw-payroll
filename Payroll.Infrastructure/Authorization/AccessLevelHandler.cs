using Microsoft.AspNetCore.Authorization;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Infrastructure.Authorization
{
	public class AccessLevelHandler : AuthorizationHandler<AccessLevelRequirement>
	{
		private readonly IApplicationUserProfileService _applicationUserProfileService;

		public AccessLevelHandler(
			IApplicationUserProfileService applicationUserProfileService)
		{
			_applicationUserProfileService = applicationUserProfileService ?? throw new ArgumentNullException(nameof(applicationUserProfileService));
		}
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessLevelRequirement requirement)
		{
			var subject = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

			var accessLevel = _applicationUserProfileService.GetApplicationUserProfile(subject)?.AccessLevel;

			foreach(string permittedAccessLevel in requirement.PermittedAccessLevels)
			{
				if(accessLevel == permittedAccessLevel)
				{
					context.Succeed(requirement);
					return Task.CompletedTask;
				}
			}

			context.Fail();
			return Task.CompletedTask;
		}
	}
}
