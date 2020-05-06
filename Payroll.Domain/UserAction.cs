using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class UserAction : Record
	{
		public string User { get; set; }
		public string Action { get; set; }
	}
}
