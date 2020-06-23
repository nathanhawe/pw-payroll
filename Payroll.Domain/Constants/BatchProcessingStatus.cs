using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Domain.Constants
{
	public enum BatchProcessingStatus
	{
		NotStarted = 0,
		Starting,
		Downloading,
		CrewBossCalculations,
		GrossCalculations,
		PaidSickLeaveCalculations,
		AdditionalCalculations,
		Adjustments,
		Summaries,
		Uploading,
		Success,
		Failed
	}
}
