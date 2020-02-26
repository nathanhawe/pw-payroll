using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
    public interface IMinimumWageService
    {
        public decimal GetMinimumWageOnDate(DateTime date);        
    }
}
