using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll.Domain
{
	public class CulturalLaborWage : Record
	{
		public DateTime EffectiveDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Wage { get; set; }
	}
}
