using IdentityModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PrimaCompany.IDP.Exceptions;
using PrimaCompany.IDP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.UserRegistration
{
	public class UserRegistrationController : Controller
	{
		private readonly ILogger<UserRegistrationController> _logger;
		private readonly ILocalUserService _localUserService;
		private readonly IIdentityServerInteractionService _interaction;

		public UserRegistrationController(
			ILogger<UserRegistrationController> logger,
			ILocalUserService localUserService,
			IIdentityServerInteractionService interaction)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
			_interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
		}

		[HttpGet]
		public IActionResult RegisterUser(string returnUrl)
		{
			var vm = new RegisterUserViewModel()
			{
				ReturnUrl = returnUrl
			};

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var userToCreate = new Entities.User
			{
				Username = model.Username,
				Subject = Guid.NewGuid().ToString(),
				Email = model.Email,
				Active = true
			};

			userToCreate.Claims.Add(new Entities.UserClaim()
			{
				Type = JwtClaimTypes.GivenName,
				Value = model.GivenName
			});

			userToCreate.Claims.Add(new Entities.UserClaim()
			{
				Type = JwtClaimTypes.FamilyName,
				Value = model.FamilyName
			});

			try
			{
				_localUserService.AddUser(userToCreate, model.Password);
				await _localUserService.SaveChangesAsync();

				// lot the user in after successfull registration
				await HttpContext.SignInAsync(userToCreate.Subject, userToCreate.Username);

				// continue with the flow
				if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
				{
					return Redirect(model.ReturnUrl);
				}

				return Redirect("~/");
			}
			catch (UsernameIsNotUniqueException)
			{
				ModelState.AddModelError("Username", "This username has already been registered.");
				return View(model);
			}
			catch (EmailIsNotUniqueException)
			{
				ModelState.AddModelError("Email", "This email address has already been registered.");
				return View(model);
			}
			catch (EmailIsInvalidException)
			{
				ModelState.AddModelError("Email", "Your email must be a valid @prima.com, @wawonapacking.com, or @gerawan.com address.");
				return View(model);
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, "An exception occurred during user registration. {Exception}", ex);
				ModelState.AddModelError("", "An unknown error occurred.");
				return View(model);
			}
		}
	}
}
