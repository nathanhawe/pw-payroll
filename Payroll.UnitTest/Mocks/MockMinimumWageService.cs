using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
    public class MockMinimumWageService : IMinimumWageService
    {
        public Dictionary<DateTime, decimal> MinimumWages { get; set; } = new Dictionary<DateTime, decimal>();

        public void AddWage(MinimumWage minimumWage)
        {
            throw new NotImplementedException();
        }

        public MinimumWage DeleteWage(int id)
        {
            throw new NotImplementedException();
        }

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

		public int GetTotalMininumWageCount()
		{
			throw new NotImplementedException();
		}

		public MinimumWage GetWage(int id)
        {
            throw new NotImplementedException();
        }

        public List<MinimumWage> GetWages()
        {
            throw new NotImplementedException();
        }

		public List<MinimumWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			throw new NotImplementedException();
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

        public MinimumWage UpdateWage(int id, MinimumWage minimumWage)
        {
            throw new NotImplementedException();
        }
    }
}
