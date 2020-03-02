﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain
{
    public class RanchSummary : Record
    {
        public int BatchId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime WeekEndDate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal TotalGross { get; set; }
        public decimal CulturalHours { get; set; }
        public int LastCrew { get; set; }
    }
}
