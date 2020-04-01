using System;
using System.Collections.Generic;
using System.Text;

namespace QuickBase.Api.Exceptions
{
	public class QuickBaseException : Exception
	{
		public QuickBaseException(string message)
			:base(message) { }

		public QuickBaseException(string errorCode, string errorText)
			:base($"Error Code '{errorCode}': '{errorText}'") { }
	}
}
