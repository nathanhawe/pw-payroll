using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payroll.Service.Interface;

namespace Payroll.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]

	public class ApplicationUserProfileController : ControllerBase
	{
		private readonly ILogger<ApplicationUserProfileController> _logger;
		private readonly IApplicationUserProfileService _applicationUserProfileService;

		public ApplicationUserProfileController(
			ILogger<ApplicationUserProfileController> logger,
			IApplicationUserProfileService applicationUserProfileService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_applicationUserProfileService = applicationUserProfileService ?? throw new ArgumentNullException(nameof(applicationUserProfileService));
		}

		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.SubjectMustMatchUser)]
		[HttpGet("{subject}", Name = "GetApplicationUserProfile")]
		public IActionResult GetApplicationUserProfile(string subject)
		{
			var applicationUserProfileFromRepo = _applicationUserProfileService.GetApplicationUserProfile(subject);

			if (applicationUserProfileFromRepo == null)
			{
				// subject must come from token
				var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

				applicationUserProfileFromRepo = new Domain.ApplicationUserProfile()
				{
					Subject = subject,
					AccessLevel = Domain.Constants.AccessLevel.Viewer.ToString()
				};

				_applicationUserProfileService.AddApplicationUserProfile(applicationUserProfileFromRepo);
			}

			return Ok(applicationUserProfileFromRepo);
		}
	}
}
