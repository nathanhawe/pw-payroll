using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.Authorization
{
	public class AccessLevelRequirement : IAuthorizationRequirement
	{
		public List<string> PermittedAccessLevels { get; } = new List<string>();

		public AccessLevelRequirement(string permittedAccessLevel)
		{
			PermittedAccessLevels.Add(permittedAccessLevel);
		}
		public AccessLevelRequirement(params string[] permittedAccessLevels)
		{
			PermittedAccessLevels.AddRange(permittedAccessLevels);
		}
	}

}
