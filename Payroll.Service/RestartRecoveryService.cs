using Microsoft.Extensions.Logging;
using Payroll.Data;
using Payroll.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Payroll.Service
{
	public class RestartRecoveryService : IRestartRecoveryService
	{
		private readonly ILogger<RestartRecoveryService> _logger;
		private readonly PayrollContext _context;

		public RestartRecoveryService(
			ILogger<RestartRecoveryService> logger, 
			PayrollContext context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public void HandleRestart()
		{
			
			try
			{
				FailIncompleteAuditLocks();
			}
			catch(Exception ex)
			{
				_logger.Log(LogLevel.Error, "Exception thrown attempting to fail incomplete audit locks {Exception}", ex);
			}

			try
			{
				FailIncompleteBatches();

			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, "Exception thrown attempting to fail incomplete batches {Exception}", ex);
			}

			try
			{
				FailIncompleteSummaries();

			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, "Exception thrown attempting to fail incomplete summaries {Exception}", ex);
			}
		}

		private void FailIncompleteAuditLocks()
		{
			var incompleteAuditLocks = _context.AuditLockBatches.Where(x => !x.IsDeleted && !x.IsComplete).ToList();
			foreach(var auditLock in incompleteAuditLocks)
			{
				auditLock.StatusMessage = $"Failed due to server restart while at step '{auditLock.ProcessingStatus}'.";
				auditLock.ProcessingStatus = Domain.Constants.BatchProcessingStatus.Failed;
				auditLock.IsComplete = true;
			}
			_context.SaveChanges();
			_logger.Log(LogLevel.Information, $"Failed {incompleteAuditLocks.Count()} incomplete audit locks.");
		}

		private void FailIncompleteBatches()
		{
			var incompleteBatches = _context.Batches.Where(x => !x.IsDeleted && !x.IsComplete).ToList();
			foreach (var batch in incompleteBatches)
			{
				batch.StatusMessage = $"Failed due to server restart while at step '{batch.ProcessingStatus}'.";
				batch.ProcessingStatus = Domain.Constants.BatchProcessingStatus.Failed;
				batch.IsComplete = true;
			}
			_context.SaveChanges();
			_logger.Log(LogLevel.Information, $"Failed {incompleteBatches.Count()} incomplete batches.");
		}

		private void FailIncompleteSummaries()
		{
			var incompleteSummaries = _context.SummaryBatches.Where(x => !x.IsDeleted && !x.IsComplete).ToList();
			foreach (var summary in incompleteSummaries)
			{
				summary.StatusMessage = $"Failed due to server restart while at step '{summary.ProcessingStatus}'.";
				summary.ProcessingStatus = Domain.Constants.BatchProcessingStatus.Failed;
				summary.IsComplete = true;
			}
			_context.SaveChanges();
			_logger.Log(LogLevel.Information, $"Failed {incompleteSummaries.Count()} incomplete summaries.");
		}
	}
}
