﻿using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Payroll.Data.QuickBase
{
	public interface IPlantSummariesRepo
	{
		public IEnumerable<PlantSummary> Get(DateTime weekEndDate);
		public void Save(IEnumerable<PlantSummary> plantSummaries);
	}
}
