﻿namespace Payroll.Infrastructure.Email
{
	public interface IEmailService
	{
		void SendEmail(string recipient, string subject, string body);
	}
}
