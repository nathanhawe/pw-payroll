﻿using Microsoft.EntityFrameworkCore;
using PrimaCompany.IDP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrimaCompany.IDP.DbContexts
{
	public class IdentityDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<UserClaim> UserClaims { get; set; }

		public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Subject)
				.IsUnique();

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Username)
				.IsUnique();

			modelBuilder.Entity<User>().HasData(
				new User()
				{
					Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
					Password = "password",
					Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
					Username = "Frank",
					Active = true
				},
				new User()
				{
					Id = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
					Password = "password",
					Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
					Username = "Claire",
					Active = true
				});

			modelBuilder.Entity<UserClaim>().HasData(
				new UserClaim()
				{
					Id = Guid.NewGuid(),
					UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
					Type = "given_name",
					Value = "Frank"
				},
				new UserClaim()
				{
					Id = Guid.NewGuid(),
					UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
					Type = "family_name",
					Value = "Underwood"
				},
				new UserClaim()
				{
					Id = Guid.NewGuid(),
					UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
					Type = "given_name",
					Value = "Claire"
				},
				new UserClaim()
				{
					Id = Guid.NewGuid(),
					UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
					Type = "family_name",
					Value = "Underwood"
				});
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var updatedConcurrencyAwareEntities = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Modified)
				.OfType<IConcurrencyAware>();

			foreach(var entity in updatedConcurrencyAwareEntities)
			{
				entity.ConcurrencyStamp = Guid.NewGuid().ToString();
			}

			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
