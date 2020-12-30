using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.Models
{
	public class ApplicationUserProfileViewModel
	{
		[Required]
		public string AccessLevel { get; set; }

		[Required]
		[MaxLength(250)]
		public string Name { get; set; }
		
		[Required]
		[MaxLength(250)]
		[EmailAddress]
		public string Email { get; set; }
	}
}
