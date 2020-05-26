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
			services.AddScoped<Service.Interface.ICrewBossWageService, Service.CrewBossWageService>();
			services.AddScoped<Service.Interface.ICrewLaborWageService, Service.CrewLaborWageService>();
			services.AddScoped<Service.Interface.IMinimumWageService, Service.MinimumWageService>();
			

			// Add application repositories
			services.AddScoped<Payroll.Data.QuickBase.ICrewBossPayRepo, Payroll.Data.QuickBase.CrewBossPayRepo>();
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
