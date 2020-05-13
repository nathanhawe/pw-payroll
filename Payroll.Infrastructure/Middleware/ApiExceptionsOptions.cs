using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.Middleware
{
	public class ApiExceptionsOptions
	{
		public Action<HttpContext, Exception, ApiError> AddResponseDetails { get; set; }
	}
}
