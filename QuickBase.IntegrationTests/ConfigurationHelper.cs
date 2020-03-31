using Microsoft.Extensions.Configuration;
using Payroll;

namespace QuickBase.IntegrationTests
{
	public static class ConfigurationHelper
	{
		public static IConfigurationRoot GetIConfigurationRoot()
		{
			var builder = new ConfigurationBuilder();
			builder.AddUserSecrets<Startup>();

			return builder.Build();
		}
	}
}
