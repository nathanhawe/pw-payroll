using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Payroll.Infrastructure.Middleware
{
	public class ApiExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ApiExceptionMiddleware> _logger;
		private readonly ApiExceptionsOptions _options;

		public ApiExceptionMiddleware(ApiExceptionsOptions options, RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
			_options = options;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch(Exception ex)
			{
				await HandleExceptionAsyc(context, ex, _options);
			}
		}

		private Task HandleExceptionAsyc(HttpContext context, Exception ex, ApiExceptionsOptions options)
		{
			var error = new ApiError
			{
				Id = Guid.NewGuid().ToString(),
				Status = (short)HttpStatusCode.InternalServerError,
				Title = "An error occured."
			};

			options.AddResponseDetails?.Invoke(context, ex, error);

			var innerMessage = GetInnermostExceptionMessage(ex);

			_logger.Log(LogLevel.Error, ex, "Exception Logged - " + innerMessage + " -- {ErrorId}.", error.Id);

			var result = JsonConvert.SerializeObject(error);
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			
			return context.Response.WriteAsync(result);
		}

		private string GetInnermostExceptionMessage(Exception ex)
		{
			if (ex.InnerException != null) return GetInnermostExceptionMessage(ex.InnerException);

			return ex.Message;
		}
	}
}
