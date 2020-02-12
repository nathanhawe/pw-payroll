using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
    public class MinimumWage : Record
    {
        public decimal Wage { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
