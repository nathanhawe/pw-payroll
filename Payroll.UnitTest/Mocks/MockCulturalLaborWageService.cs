using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Mocks
{
	/// <summary>
	/// Simple mock of the ICulturalLaborWageService interface that returns a single value for all dates.
	/// </summary>
	public class MockCulturalLaborWageService : Service.Interface.ICulturalLaborWageService
	{
		private decimal _defaultWage;
		public MockCulturalLaborWageService(decimal defaultWage)
		{
			_defaultWage = defaultWage;
		}

		public void AddWage(CulturalLaborWage culturalLaborWage)
		{
			throw new NotImplementedException();
		}

		public CulturalLaborWage DeleteWage(int id)
		{
			throw new NotImplementedException();
		}

		public int GetTotalCulturalLaborWageCount()
		{
			throw new NotImplementedException();
		}

		public CulturalLaborWage GetWage(int id)
		{
			throw new NotImplementedException();
		}

		public decimal GetWage(DateTime shiftDate)
		{
			return _defaultWage;
		}

		public List<CulturalLaborWage> GetWages(int offset, int limit, bool orderByDescending)
		{
			throw new NotImplementedException();
		}

		public CulturalLaborWage UpdateWage(int id, CulturalLaborWage culturalLaborWage)
		{
			throw new NotImplementedException();
		}
	}
}
