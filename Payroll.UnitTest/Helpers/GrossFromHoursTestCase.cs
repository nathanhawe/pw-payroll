using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Helpers
{
    public class GrossFromHoursTestCase
    {
        public int Id { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal ExpectedGrossFromHours { get; set; }
        public bool UseOldHourlyRate { get; set; } = false;
        public decimal OldHourlyRate { get; set; } = 0;
    }
}
