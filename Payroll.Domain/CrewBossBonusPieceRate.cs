using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Domain
{
	public class CrewBossBonusPieceRate : Record
	{
		public int LaborCode { get; set; }
		public DateTime EffectiveDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal PerTreeBonus { get; set; }
	}
}
