﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Payroll.Domain
{
	public class PlantPayLine : Record
	{
		public int BatchId { get; set; }
		public int LayoffId { get; set; }
		public int QuickBaseRecordId { get; set; }
		public DateTime WeekEndDate { get; set; }
		public DateTime ShiftDate { get; set; }
		public int Plant { get; set; }
		public string EmployeeId { get; set; }
		public int LaborCode { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HoursWorked { get; set; }
		public string PayType { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Pieces { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OtDtWotRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OtDtWotHours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal GrossFromHours { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal GrossFromPieces { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal GrossFromIncentive { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal OtherGross { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalGross { get; set; }
		public bool AlternativeWorkWeek { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyRateOverride { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal EmployeeHourlyRate { get; set; }
		public bool IsH2A { get; set; }
		public bool IsIncentiveDisqualified { get; set; }
		public bool HasNonPrimaViolation { get; set; }
		public bool UseIncreasedRate { get; set; }

		[Column(TypeName = "decimal(18,16)")]
		public decimal NonPrimaRate { get; set; }

		[Column(TypeName = "decimal(18,16)")]
		public decimal PrimaRate { get; set; }

		[Column(TypeName = "decimal(18,16)")]
		public decimal IncreasedRate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal NonDiscretionaryBonusRate { get; set; }

		public bool UseCrewLaborRateForMinimumAssurance { get; set; }

		public int BoxStyle { get; set; }
		public string BoxStyleDescription { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal H2AHoursOffered { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal SickLeaveRequested { get; set; }

		public int PackerNumber { get; set; }
		public string Packline { get; set; }
		public string PositionTitle { get; set; }

		[NotMapped]
		public bool SpecialAdjustmentApproved { get; set; }
	}
}
