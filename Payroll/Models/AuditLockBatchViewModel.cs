using System.ComponentModel.DataAnnotations;

namespace Payroll.Models
{
	public class AuditLockBatchViewModel : BatchViewModel
	{
		[Required]
		public bool Lock { get; set; }		
	}
}
