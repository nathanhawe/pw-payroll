using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payroll.Data;
using Payroll.Infrastructure.Middleware;

namespace Payroll
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			// Add IdentityServer4 authentication service
			services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					options.Authority = "https://localhost:6001";
					options.ApiName = "timeandattendanceapi";
					options.ApiSecret = "apisecret";
				});

			services.AddDbContext<PayrollContext>(opt =>
				opt.UseSqlServer(Configuration.GetConnectionString("PayrollConnection"))
			);
			
			services.AddScoped<QuickBase.Api.IQuickBaseConnection>(x =>
				ActivatorUtilities.CreateInstance<QuickBase.Api.QuickBaseConnection>(x, Configuration["QuickBase:Realm"], Configuration["QuickBase:UserToken"])
			);

			// Add application services
			services.AddScoped<Service.Interface.IBatchService, Service.BatchService>();
			services.AddScoped<Service.Interface.IRanchHourlyRateSelector, Service.RanchHourlyRateSelector>();
			services.AddScoped<Service.Interface.IPlantHourlyRateSelector, Service.PlantHourlyRateSelector>();
			services.AddScoped<Service.Interface.ICrewBossWageService, Service.CrewBossWageService>();
			services.AddScoped<Service.Interface.ICrewLaborWageService, Service.CrewLaborWageService>();
			services.AddScoped<Service.Interface.IMinimumWageService, Service.MinimumWageService>();
			services.AddScoped<Service.Interface.IGrossFromHoursCalculator, Service.GrossFromHoursCalculator>();
			services.AddScoped<Service.Interface.IGrossFromPiecesCalculator, Service.GrossFromPiecesCalculator>();
			services.AddScoped<Service.Interface.IGrossFromIncentiveCalculator, Service.GrossFromIncentiveCalculator>();
			services.AddScoped<Service.Interface.ITotalGrossCalculator, Service.TotalGrossCalculator>();
			services.AddScoped<Service.Interface.IDailySummaryCalculator, Service.DailySummaryCalculator>();
			services.AddScoped<Service.Interface.IRoundingService, Service.RoundingService>();
			services.AddScoped<Service.Interface.IPaidSickLeaveService, Service.PaidSickLeaveService>();
			services.AddScoped<Service.Interface.ICrewBossPayService, Service.CrewBossPayService>();
			services.AddScoped<Service.Interface.IRanchDailyOTDTHoursCalculator, Service.RanchDailyOTDTHoursCalculator>();
			services.AddScoped<Service.Interface.IRanchWeeklyOTHoursCalculator, Service.RanchWeeklyOTHoursCalculator>();
			services.AddScoped<Service.Interface.IRanchWeeklySummaryCalculator, Service.RanchWeeklySummaryCalculator>();
			services.AddScoped<Service.Interface.IRanchMinimumMakeUpCalculator, Service.RanchMinimumMakeUpCalculator>();
			services.AddScoped<Service.Interface.IRanchSummaryService, Service.RanchSummaryService>();
			services.AddScoped<Service.Interface.IPlantDailyOTDTHoursCalculator, Service.PlantDailyOTDTHoursCalculator>();
			services.AddScoped<Service.Interface.IPlantWeeklyOTHoursCalculator, Service.PlantWeeklyOTHoursCalculator>();
			services.AddScoped<Service.Interface.IPlantWeeklySummaryCalculator, Service.PlantWeeklySummaryCalculator>();
			services.AddScoped<Service.Interface.IPlantMinimumMakeUpCalculator, Service.PlantMinimumMakeUpCalculator>();
			services.AddScoped<Service.Interface.IPlantSummaryService, Service.PlantSummaryService>();
			services.AddScoped<Service.Interface.ITimeAndAttendanceService, Service.TimeAndAttendanceService>();


			// Add application repositories
			services.AddScoped<Payroll.Data.QuickBase.IPslTrackingDailyRepo, Payroll.Data.QuickBase.PslTrackingDailyRepo>();
			services.AddScoped<Payroll.Data.QuickBase.ICrewBossPayRepo, Payroll.Data.QuickBase.CrewBossPayRepo>();
			services.AddScoped<Payroll.Data.QuickBase.IRanchPayrollRepo, Payroll.Data.QuickBase.RanchPayrollRepo>();
			services.AddScoped<Payroll.Data.QuickBase.IRanchPayrollAdjustmentRepo, Payroll.Data.QuickBase.RanchPayrollAdjustmentRepo>();
			services.AddScoped<Payroll.Data.QuickBase.IRanchSummariesRepo, Payroll.Data.QuickBase.RanchSummariesRepo>();
			services.AddScoped<Payroll.Data.QuickBase.IPlantPayrollRepo, Payroll.Data.QuickBase.PlantPayrollRepo>();
			services.AddScoped<Payroll.Data.QuickBase.IPlantPayrollAdjustmentRepo, Payroll.Data.QuickBase.PlantPayrollAdjustmentRepo>();
			services.AddScoped<Payroll.Data.QuickBase.IPlantSummariesRepo, Payroll.Data.QuickBase.PlantSummariesRepo>();

			// Add hosted services
			services.AddHostedService<Payroll.Infrastructure.HostedServices.QueuedHostedService>();
			services.AddSingleton<Payroll.Infrastructure.HostedServices.IBackgroundTaskQueue, Payroll.Infrastructure.HostedServices.BackgroundTaskQueue>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				//app.UseDeveloperExceptionPage();
			}
			app.UseApiExceptionHandler();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
