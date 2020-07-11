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
using System.Security.Cryptography.X509Certificates;

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
			services.AddScoped<IEmailService>(x =>
				ActivatorUtilities.CreateInstance<SmtpEmailService>(
					x,
					Configuration["Email:ServerAddress"],
					Configuration["Email:FromMailboxName"],
					Configuration["Email:FromMailboxAddress"],
					(int.TryParse(Configuration["Email:Port"], out int port) ? port : 25))
				);

			var builder = services.AddIdentityServer();

			builder.AddProfileService<LocalUserProfileService>();

			if (Environment.IsDevelopment())
			{
				builder.AddDeveloperSigningCredential();
			}
			else
			{
				builder.AddSigningCredential(LoadCertificateFromStore());
			}

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

		public X509Certificate2 LoadCertificateFromStore()
		{
			string thumbprint = Configuration["SigningCertificate:Thumbprint"];
			using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			store.Open(OpenFlags.ReadOnly);
			var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
			if(certCollection.Count == 0)
			{
				throw new Exception($"The certificate with thumbprint '{thumbprint}' was not found.");
			}
			return certCollection[0];
		}
	}
}
