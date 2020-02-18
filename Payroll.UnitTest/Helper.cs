using Payroll.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.UnitTest
{
    public static class Helper
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
            bool fiveEight = false)
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
                FiveEight = fiveEight
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
            decimal gross = 0)
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
                Gross = gross
            };
        }

        public static Batch MockBatch(
            int? id = null,
            DateTime? dateCreated = null,
            DateTime? dateModified = null,
            bool? isDeleted = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string state = null,
            bool? isComplete = null,
            string owner = null)
        {
            id ??= 0;
            dateCreated ??= DateTime.Now;
            dateModified ??= DateTime.Now;
            isDeleted ??= false;
            startDate ??= DateTime.Now;
            //endDate ??= DateTime.Now;
            //state ??= "Pending";
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
                State = state,
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
    }
}
