using Microsoft.EntityFrameworkCore;
using Payroll.Domain;
using System;

namespace Payroll.Data
{
	public class PayrollContext : DbContext
	{
		public PayrollContext()
		{

		}
		public PayrollContext(DbContextOptions<PayrollContext> options)
			: base(options)
		{

		}

		private DateTime DefaultDate { get; set; } = new DateTime(2000, 1, 1);

		public DbSet<Batch> Batches { get; set; }
		public DbSet<MinimumWage> MinimumWages { get; set; }
		public DbSet<CrewBossWage> CrewBossWages { get; set; }
		public DbSet<CrewBossPayLine> CrewBossPayLines { get; set; }
		public DbSet<RanchPayLine> RanchPayLines { get; set; }
		public DbSet<PaidSickLeave> PaidSickLeaves { get; set; }
		public DbSet<CrewLaborWage> CrewLaborWages { get; set; }
		public DbSet<RanchAdjustmentLine> RanchAdjustmentLines { get; set; }
		public DbSet<RanchSummary> RanchSummaries { get; set; }
		public DbSet<PlantPayLine> PlantPayLines { get; set; }
		public DbSet<PlantAdjustmentLine> PlantAdjustmentLines { get; set; }
		public DbSet<PlantSummary> PlantSummaries { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			SeedMinimumWage(modelBuilder);
			SeedCrewBossWage(modelBuilder);
			SeedCrewLaborWage(modelBuilder);
		}

		protected void SeedMinimumWage(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MinimumWage>().HasData(
				CreateMinimumWage(1, new DateTime(2010, 1, 1), 8),
				CreateMinimumWage(2, new DateTime(2014, 7, 1), 9),
				CreateMinimumWage(3, new DateTime(2016, 1, 1), 10),
				CreateMinimumWage(4, new DateTime(2017, 1, 1), 10.5M),
				CreateMinimumWage(5, new DateTime(2018, 1, 1), 11),
				CreateMinimumWage(6, new DateTime(2019, 1, 1), 12),
				CreateMinimumWage(7, new DateTime(2020, 1, 1), 13),
				CreateMinimumWage(8, new DateTime(2021, 1, 1), 14),
				CreateMinimumWage(9, new DateTime(2022, 1, 1), 15));

		}

		protected void SeedCrewBossWage(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CrewBossWage>().HasData(
				CreateCrewBossWage(1, new DateTime(2019, 12, 2), 36, 24.5M),
				CreateCrewBossWage(2, new DateTime(2019, 12, 2), 35, 24),
				CreateCrewBossWage(3, new DateTime(2019, 12, 2), 34, 23.5M),
				CreateCrewBossWage(4, new DateTime(2019, 12, 2), 33, 23),
				CreateCrewBossWage(5, new DateTime(2019, 12, 2), 32, 22.5M),
				CreateCrewBossWage(6, new DateTime(2019, 12, 2), 31, 22),
				CreateCrewBossWage(7, new DateTime(2019, 12, 2), 30, 21.5M),
				CreateCrewBossWage(8, new DateTime(2019, 12, 2), 29, 21),
				CreateCrewBossWage(9, new DateTime(2019, 12, 2), 28, 20.5M),
				CreateCrewBossWage(10, new DateTime(2019, 12, 2), 27, 20),
				CreateCrewBossWage(11, new DateTime(2019, 12, 2), 26, 19.5M),
				CreateCrewBossWage(12, new DateTime(2019, 12, 2), 25, 19),
				CreateCrewBossWage(13, new DateTime(2019, 12, 2), 24, 18.5M),
				CreateCrewBossWage(14, new DateTime(2019, 12, 2), 23, 18.25M),
				CreateCrewBossWage(15, new DateTime(2019, 12, 2), 22, 18),
				CreateCrewBossWage(16, new DateTime(2019, 12, 2), 21, 17.75M),
				CreateCrewBossWage(17, new DateTime(2019, 12, 2), 20, 17.5M),
				CreateCrewBossWage(18, new DateTime(2019, 12, 2), 19, 17.25M),
				CreateCrewBossWage(19, new DateTime(2019, 12, 2), 18, 17),
				CreateCrewBossWage(20, new DateTime(2019, 12, 2), 17, 16.75M),
				CreateCrewBossWage(21, new DateTime(2019, 12, 2), 16, 16.5M),
				CreateCrewBossWage(22, new DateTime(2019, 12, 2), 15, 16.25M),
				CreateCrewBossWage(23, new DateTime(2019, 12, 2), 0, 16));
		}

		private void SeedCrewLaborWage(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CrewLaborWage>().HasData(
				CreateCrewLaborWage(1, new DateTime(2000, 1, 1), 13),
				CreateCrewLaborWage(2, new DateTime(2020, 1, 21), 14));
		}

		protected MinimumWage CreateMinimumWage(int id, DateTime effectiveDate, decimal wage)
		{
			return new MinimumWage
			{
				DateCreated = DefaultDate,
				DateModified = DefaultDate,
				Id = id,
				EffectiveDate = effectiveDate,
				Wage = wage
			};
		}

		protected CrewBossWage CreateCrewBossWage(int id, DateTime effectiveDate, int workerCountThreshold, decimal wage)
		{
			return new CrewBossWage
			{
				DateCreated = DefaultDate,
				DateModified = DefaultDate,
				EffectiveDate = effectiveDate,
				Id = id,
				WorkerCountThreshold = workerCountThreshold,
				Wage = wage
			};
		}

		protected CrewLaborWage CreateCrewLaborWage(int id, DateTime effectiveDate, decimal wage)
		{
			return new CrewLaborWage
			{
				DateCreated = DefaultDate,
				DateModified = DefaultDate,
				Id = id,
				EffectiveDate = effectiveDate,
				Wage = wage
			};
		}
	}


}
