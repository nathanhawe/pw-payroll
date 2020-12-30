using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Payroll.Data;
using Payroll.Domain.Constants;
using Payroll.Infrastructure.Authorization;
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
			// Explicitly define which origins are allowed in CORS.
			var origins = Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder
						.WithOrigins(origins)
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			// Set serialization naming policies to camel case. These settings do not affect the automatic response sent
			// during model binding errors.
			services.AddControllers()
				.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			});

			// HTTP context accessor is used by the authorization handlers defined below.
			services.AddHttpContextAccessor();
			services.AddScoped<IAuthorizationHandler, SubjectMustMatchUserHandler>();
			services.AddScoped<IAuthorizationHandler, AccessLevelHandler>();

			// Email and communication services
			services.AddScoped<Payroll.Infrastructure.Email.IEmailService>(x =>
				ActivatorUtilities.CreateInstance<Payroll.Infrastructure.Email.SmtpEmailService>(
					x,
					Configuration["Email:ServerAddress"],
					Configuration["Email:FromMailboxName"],
					Configuration["Email:FromMailboxAddress"],
					(int.TryParse(Configuration["Email:Port"], out int port) ? port : 25))
				);
			services.AddSingleton<Infrastructure.ErrorReporting.IErrorReportingService, 
				Infrastructure.ErrorReporting.EmailErrorReportingService>();

			// Define authorization policies
			services.AddAuthorization(authorizationOptions =>
			{
				authorizationOptions.AddPolicy(
					AuthorizationPolicyConstants.SubjectMustMatchUser,
					policyBuilder =>
					{
						policyBuilder.RequireAuthenticatedUser();
						policyBuilder.AddRequirements(new SubjectMustMatchUserRequirement());
					});

				authorizationOptions.AddPolicy(
					AuthorizationPolicyConstants.MustBeViewingUser,
					policyBuilder =>
					{
						policyBuilder.RequireAuthenticatedUser();
						policyBuilder.AddRequirements(new AccessLevelRequirement(AccessLevel.Viewer.ToString(), AccessLevel.BatchCreator.ToString(), AccessLevel.Administrator.ToString()));
					});

				authorizationOptions.AddPolicy(
					AuthorizationPolicyConstants.MustBeBatchCreatingUser,
					policyBuilder =>
					{
						policyBuilder.RequireAuthenticatedUser();
						policyBuilder.AddRequirements(new AccessLevelRequirement(AccessLevel.BatchCreator.ToString(), AccessLevel.Administrator.ToString()));
					});

				authorizationOptions.AddPolicy(
					AuthorizationPolicyConstants.MustBeAdministrationUser,
					policyBuilder =>
					{
						policyBuilder.RequireAuthenticatedUser();
						policyBuilder.AddRequirements(new AccessLevelRequirement(AccessLevel.Administrator.ToString()));
					});
			});

			// Add IdentityServer4 authentication service
			services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					options.Authority = Configuration["IdentityProvider:Authority"];
					options.ApiName = Configuration["IdentityProvider:ApiName"];
					options.ApiSecret = Configuration["IdentityProvider:ApiSecret"];
				});

			// Inject the database context for application data
			services.AddDbContext<PayrollContext>(opt =>
				opt.UseSqlServer(Configuration.GetConnectionString("PayrollConnection"))
			);
			
			// Inject the Quick Base connection
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
			services.AddScoped<Service.Interface.IApplicationUserProfileService, Service.ApplicationUserProfileService>();
			services.AddScoped<Service.Interface.ISummaryBatchService, Service.SummaryBatchService>();
			services.AddScoped<Service.Interface.ISummaryCreationService, Service.SummaryCreationService>();
			services.AddScoped<Service.Interface.IAuditLockBatchService, Service.AuditLockBatchService>();
			services.AddScoped<Service.Interface.IAuditLockService, Service.AuditLockService>();
			services.AddTransient<Service.Interface.IRestartRecoveryService, Service.RestartRecoveryService>();


			// Add application repositories
			services.AddScoped<Data.QuickBase.IPslTrackingDailyRepo, Data.QuickBase.PslTrackingDailyRepo>();
			services.AddScoped<Data.QuickBase.ICrewBossPayRepo, Data.QuickBase.CrewBossPayRepo>();
			services.AddScoped<Data.QuickBase.IRanchPayrollRepo, Data.QuickBase.RanchPayrollRepo>();
			services.AddScoped<Data.QuickBase.IRanchPayrollAdjustmentRepo, Data.QuickBase.RanchPayrollAdjustmentRepo>();
			services.AddScoped<Data.QuickBase.IRanchSummariesRepo, Data.QuickBase.RanchSummariesRepo>();
			services.AddScoped<Data.QuickBase.IPlantPayrollRepo, Data.QuickBase.PlantPayrollRepo>();
			services.AddScoped<Data.QuickBase.IPlantPayrollAdjustmentRepo, Data.QuickBase.PlantPayrollAdjustmentRepo>();
			services.AddScoped<Data.QuickBase.IPlantSummariesRepo, Data.QuickBase.PlantSummariesRepo>();
			services.AddScoped<Data.QuickBase.IRanchPayrollOutRepo, Data.QuickBase.RanchPayrollOutRepo>();
			services.AddScoped<Data.QuickBase.IRanchPayrollAdjustmentOutRepo, Data.QuickBase.RanchPayrollAdjustmentOutRepo>();
			services.AddScoped<Data.QuickBase.IPlantPayrollOutRepo, Data.QuickBase.PlantPayrollOutRepo>();
			services.AddScoped<Data.QuickBase.IPlantPayrollAdjustmentOutRepo, Data.QuickBase.PlantPayrollAdjustmentOutRepo>();


			// Add hosted services
			services.AddHostedService<Infrastructure.HostedServices.QueuedHostedService>();
			services.AddSingleton<Infrastructure.HostedServices.IBackgroundTaskQueue, Infrastructure.HostedServices.BackgroundTaskQueue>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Payroll.Service.Interface.IRestartRecoveryService restartRecoveryService)
		{

			app.UseApiExceptionHandler();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			// Invoke the restart recovery service to handle incomplete batches
			restartRecoveryService.HandleRestart();

		}
	}
}
