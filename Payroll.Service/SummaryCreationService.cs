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
	/// Manages the summary creation workflow
	/// </summary>
	public class SummaryCreationService : ISummaryCreationService
	{
		private readonly ILogger<SummaryCreationService> _logger;

		// Database context
		private readonly Data.PayrollContext _context;

		// Repositories
		private readonly IRanchPayrollOutRepo _ranchPayrollOutRepo;
		private readonly IRanchSummariesRepo _ranchSummariesRepo;
		private readonly IPlantPayrollOutRepo _plantPayrollOutRepo;
		private readonly IPlantSummariesRepo _plantSummariesRepo;

		// Services
		private readonly IRanchSummaryService _ranchSummaryService;
		private readonly IPlantSummaryService _plantSummaryService;

		public SummaryCreationService(
			ILogger<SummaryCreationService> logger,
			Data.PayrollContext payrollContext,
			IRanchPayrollOutRepo ranchPayrollOutRepo,
			IRanchSummariesRepo ranchSummariesRepo,
			IPlantPayrollOutRepo plantPayrollOutRepo,
			IPlantSummariesRepo plantSummariesRepo,
			IRanchSummaryService ranchSummaryService,
			IPlantSummaryService plantSummaryService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_context = payrollContext ?? throw new ArgumentNullException(nameof(payrollContext));
			_ranchPayrollOutRepo = ranchPayrollOutRepo ?? throw new ArgumentNullException(nameof(ranchPayrollOutRepo));
			_ranchSummariesRepo = ranchSummariesRepo ?? throw new ArgumentNullException(nameof(ranchSummariesRepo));
			_plantPayrollOutRepo = plantPayrollOutRepo ?? throw new ArgumentNullException(nameof(plantPayrollOutRepo));
			_plantSummariesRepo = plantSummariesRepo ?? throw new ArgumentNullException(nameof(plantSummariesRepo));
			_ranchSummaryService = ranchSummaryService ?? throw new ArgumentNullException(nameof(ranchSummaryService));
			_plantSummaryService = plantSummaryService ?? throw new ArgumentNullException(nameof(plantSummaryService));
		}

		public void CreateSummaries(int summaryBatchId)
		{
			try
			{
				_logger.Log(LogLevel.Information, "Starting summary creation for summary batch {summaryBatchId}", summaryBatchId);
				var summaryBatch = _context.SummaryBatches.Where(x => x.Id == summaryBatchId).FirstOrDefault();
				if (summaryBatch == null) throw new Exception($"The provided summary batch ID of '{summaryBatchId}' was not found in the database.");
				SetSummaryBatchStatus(summaryBatchId, BatchProcessingStatus.Starting);

				switch (summaryBatch.Company)
				{
					case Company.Plants: PerformPlantCalculations(summaryBatch); break;
					case Company.Ranches: PerformRanchCalculations(summaryBatch); break;
					default: throw new Exception($"Unknown company value '{summaryBatch.Company}'.");
				}
				_logger.Log(LogLevel.Information, "Completed summary creation for summary batch {batchId}", summaryBatchId);
				SetSummaryBatchStatus(summaryBatchId, BatchProcessingStatus.Success);
			}
			catch(Exception ex)
			{
				SetSummaryBatchStatus(summaryBatchId, BatchProcessingStatus.Failed);
				throw ex;
			}
		}

		private void PerformPlantCalculations(SummaryBatch summaryBatch)
		{
			/* Download Quick Base data */
			_logger.Log(LogLevel.Information, "Downloading plant payroll data from Quick Base for {summaryBatchId}", summaryBatch.Id);
			SetSummaryBatchStatus(summaryBatch.Id, BatchProcessingStatus.Downloading);
			
			var plantPayLines = _plantPayrollOutRepo.GetForSummaries(summaryBatch.WeekEndDate, summaryBatch.LayoffId ?? 0).ToList();

			/* Create Summaries */
			SetSummaryBatchStatus(summaryBatch.Id, BatchProcessingStatus.Summaries);
			_logger.Log(LogLevel.Information, "Calculating plant summaries for summary batch {summaryBatchId}", summaryBatch.Id);
			
			var summaries = _plantSummaryService.CreateSummariesFromList(plantPayLines);

			/* Update records to Quick Base */
			SetSummaryBatchStatus(summaryBatch.Id, BatchProcessingStatus.Uploading);
			_logger.Log(LogLevel.Information, "Updating Quick Base for summary batch {summaryBatchId}", summaryBatch.Id);

			var layoffId = summaryBatch.LayoffId ?? 0;
			summaries.ForEach(x => 
			{ 
				x.LayoffId = layoffId; 
				x.BatchId = summaryBatch.Id; 
			});

			var psResponse = _plantSummariesRepo.Save(summaries);
		}
		
		
		private void PerformRanchCalculations(SummaryBatch summaryBatch)
		{
			/* Download Quick Base data */
			_logger.Log(LogLevel.Information, "Downloading ranch payroll data from Quick Base for {summaryBatchId}", summaryBatch.Id);
			SetSummaryBatchStatus(summaryBatch.Id, BatchProcessingStatus.Downloading);
			
			var ranchPayLines = _ranchPayrollOutRepo.GetForSummaries(summaryBatch.WeekEndDate, summaryBatch.LayoffId ?? 0).ToList();

			/* Create Summaries */
			SetSummaryBatchStatus(summaryBatch.Id, BatchProcessingStatus.Summaries);
			_logger.Log(LogLevel.Information, "Calculating ranch summaries for summary batch {summaryBatchId}", summaryBatch.Id);

			var summaries = _ranchSummaryService.CreateSummariesFromList(ranchPayLines);
			
			/* Update records to Quick Base */
			SetSummaryBatchStatus(summaryBatch.Id, BatchProcessingStatus.Uploading);
			_logger.Log(LogLevel.Information, "Updating Quick Base for summary batch {summaryBatchId}", summaryBatch.Id);

			var layoffId = summaryBatch.LayoffId ?? 0;
			summaries.ForEach(x =>
			{
				x.LayoffId = layoffId;
				x.BatchId = summaryBatch.Id;
			});

			var rsResponse = _ranchSummariesRepo.Save(summaries);
		}

		private void SetSummaryBatchStatus(int id, BatchProcessingStatus status)
		{
			var summaryBatch = _context.SummaryBatches.Find(id);
			if(summaryBatch != null)
			{
				if(status == BatchProcessingStatus.Failed)
				{
					summaryBatch.StatusMessage = $"Summary Batch ID #{summaryBatch.Id} failed while at step '{summaryBatch.ProcessingStatus}'.";
					summaryBatch.IsComplete = true;
					summaryBatch.EndDate = DateTime.UtcNow;
				}
				else if(status == BatchProcessingStatus.Success)
				{
					summaryBatch.IsComplete = true;
					summaryBatch.EndDate = DateTime.UtcNow;
				}
				else if(status == BatchProcessingStatus.Starting)
				{
					summaryBatch.StartDate = DateTime.UtcNow;
				}
				
				summaryBatch.ProcessingStatus = status;
				_context.SaveChanges();
			}
		}
	}
}
