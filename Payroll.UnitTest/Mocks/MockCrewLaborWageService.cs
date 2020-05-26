using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
    /// <summary>
    /// Simple mock of the ICrewLaborWageSelector interface that returns a single value for all dates.
    /// </summary>
    public class MockCrewLaborWageService : Payroll.Service.Interface.ICrewLaborWageService
    {
        private decimal _defaultWage;

        public MockCrewLaborWageService(decimal defaultWage)
        {
            _defaultWage = defaultWage;
        }

        public void AddWage(CrewLaborWage crewLaborWage)
        {
            throw new NotImplementedException();
        }

        public CrewLaborWage DeleteWage(int id)
        {
            throw new NotImplementedException();
        }

        public decimal GetWage(DateTime shiftDate)
        {
            return _defaultWage;
        }

        public CrewLaborWage GetWage(int id)
        {
            throw new NotImplementedException();
        }

        public List<CrewLaborWage> GetWages()
        {
            throw new NotImplementedException();
        }

        public CrewLaborWage UpdateWage(int id, CrewLaborWage crewLaborWage)
        {
            throw new NotImplementedException();
        }
    }
}
