using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest.Helpers
{
	public static class EntityMocker
	{
		public static RanchPayLine MockRanchPayLine(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			int batchId = 0,
			int layoffId = 0,
			int quickBaseRecordId = 0,
			DateTime? weekEndDate = null,
			DateTime? shiftDate = null,
			int crew = 0,
			string employeeId = "TEST",
			int laborCode = 0,
			int blockId = 0,
			decimal hoursWorked = 0,
			string payType = "1-Regular",
			decimal pieces = 0,
			decimal pieceRate = 0,
			decimal hourlyRate = 0,
			decimal otDtWotRate = 0,
			decimal otDtWotHours = 0,
			decimal grossFromHours = 0,
			decimal grossFromPieces = 0,
			decimal otherGross = 0,
			bool alternativeWorkWeek = false,
			bool fiveEight = false,
			decimal totalGross = 0,
			int lastCrew = 0,
			decimal hourlyRateOverride = 0)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			weekEndDate ??= new DateTime(2020, 1, 5);
			shiftDate ??= new DateTime(2020, 1, 5);

			return new RanchPayLine
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				BatchId = batchId,
				LayoffId = layoffId,
				QuickBaseRecordId = quickBaseRecordId,
				WeekEndDate = weekEndDate.Value,
				ShiftDate = shiftDate.Value,
				Crew = crew,
				LastCrew = lastCrew,
				EmployeeId = employeeId,
				LaborCode = laborCode,
				BlockId = blockId,
				HoursWorked = hoursWorked,
				PayType = payType,
				Pieces = pieces,
				PieceRate = pieceRate,
				HourlyRate = hourlyRate,
				OtDtWotRate = otDtWotRate,
				OtDtWotHours = otDtWotHours,
				GrossFromHours = grossFromHours,
				GrossFromPieces = grossFromPieces,
				OtherGross = otherGross,
				TotalGross = totalGross,
				AlternativeWorkWeek = alternativeWorkWeek,
				FiveEight = fiveEight,
				HourlyRateOverride = hourlyRateOverride
			};
		}

		public static CrewBossPayLine MockCrewBossPayLine(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			int batchId = 0,
			int layoffId = 0,
			int quickBaseRecordId = 0,
			DateTime? weekEndDate = null,
			DateTime? shiftDate = null,
			int crew = 0,
			string employeeId = "TEST",
			string payMethod = "",
			int workerCount = 0,
			decimal hoursWorked = 0,
			decimal hourlyRate = 0,
			decimal gross = 0,
			bool fiveEight = false)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			weekEndDate ??= new DateTime(2020, 1, 5);
			shiftDate ??= new DateTime(2020, 1, 5);

			return new CrewBossPayLine
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				BatchId = batchId,
				LayoffId = layoffId,
				QuickBaseRecordId = quickBaseRecordId,
				WeekEndDate = weekEndDate.Value,
				ShiftDate = shiftDate.Value,
				Crew = crew,
				EmployeeId = employeeId,
				PayMethod = payMethod,
				WorkerCount = workerCount,
				HoursWorked = hoursWorked,
				HourlyRate = hourlyRate,
				Gross = gross,
				FiveEight = fiveEight
			};
		}

		public static Batch MockBatch(
			int? id = null,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool? isDeleted = null,
			DateTime? startDate = null,
			DateTime? endDate = null,
			bool? isComplete = null,
			string owner = null)
		{
			id ??= 0;
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			isDeleted ??= false;
			startDate ??= DateTime.Now;
			//endDate ??= DateTime.Now;
			isComplete ??= false;
			owner ??= "Test";

			return new Batch
			{
				Id = id.Value,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted.Value,
				StartDate = startDate,
				EndDate = endDate,
				IsComplete = isComplete.Value,
				Owner = owner
			};

		}

		public static PaidSickLeave MockPaidSickLeave(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			int batchId = 0,
			string EmployeeId = "TEST",
			DateTime? shiftDate = null,
			string company = "",
			decimal hours = 0,
			decimal gross = 0,
			decimal ninetyDayHours = 0,
			decimal ninetyDayGross = 0,
			decimal hoursUsed = 0
			)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			shiftDate ??= new DateTime(2020, 1, 5);

			return new PaidSickLeave
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				BatchId = batchId,
				EmployeeId = EmployeeId,
				ShiftDate = shiftDate.Value,
				Company = company,
				Hours = hours,
				Gross = gross,
				NinetyDayHours = ninetyDayHours,
				NinetyDayGross = ninetyDayGross,
				HoursUsed = hoursUsed
			};
		}

		public static CrewLaborWage MockCrewLaborWage(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			DateTime? effectiveDate = null,
			decimal wage = 0)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			effectiveDate ??= new DateTime(2000, 1, 1);

			return new CrewLaborWage
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				EffectiveDate = effectiveDate.Value,
				Wage = wage
			};
		}

		public static MinimumWage MockMinimumWage(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			decimal wage = 0,
			DateTime? effectiveDate = null)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			effectiveDate ??= DateTime.Now;

			return new MinimumWage
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				Wage = wage,
				EffectiveDate = effectiveDate.Value
			};
		}

		public static RanchAdjustmentLine MockRanchAdjustmentLine(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			int batchId = 0,
			int layoffId = 0,
			int quickBaseRecordId = 0,
			DateTime? weekEndDate = null,
			DateTime? shiftDate = null,
			int crew = 0,
			string employeeId = "TEST",
			int laborCode = 0,
			int blockId = 0,
			decimal hoursWorked = 0,
			string payType = "1-Regular",
			decimal pieces = 0,
			decimal pieceRate = 0,
			decimal hourlyRate = 0,
			decimal otDtWotRate = 0,
			decimal otDtWotHours = 0,
			decimal grossFromHours = 0,
			decimal grossFromPieces = 0,
			decimal otherGross = 0,
			decimal totalGross = 0,
			bool alternativeWorkWeek = false,
			bool fiveEight = false,
			DateTime? weekEndOfAdjustmentPaid = null,
			bool isOriginal = false,
			decimal oldHourlyRate = 0,
			bool useOldHourlyRate = false)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			weekEndDate ??= new DateTime(2020, 1, 5);
			shiftDate ??= new DateTime(2020, 1, 5);
			weekEndOfAdjustmentPaid ??= new DateTime(2020, 1, 12);

			return new RanchAdjustmentLine
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				BatchId = batchId,
				LayoffId = layoffId,
				QuickBaseRecordId = quickBaseRecordId,
				WeekEndDate = weekEndDate.Value,
				ShiftDate = shiftDate.Value,
				Crew = crew,
				EmployeeId = employeeId,
				LaborCode = laborCode,
				BlockId = blockId,
				HoursWorked = hoursWorked,
				PayType = payType,
				Pieces = pieces,
				PieceRate = pieceRate,
				HourlyRate = hourlyRate,
				OtDtWotRate = otDtWotRate,
				OtDtWotHours = otDtWotHours,
				GrossFromHours = grossFromHours,
				GrossFromPieces = grossFromPieces,
				OtherGross = otherGross,
				AlternativeWorkWeek = alternativeWorkWeek,
				FiveEight = fiveEight,
				WeekEndOfAdjustmentPaid = weekEndOfAdjustmentPaid.Value,
				IsOriginal = isOriginal,
				OldHourlyRate = oldHourlyRate,
				UseOldHourlyRate = useOldHourlyRate,
				TotalGross = totalGross
			};
		}

		public static PlantPayLine MockPlantPayLine(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			int batchId = 0,
			int layoffId = 0,
			int quickBaseRecordId = 0,
			DateTime? weekEndDate = null,
			DateTime? shiftDate = null,
			int plant = 0,
			string employeeId = "TEST",
			int laborCode = 0,
			decimal hoursWorked = 0,
			string payType = "1-Regular",
			decimal pieces = 0,
			decimal hourlyRate = 0,
			decimal otDtWotRate = 0,
			decimal otDtWotHours = 0,
			decimal grossFromHours = 0,
			decimal grossFromPieces = 0,
			decimal otherGross = 0,
			decimal grossFromIncentive = 0,
			bool alternativeWorkWeek = false,
			decimal totalGross = 0,
			decimal increasedRate = 0,
			decimal nonPrimaRate = 0,
			decimal primaRate = 0,
			bool isIncentiveDisqualified = false,
			bool hasNonPrimaViolation = false,
			bool useIncreasedRate = false,
			decimal nonDiscretionaryBonusRate = 0,
			bool useCrewLaborRateForMinimumAssurance = false
			)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			weekEndDate ??= new DateTime(2020, 1, 5);
			shiftDate ??= new DateTime(2020, 1, 5);

			return new PlantPayLine
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				BatchId = batchId,
				LayoffId = layoffId,
				QuickBaseRecordId = quickBaseRecordId,
				WeekEndDate = weekEndDate.Value,
				ShiftDate = shiftDate.Value,
				Plant = plant,
				EmployeeId = employeeId,
				LaborCode = laborCode,
				HoursWorked = hoursWorked,
				PayType = payType,
				Pieces = pieces,
				HourlyRate = hourlyRate,
				OtDtWotRate = otDtWotRate,
				OtDtWotHours = otDtWotHours,
				GrossFromHours = grossFromHours,
				GrossFromPieces = grossFromPieces,
				OtherGross = otherGross,
				GrossFromIncentive = grossFromIncentive,
				TotalGross = totalGross,
				AlternativeWorkWeek = alternativeWorkWeek,
				IncreasedRate = increasedRate,
				PrimaRate = primaRate,
				NonPrimaRate = nonPrimaRate,
				IsIncentiveDisqualified = isIncentiveDisqualified,
				HasNonPrimaViolation = hasNonPrimaViolation,
				UseIncreasedRate = useIncreasedRate,
				NonDiscretionaryBonusRate = nonDiscretionaryBonusRate,
				UseCrewLaborRateForMinimumAssurance = useCrewLaborRateForMinimumAssurance
			};
		}

		public static PlantAdjustmentLine MockPlantAdjustmentLine(
			int id = 0,
			DateTime? dateCreated = null,
			DateTime? dateModified = null,
			bool isDeleted = false,
			int batchId = 0,
			int layoffId = 0,
			int quickBaseRecordId = 0,
			DateTime? weekEndDate = null,
			DateTime? shiftDate = null,
			int plant = 0,
			string employeeId = "TEST",
			int laborCode = 0,
			decimal hoursWorked = 0,
			string payType = "1-Regular",
			decimal pieces = 0,
			decimal pieceRate = 0,
			decimal hourlyRate = 0,
			decimal otDtWotRate = 0,
			decimal otDtWotHours = 0,
			decimal grossFromHours = 0,
			decimal grossFromPieces = 0,
			decimal otherGross = 0,
			decimal grossFromIncentive = 0,
			bool alternativeWorkWeek = false,
			decimal totalGross = 0,
			DateTime? weekEndOfAdjustmentPaid = null,
			bool isOriginal = false,
			decimal oldHourlyRate = 0,
			bool useOldHourlyRate = false,
			bool useCrewLaborRateForMinimumAssurance = false
			)
		{
			dateCreated ??= DateTime.Now;
			dateModified ??= DateTime.Now;
			weekEndDate ??= new DateTime(2020, 1, 5);
			shiftDate ??= new DateTime(2020, 1, 5);
			weekEndOfAdjustmentPaid ??= new DateTime(2020, 1, 12);

			return new PlantAdjustmentLine
			{
				Id = id,
				DateCreated = dateCreated.Value,
				DateModified = dateModified.Value,
				IsDeleted = isDeleted,
				BatchId = batchId,
				LayoffId = layoffId,
				QuickBaseRecordId = quickBaseRecordId,
				WeekEndDate = weekEndDate.Value,
				ShiftDate = shiftDate.Value,
				Plant = plant,
				EmployeeId = employeeId,
				LaborCode = laborCode,
				HoursWorked = hoursWorked,
				PayType = payType,
				Pieces = pieces,
				PieceRate = pieceRate,
				HourlyRate = hourlyRate,
				OtDtWotRate = otDtWotRate,
				OtDtWotHours = otDtWotHours,
				GrossFromHours = grossFromHours,
				GrossFromPieces = grossFromPieces,
				OtherGross = otherGross,
				GrossFromIncentive = grossFromIncentive,
				TotalGross = totalGross,
				AlternativeWorkWeek = alternativeWorkWeek,
				WeekEndOfAdjustmentPaid = weekEndOfAdjustmentPaid.Value,
				IsOriginal = isOriginal,
				OldHourlyRate = oldHourlyRate,
				UseOldHourlyRate = useOldHourlyRate,
				UseCrewLaborRateForMinimumAssurance = useCrewLaborRateForMinimumAssurance
			};
		}
	}
}
