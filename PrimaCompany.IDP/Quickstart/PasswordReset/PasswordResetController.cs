using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PrimaCompany.IDP.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.PasswordReset
{
	public class PasswordResetController : Controller
	{
		private readonly ILogger<PasswordResetController> _logger;
		private readonly ILocalUserService _localUserService;
		private readonly IEmailService _emailService;

		public PasswordResetController(
			ILogger<PasswordResetController> logger,
			ILocalUserService localUserService,
			IEmailService emailService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
		}

		[HttpGet]
		public IActionResult RequestPassword()
		{
			var vm = new RequestPasswordViewModel();
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RequestPassword(RequestPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				var securityCode = await _localUserService.InitiatePasswordResetRequest(model.Email);
				await _localUserService.SaveChangesAsync();

				// create reset link and send it to the user's email address
				var link = Url.ActionLink("ResetPassword", "PasswordReset", new { securityCode });
				_emailService.SendEmail(model.Email, "Password Reset", $"Reset your password by following this link: {link}");
			}
			catch(Exception ex)
			{
				_logger.Log(LogLevel.Error, "Unable to complete password reset request for the email '{email}' because an exception was thrown. {exception}", model.Email, ex);
			}

			return View("PasswordResetRequestSent");
		}

		[HttpGet]
		public IActionResult ResetPassword(string securityCode)
		{
			var vm = new ResetPasswordViewModel()
			{
				SecurityCode = securityCode
			};

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var success = false;


			try
			{
				success = await _localUserService.SetPassword(model.SecurityCode, model.Password);
				await _localUserService.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, "Unable to reset password because an exception was thrown. {exception}", ex);
			}

			if (success)
			{
				ViewData["Message"] = "Your password was successfully changed.";
			}
			else
			{
				ViewData["Message"] = "Your password could not be changed, please contact your administrator or request a new reset link.";
			}

			
			return View("ResetPasswordResult");
		}
	}
}
