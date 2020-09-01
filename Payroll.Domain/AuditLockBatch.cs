using Payroll.Domain.Constants;
using System;

namespace Payroll.Domain
{
	public class AuditLockBatch : Record
	{
		public DateTime WeekEndDate { get; set; }
		public int? LayoffId { get; set; }
		public string Company { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public BatchProcessingStatus ProcessingStatus { get; set; } = BatchProcessingStatus.NotStarted;
		public string StatusMessage { get; set; }
		public bool IsComplete { get; set; }
		public string Owner { get; set; }
		public bool Lock { get; set; }
	}
}
