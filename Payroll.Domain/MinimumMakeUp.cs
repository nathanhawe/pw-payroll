using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
    public class MinimumMakeUp
    {
        public string EmployeeId { get; set; }
        public int Crew { get; set; }
        public DateTime WeekEndDate { get; set; }
        public DateTime ShiftDate { get; set; }
        public decimal Gross { get; set; }
    }
}
