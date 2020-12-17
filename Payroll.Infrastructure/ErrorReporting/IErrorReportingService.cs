using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Infrastructure.ErrorReporting
{
	public interface IErrorReportingService
	{
		void ReportError(string source, string message);
	}
}
