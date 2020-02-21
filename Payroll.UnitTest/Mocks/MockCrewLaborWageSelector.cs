using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
    /// <summary>
    /// Simple mock of the ICrewLaborWageSelector interface that returns a single value for all dates.
    /// </summary>
    public class MockCrewLaborWageSelector : Payroll.Service.Interface.ICrewLaborWageSelector
    {
        private decimal _defaultWage;

        public MockCrewLaborWageSelector(decimal defaultWage)
        {
            _defaultWage = defaultWage;
        }

        public decimal GetCrewLaborWage(DateTime shiftDate)
        {
            return _defaultWage;
        }
    }
}
