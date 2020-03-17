using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Helpers
{
    public class GrossFromIncentiveTestCase
    {
        public int Id { get; set; }
        public int LaborCode { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal Pieces { get; set; }
        public decimal IncreasedRate { get; set; }
        public decimal PrimaRate { get; set; }
        public decimal NonPrimaRate { get; set; }
        public decimal ExpectedGross { get; set; }
    }
}
