using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.Middleware
{
	public static class ApiExceptionMiddlewareExtensions
	{
		public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder)
		{
			var options = new ApiExceptionsOptions();
			return builder.UseMiddleware<ApiExceptionMiddleware>(options);
		}

		public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder, Action<ApiExceptionsOptions> configureOptions)
		{
			var options = new ApiExceptionsOptions();
			configureOptions(options);

			return builder.UseMiddleware<ApiExceptionMiddleware>(options);
		}
	}
}
