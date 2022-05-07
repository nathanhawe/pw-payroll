using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service.Interface
{
	public interface IPlantHourlyRateSelector
	{
		public decimal GetHourlyRate(string payType, int laborCode, decimal employeeHourlyRate, decimal hourlyRateOverride, bool isH2A, Domain.Constants.Plant plant, DateTime shiftDate, decimal payLineHourlyRate, string positionTitle);
	}
}
