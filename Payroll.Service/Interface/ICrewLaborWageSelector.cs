using System;

namespace Payroll.Service.Interface
{
    public interface ICrewLaborWageSelector
    {
        public decimal GetCrewLaborWage(DateTime shiftDate);        
    }
}
