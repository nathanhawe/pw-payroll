using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class RanchBonusPieceRate : Record
	{
		public int BatchId { get; set; }
		public int QuickBaseRecordId { get; set; }
		public int BlockId { get; set; }
		public int LaborCode { get; set; }
		public DateTime EffectiveDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal PerHourThreshold { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal PerTreeBonus { get; set; }
		
		public string Designation { get; set; }
	}
}
