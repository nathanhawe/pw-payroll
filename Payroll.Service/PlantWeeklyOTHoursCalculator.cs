using Payroll.Domain;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Service
{
	public class PlantWeeklyOTHoursCalculator : IWeeklyOTHoursCalculator
	{
		public List<WeeklyOverTimeHours> GetWeeklyOTHours(List<WeeklySummary> weeklySummaries)
		{
			throw new NotImplementedException();
		}
	}
}
