using Payroll.ModelValidators;
using System;
using System.ComponentModel.DataAnnotations;

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
