using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payroll.Data.QuickBase;
using Payroll.Domain;
using Payroll.Domain.Constants;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	/// <summary>
	/// Manages the audit lock workflow
	/// </summary>
	public class AuditLockService : IAuditLockService
	{
		private readonly ILogger<AuditLockService> _logger;

		// Database context
		private readonly Data.PayrollContext _context;

		// Repositories
		private readonly IRanchPayrollRepo _ranchPayrollRepo;
		private readonly IPlantPayrollRepo _plantPayrollRepo;

		public AuditLockService(
			ILogger<AuditLockService> logger,
			Data.PayrollContext payrollContext,
			IRanchPayrollRepo ranchPayrollRepo,
			IPlantPayrollRepo plantPayrollRepo)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_context = payrollContext ?? throw new ArgumentNullException(nameof(payrollContext));
			_ranchPayrollRepo = ranchPayrollRepo ?? throw new ArgumentNullException(nameof(ranchPayrollRepo));
			_plantPayrollRepo = plantPayrollRepo ?? throw new ArgumentNullException(nameof(plantPayrollRepo));
		}

		public void ProcessAuditLockBatch(int auditLockBatchId)
		{
			try
			{
				_logger.Log(LogLevel.Information, "Starting audit lock for audit lock batch {auditLockBatchId}", auditLockBatchId);
				var auditLockBatch = _context.AuditLockBatches.Where(x => x.Id == auditLockBatchId).FirstOrDefault();
				if (auditLockBatch == null) throw new Exception($"The provided audit lock batch ID of '{auditLockBatchId}' was not found in the database.");
				SetAuditLockBatchStatus(auditLockBatchId, BatchProcessingStatus.Starting);

				switch (auditLockBatch.Company)
				{
					case Company.Plants: PerformPlantsAuditLock(auditLockBatch); break;
					case Company.Ranches: PerformRanchAuditLock(auditLockBatch); break;
					default: throw new Exception($"Unknown company value '{auditLockBatch.Company}'.");
				}
				_logger.Log(LogLevel.Information, "Completed audit locking for audit lock batch {auditLockBatchId}", auditLockBatchId);
				SetAuditLockBatchStatus(auditLockBatchId, BatchProcessingStatus.Success);
			}
			catch(Exception ex)
			{
				SetAuditLockBatchStatus(auditLockBatchId, BatchProcessingStatus.Failed);
				throw ex;
			}
		}
				

		private void PerformPlantsAuditLock(AuditLockBatch auditLockBatch)
		{
			/* Download Quick Base data */
			_logger.Log(LogLevel.Information, "Downloading plant payroll data from Quick Base for {auditLockBatchId}", auditLockBatch.Id);
			SetAuditLockBatchStatus(auditLockBatch.Id, BatchProcessingStatus.Downloading);
			
			var plantPayLines = _plantPayrollRepo.GetForSummaries(auditLockBatch.WeekEndDate, auditLockBatch.LayoffId ?? 0).ToList();

			/* Update records to Quick Base */
			SetAuditLockBatchStatus(auditLockBatch.Id, BatchProcessingStatus.Uploading);
			_logger.Log(LogLevel.Information, "Updating Quick Base for audit lock batch {auditLockBatchId}", auditLockBatch.Id);

			if (auditLockBatch.Lock)
			{
				_plantPayrollRepo.Lock(plantPayLines);
			}
			else
			{
				_plantPayrollRepo.Unlock(plantPayLines);
			}
		}

		private void PerformRanchAuditLock(AuditLockBatch auditLockBatch)
		{
			/* Download Quick Base data */
			_logger.Log(LogLevel.Information, "Downloading ranch payroll data from Quick Base for {auditLockBatchId}", auditLockBatch.Id);
			SetAuditLockBatchStatus(auditLockBatch.Id, BatchProcessingStatus.Downloading);

			var ranchPayLines = _ranchPayrollRepo.GetForSummaries(auditLockBatch.WeekEndDate, auditLockBatch.LayoffId ?? 0).ToList();

			/* Update records to Quick Base */
			SetAuditLockBatchStatus(auditLockBatch.Id, BatchProcessingStatus.Uploading);
			_logger.Log(LogLevel.Information, "Updating Quick Base for audit lock batch {auditLockBatchId}", auditLockBatch.Id);

			if (auditLockBatch.Lock)
			{
				_ranchPayrollRepo.Lock(ranchPayLines);
			}
			else
			{
				_ranchPayrollRepo.Unlock(ranchPayLines);
			}
		}

		private void SetAuditLockBatchStatus(int id, BatchProcessingStatus status)
		{
			var auditLockBatch = _context.AuditLockBatches.Find(id);
			if(auditLockBatch != null)
			{
				if(status == BatchProcessingStatus.Failed)
				{
					auditLockBatch.StatusMessage = $"Audit Lock Batch ID #{auditLockBatch.Id} failed while at step '{auditLockBatch.ProcessingStatus}'.";
					auditLockBatch.IsComplete = true;
					auditLockBatch.EndDate = DateTime.UtcNow;
				}
				else if(status == BatchProcessingStatus.Success)
				{
					auditLockBatch.IsComplete = true;
					auditLockBatch.EndDate = DateTime.UtcNow;
				}
				else if(status == BatchProcessingStatus.Starting)
				{
					auditLockBatch.StartDate = DateTime.UtcNow;
				}
				
				auditLockBatch.ProcessingStatus = status;
				_context.SaveChanges();
			}
		}
	}
}
