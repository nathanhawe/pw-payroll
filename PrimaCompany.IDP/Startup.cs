// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrimaCompany.IDP.DbContexts;
using PrimaCompany.IDP.Entities;
using PrimaCompany.IDP.Services;
using System;
using System.Linq;
using System.Reflection;

namespace PrimaCompany.IDP
{
	public class Startup
	{
		public IWebHostEnvironment Environment { get; }
		public IConfiguration Configuration { get; }

		public Startup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var idpConnectionString = Configuration.GetConnectionString("IDPConnection");
			var identityDataConnectionString = Configuration.GetConnectionString("IdentityDataConnection");

			services.AddControllersWithViews();

			services.AddDbContext<IdentityDbContext>(options =>
			{
				options.UseSqlServer(identityDataConnectionString);
			});

			services.AddScoped<IPasswordHasher<Entities.User>, PasswordHasher<Entities.User>>();
			services.AddScoped<ILocalUserService, LocalUserService>();

			var builder = services.AddIdentityServer();

			builder.AddProfileService<LocalUserProfileService>();

			// not recommended for production - you need to store your key material somewhere secure
			builder.AddDeveloperSigningCredential();

			var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

			

			builder.AddConfigurationStore(options =>
			{
				options.ConfigureDbContext = builder => builder.UseSqlServer(idpConnectionString, options => options.MigrationsAssembly(migrationsAssembly));
			});

			builder.AddOperationalStore(options =>
			{
				options.ConfigureDbContext = builder => builder.UseSqlServer(idpConnectionString, options => options.MigrationsAssembly(migrationsAssembly));
			});
		}

		public void Configure(IApplicationBuilder app)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			
			// uncomment if you want to add MVC
			app.UseStaticFiles();
			app.UseRouting();

			app.UseIdentityServer();

			// uncomment, if you want to add MVC
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
			
		}
	}
}
