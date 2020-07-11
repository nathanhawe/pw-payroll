using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.Services
{
	public interface IEmailService
	{
		void SendEmail(string recipient, string subject, string body);
	}
}
