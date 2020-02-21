﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
    public class WeeklyEffectiveRate
    {
        public string EmployeeId { get; set; }
        public DateTime WeekEndDate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal NonProductiveTime { get; set; }
        public decimal TotalGross { get; set; }
        public decimal EffectiveHourlyRate { get; set; }
    }
}
