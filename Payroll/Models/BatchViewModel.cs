using Payroll.ModelValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.Models
{
	public class BatchViewModel
	{
		[Required]
		[Day(DayOfWeek.Sunday)]
		public DateTime? WeekEndDate { get; set; }
		
		public int? LayoffId { get; set; }
		
		[Required]
		[Company]
		public string Company { get; set; }
		
	}
}
