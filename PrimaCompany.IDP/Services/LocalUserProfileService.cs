﻿using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.Services
{
	public class LocalUserProfileService : IProfileService
	{
		private readonly ILocalUserService _localUserService;

		public LocalUserProfileService(ILocalUserService localUserService)
		{
			_localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
		}
		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var subjectId = context.Subject.GetSubjectId();
			var claimsForUser = (await _localUserService.GetUserClaimsBySubjectAsyc(subjectId)).ToList();

			context.AddRequestedClaims(claimsForUser.Select(c => new System.Security.Claims.Claim(c.Type, c.Value)).ToList());
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			var subjectId = context.Subject.GetSubjectId();
			context.IsActive = await _localUserService.IsUserActive(subjectId);
		}
	}
}
