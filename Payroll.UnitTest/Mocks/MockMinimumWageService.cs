using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
    public class MockMinimumWageService : IMinimumWageService
    {
        public Dictionary<DateTime, decimal> MinimumWages { get; set; } = new Dictionary<DateTime, decimal>();
        
        public decimal GetMinimumWageOnDate(DateTime date)
        {
            if (MinimumWages.ContainsKey(date))
            {
                return MinimumWages[date];
            }
            else
            {
                return 0M;
            }
        }

        public void Test_AddMinimumWage(DateTime date, decimal wage)
        {
            if (MinimumWages.ContainsKey(date))
            {
                MinimumWages[date] = wage;
            }
            else
            {
                MinimumWages.Add(date, wage);
            }
        }

        public void Test_RemoveMinimumWage(DateTime date)
        {
            if (MinimumWages.ContainsKey(date)) MinimumWages.Remove(date);
        }
    }
}
