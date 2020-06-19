using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Payroll.Domain
{
	public class ApplicationUserProfile
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Subject { get; set; }

		[Required]
		[MaxLength(250)]
		public string AccessLevel { get; set; }
	}
}
