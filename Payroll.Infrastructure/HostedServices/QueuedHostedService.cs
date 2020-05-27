﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Payroll.Infrastructure.HostedServices
{
	public class QueuedHostedService : BackgroundService
	{
		private readonly ILogger _logger;
		public IBackgroundTaskQueue TaskQueue { get; }

		public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
		{
			TaskQueue = taskQueue;
			_logger = loggerFactory.CreateLogger<QueuedHostedService>();
		}

		protected async override Task ExecuteAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Queued Hosted Service is starting.");

			while (!cancellationToken.IsCancellationRequested)
			{
				var workItem = await TaskQueue.DequeueAsync(cancellationToken);
				try
				{
					await workItem(cancellationToken);
				}
				catch(Exception ex)
				{
					_logger.LogError(ex, "Error occured executing {workItem}.", nameof(workItem));
				}
			}

			_logger.LogInformation("Queued Hosted Service is stopping.");
		}
	}
}
