using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.ErrorReporting
{
	public class EmailErrorReportingService : IErrorReportingService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IConfiguration _config;

		public EmailErrorReportingService(
			IServiceScopeFactory serviceScopeFactory, 
			IConfiguration config)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			_config = config ?? throw new ArgumentNullException(nameof(config));
		}
		public void ReportError(string source, string message)
		{
			var recipients = _config.GetValue<string>("Email:ErrorToAddress");
			if (!string.IsNullOrWhiteSpace(recipients))
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var emailService = scope.ServiceProvider.GetRequiredService<Payroll.Infrastructure.Email.IEmailService>();
				emailService.SendEmail(recipients, $"{System.Environment.MachineName}: Error from '{source}'", message);
			}
		}
	}
}
