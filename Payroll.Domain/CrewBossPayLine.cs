using System;

namespace Payroll.Domain
{
    public class CrewBossPayLine : Record
    {
        public int BatchId { get; set; }
        public int LayoffId { get; set; }
        public int QuickBaseRecordId { get; set; }
        public DateTime WeekEndDate { get; set; }
        public DateTime ShiftDate { get; set; }
        public int Crew { get; set; }
        public string EmployeeId { get; set; }
        public string PayMethod { get; set; }
        public int WorkerCount { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal Gross { get; set; }
    }
}
