using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
	public class Record
	{
		public int Id { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public bool IsDeleted { get; set; }
	}
}
