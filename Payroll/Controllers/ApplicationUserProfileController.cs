using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payroll.Models;
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
		private readonly IUserActionService _userActionService;

		public ApplicationUserProfileController(
			ILogger<ApplicationUserProfileController> logger,
			IApplicationUserProfileService applicationUserProfileService,
			IUserActionService userActionService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_applicationUserProfileService = applicationUserProfileService ?? throw new ArgumentNullException(nameof(applicationUserProfileService));
			_userActionService = userActionService ?? throw new ArgumentNullException(nameof(userActionService));
		}

		[HttpGet]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<IEnumerable<Domain.ApplicationUserProfile>>> Get(
			int offset = 0,
			int limit = 20,
			bool orderByDescending = true)
		{
			var profiles = _applicationUserProfileService.GetApplicationUserProfiles(offset, limit, orderByDescending);
			int batchCount = _applicationUserProfileService.GetTotalCount();

			return Ok(new Models.ApiResponse<IEnumerable<Domain.ApplicationUserProfile>>
			{
				Data = profiles,
				Pagination = new Models.Pagination
				{
					Offset = offset,
					Limit = limit,
					Total = batchCount,
					OrderByDescending = orderByDescending
				}
			});
		}

		[HttpGet("{id}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<ApiResponse<IEnumerable<Domain.ApplicationUserProfile>>> Get(
			string id)
		{
			var profile = _applicationUserProfileService.GetApplicationUserProfileFromId(id);

			return Ok(new Models.ApiResponse<Domain.ApplicationUserProfile>
			{
				Data = profile,
			});
		}

		[HttpPut("{id}")]
		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.MustBeAdministrationUser)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Models.ApiResponse<Domain.ApplicationUserProfile>> Put(
			string id,
			[FromBody]Models.ApplicationUserProfileViewModel viewModel)
		{
			var errors = new Dictionary<string, List<string>>();
			var userProfile = new Domain.ApplicationUserProfile
			{
				Email = viewModel.Email,
				Name = viewModel.Name,
				AccessLevel = viewModel.AccessLevel
			};
			
			try
			{
				// Ensure that the current user is not downgrading their access level
				var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
				var updatedUserProfile = _applicationUserProfileService.GetApplicationUserProfileFromId(id);
				if(updatedUserProfile.Subject != subjectFromToken || userProfile.AccessLevel == Payroll.Domain.Constants.AccessLevel.Administrator.ToString())
				{
					updatedUserProfile = _applicationUserProfileService.UpdateApplicationUserProfile(id, userProfile);
					LogUserAction("PUT", updatedUserProfile);
					return Ok(
						new ApiResponse<Domain.ApplicationUserProfile>
						{
							Data = updatedUserProfile
						});
				}
				else
				{
					errors.Add("accessLevel", new List<string> { "Users cannot change their own access level." });
				}
			}
			catch(Exception ex)
			{
				_logger.Log(LogLevel.Error, "Unable to update {ApplicationUserProfile} because an exception occured. {Exception}", viewModel, ex);
				errors.Add("", new List<string> { "Unable to complete update because an error occured." });
			}

			return BadRequest(
				new ApiResponse<Domain.ApplicationUserProfile>
				{
					Errors = errors,
				});
		}

		[Authorize(Policy = Infrastructure.Authorization.AuthorizationPolicyConstants.SubjectMustMatchUser)]
		[HttpGet("FromSubject/{subject}", Name = "GetApplicationUserProfile")]
		public IActionResult GetApplicationUserProfile(
			string subject, 
			string email,
			string name)
		{
			var applicationUserProfileFromRepo = _applicationUserProfileService.GetApplicationUserProfileFromSubject(subject);

			if (applicationUserProfileFromRepo == null)
			{
				// subject must come from token
				var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

				applicationUserProfileFromRepo = new Domain.ApplicationUserProfile()
				{
					Subject = subject,
					AccessLevel = Domain.Constants.AccessLevel.Viewer.ToString(),
					Name = name,
					Email = email
				};

				_applicationUserProfileService.AddApplicationUserProfile(applicationUserProfileFromRepo);
			}

			return Ok(
				new ApiResponse<Domain.ApplicationUserProfile>
				{ 
					Data = applicationUserProfileFromRepo
				});
		}

		[NonAction]
		private void LogUserAction(string action, Domain.ApplicationUserProfile applicationUser)
		{
			var subjectFromToken = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
			var message = JsonSerializer.Serialize(applicationUser);
			_userActionService.AddActionForSubject(subjectFromToken, $"Application User Profile ({action}) '{message}'");
		}
	}
}
