using Payroll.Data;
using Payroll.Domain;
using Payroll.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
    /// <summary>
    /// Exposes database interactions with the PaidSickLeave table and performs 
    /// PSL calculations.
    /// </summary>
    public class PaidSickLeaveService
    {
        private readonly PayrollContext _context;

        /// <summary>
        /// Type of action performed by UpdateOrInsert method.
        /// </summary>
        private enum UpdateOrInsertType
        {
            All,
            Tracking,
            Usage,
            NinetyDay
        }

        public PaidSickLeaveService(PayrollContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Updates PSL records with new Gross and Hour values for the provided batch and company.
        /// This method creates a new PSL record if a matching one does not already exist.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="company"></param>
        public void UpdateTracking(int batchId, string company)
        {
            if(company == Company.Ranches)
            {
                UpdateRanchTracking(batchId);
            }
            else if(company == Company.Plants)
            {
                UpdatePlantTracking(batchId);
            }
        }
        
        /// <summary>
        /// Updates PSL records with new HoursUsed based on the provided batch and company.
        /// This method creates a new PSL record if a matching one does not already exist.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="company"></param>
        public void UpdateUsage(int batchId, string company)
        {
            if(company == Company.Ranches)
            {
                UpdateRanchUsage(batchId);
            }
            else if(company == Company.Plants)
            {
                UpdatePlantUsage(batchId);
            }
        }

        /// <summary>
        /// Updates PSL records within the inclusive range of <c>startDate</c> to <c>endDate</c> with
        /// new NinetyDayHours and NinetyDayGross values. This method creates a new PSL record if a 
        /// matching one does not already exist.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="company"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void CalculateNinetyDay(int batchId, string company, DateTime startDate, DateTime endDate)
        {
            var paidSickLeaves = new List<PaidSickLeave>();
            List<PaidSickLeave> tempPsl;

            // Retrieve all PSL lines
            // For each PSL within startDate and endDate, figure out 90 day hour and gross totals
            for (DateTime calculatingDate = startDate; calculatingDate <= endDate; calculatingDate = calculatingDate.AddDays(1))
            {
                // Retrieve all lines from 1 to 91 days in the past
                // Group by EmployeeID, sum HoursWorked and TotalGross
                tempPsl = _context.PaidSickLeaves
                    .Where(x =>
                        x.BatchId == batchId
                        && x.Company == company
                        && x.ShiftDate >= calculatingDate.AddDays(-91)
                        && x.ShiftDate < calculatingDate
                        && !x.IsDeleted)
                    .GroupBy(g => new { g.EmployeeId }, (key, group) => new PaidSickLeave
                    {
                        EmployeeId = key.EmployeeId,
                        ShiftDate = calculatingDate,
                        Company = company,
                        NinetyDayHours = group.Sum(x => x.Hours),
                        NinetyDayGross = group.Sum(x => x.Gross)
                    })
                    .ToList();
                paidSickLeaves.AddRange(tempPsl);
            }

            UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.NinetyDay);
        }

        /// <summary>
        /// Private helper method to update existing PSL records or insert new ones.
        /// </summary>
        /// <param name="paidSickLeaves"></param>
        /// <param name="type"></param>
        private void UpdateOrInsert(List<PaidSickLeave> paidSickLeaves, UpdateOrInsertType type)
        {
            var toUpdate = new List<PaidSickLeave>();
            var toInsert = new List<PaidSickLeave>();
            PaidSickLeave existingSickLeave;

            foreach (var psl in paidSickLeaves)
            {
                existingSickLeave = _context.PaidSickLeaves
                    .Where(x =>
                        x.EmployeeId == psl.EmployeeId
                        && x.ShiftDate == psl.ShiftDate
                        && x.Company == psl.Company
                        && !x.IsDeleted)
                    .FirstOrDefault();

                if (existingSickLeave != null)
                {
                    if(type == UpdateOrInsertType.Tracking || type == UpdateOrInsertType.All)
                    {
                        existingSickLeave.Hours = psl.Hours;
                        existingSickLeave.Gross = psl.Gross;
                    }
                    if(type == UpdateOrInsertType.Usage || type == UpdateOrInsertType.All)
                    {
                        existingSickLeave.HoursUsed = psl.HoursUsed;
                    }
                    if(type == UpdateOrInsertType.NinetyDay || type == UpdateOrInsertType.All)
                    {
                        existingSickLeave.NinetyDayHours = psl.NinetyDayHours;
                        existingSickLeave.NinetyDayGross = psl.NinetyDayGross;
                    }
                }
                else
                {
                    _context.Add(psl);
                }
            }

            // Commit all changes made above
            _context.SaveChanges();
        }

        /// <summary>
        /// Private helper method to update ranch accrual tracking.
        /// </summary>
        /// <param name="batchId"></param>
        private void UpdateRanchTracking(int batchId)
        {
            // Retrieve ranch paylines with Regular, Pieces, Hourly Plus Pieces, and Crew Boss pay types
            // Group all pay lines by employee and date, sum hours, sum gross.
            var paidSickLeaves = _context.RanchPayLines
                    .Where(x => x.BatchId == batchId && 
                        (
                            x.PayType == PayType.Regular
                            || x.PayType == PayType.Pieces
                            || x.PayType == PayType.HourlyPlusPieces
                            || x.PayType == PayType.CBCommission
                            || x.PayType == PayType.CBDaily
                            || x.PayType == PayType.CBHourlyTrees
                            || x.PayType == PayType.CBHourlyVines
                            || x.PayType == PayType.CBPerWorker
                            || x.PayType == PayType.CBSouthDaily
                            || x.PayType == PayType.CBSouthHourly)
                        && !x.IsDeleted)
                    .GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
                    {
                        BatchId = batchId,
                        EmployeeId = key.EmployeeId,
                        ShiftDate = key.ShiftDate,
                        Hours = group.Sum(x => x.HoursWorked),
                        Gross = group.Sum(x => x.TotalGross),
                        Company = Company.Ranches
                    })
                    .ToList();

            UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Tracking);
        }

        /// <summary>
        /// Private helper method to update plant accrual tracking.
        /// </summary>
        /// <param name="batchId"></param>
        private void UpdatePlantTracking(int batchId)
        {
            // Retrieve plant paylines with Regular and Pieces pay types only.
            // Group all pay lines by employee and date, sum hours, sum gross.
            var paidSickLeaves = _context.PlantPayLines
                    .Where(x => x.BatchId == batchId &&
                        (
                            x.PayType == PayType.Regular
                            || x.PayType == PayType.Pieces)
                        && !x.IsDeleted)
                    .GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
                    {
                        BatchId = batchId,
                        EmployeeId = key.EmployeeId,
                        ShiftDate = key.ShiftDate,
                        Hours = group.Sum(x => x.HoursWorked),
                        Gross = group.Sum(x => x.TotalGross),
                        Company = Company.Plants
                    })
                    .ToList();

            UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Tracking);
        }

        /// <summary>
        /// Private helper method to update ranch PSL usage.
        /// </summary>
        /// <param name="batchId"></param>
        private void UpdateRanchUsage(int batchId)
        {
            // Retrieve ranch paylines with Sick Leave as the pay type
            // Group by employee and shift date, sum hours worked.
            var paidSickLeaves = _context.RanchPayLines
                .Where(x => 
                    x.BatchId == batchId 
                    && x.PayType == PayType.SickLeave
                    && !x.IsDeleted)
                .GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
                {
                    BatchId = batchId,
                    EmployeeId = key.EmployeeId,
                    ShiftDate = key.ShiftDate,
                    HoursUsed = group.Sum(x => x.HoursWorked),
                    Company = Company.Ranches
                })
                .ToList();

            UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Usage);
        }

        /// <summary>
        /// Private helper method to update plant PSL usage.
        /// </summary>
        /// <param name="batchId"></param>
        private void UpdatePlantUsage(int batchId)
        {
            // Retrieve plant paylines with Sick Leave as the pay type
            // Group by employee and shift date, sum hours worked.
            var paidSickLeaves = _context.PlantPayLines
                .Where(x => 
                    x.BatchId == batchId 
                    && x.PayType == PayType.SickLeave
                    && !x.IsDeleted)
                .GroupBy(g => new { g.EmployeeId, g.ShiftDate }, (key, group) => new PaidSickLeave
                {
                    BatchId = batchId,
                    EmployeeId = key.EmployeeId,
                    ShiftDate = key.ShiftDate,
                    HoursUsed = group.Sum(x => x.HoursWorked),
                    Company = Company.Plants
                })
                .ToList();

            UpdateOrInsert(paidSickLeaves, UpdateOrInsertType.Usage);
        }
               
    }
}
