using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.Authorization
{
	public class AccessLevelRequirement : IAuthorizationRequirement
	{
		public string RequiredAccessLevel { get; }

		public AccessLevelRequirement(string requiredAccessLevel)
		{
			RequiredAccessLevel = requiredAccessLevel;
		}
	}

}
