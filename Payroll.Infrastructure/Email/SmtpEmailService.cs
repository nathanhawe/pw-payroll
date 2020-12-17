using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;

namespace Payroll.Infrastructure.Email
{
	public class SmtpEmailService : IEmailService
	{
		private readonly string _serverAddress;
		private readonly int _port;
		private readonly string _mailboxName;
		private readonly string _mailboxAddress;
		private readonly ILogger<SmtpEmailService> _logger;

		
		public SmtpEmailService(
			ILogger<SmtpEmailService> logger,
			string serverAddress, 
			string mailboxName, 
			string mailboxAddress,
			int port)
		{
			_serverAddress = serverAddress ?? throw new ArgumentNullException(nameof(serverAddress));
			_port = port;
			_mailboxName = mailboxName ?? throw new ArgumentNullException(nameof(mailboxName));
			_mailboxAddress = mailboxAddress ?? throw new ArgumentNullException(nameof(mailboxAddress));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		}

		public void SendEmail(string recipient, string subject, string body)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_mailboxName, _mailboxAddress));
			message.To.Add(MailboxAddress.Parse(recipient));
			message.Subject = subject;
			message.Body = new TextPart("plain")
			{
				Text = body
			};
			
			using var client = new SmtpClient();
			try
			{
				client.Connect(_serverAddress, _port);
				client.Send(message);
			}
			catch(Exception ex)
			{
				_logger.Log(LogLevel.Error, "An exception was thrown while attempting to send a message to {Email}. {Exception}", recipient, ex);
			}
			finally
			{
				client.Disconnect(true);
			}
		}
	}
}
