﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payroll.Data;

namespace Payroll.Data.Migrations
{
    [DbContext(typeof(PayrollContext))]
    [Migration("20200218224403_AddPSL")]
    partial class AddPSL
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Payroll.Domain.Batch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Batches");
                });

            modelBuilder.Entity("Payroll.Domain.CrewBossPayLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BatchId")
                        .HasColumnType("int");

                    b.Property<int>("Crew")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Gross")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("HourlyRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("HoursWorked")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("LayoffId")
                        .HasColumnType("int");

                    b.Property<string>("PayMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuickBaseRecordId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ShiftDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("WeekEndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorkerCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CrewBossPayLines");
                });

            modelBuilder.Entity("Payroll.Domain.CrewBossWage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("Wage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("WorkerCountThreshold")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CrewBossWages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(7374),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(7396),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 24.5m,
                            WorkerCountThreshold = 36
                        },
                        new
                        {
                            Id = 2,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(8994),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9011),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 24m,
                            WorkerCountThreshold = 35
                        },
                        new
                        {
                            Id = 3,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9043),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9046),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 23.5m,
                            WorkerCountThreshold = 34
                        },
                        new
                        {
                            Id = 4,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9049),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9052),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 23m,
                            WorkerCountThreshold = 33
                        },
                        new
                        {
                            Id = 5,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9055),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9058),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 22.5m,
                            WorkerCountThreshold = 32
                        },
                        new
                        {
                            Id = 6,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9061),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9063),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 22m,
                            WorkerCountThreshold = 31
                        },
                        new
                        {
                            Id = 7,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9066),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9068),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 21.5m,
                            WorkerCountThreshold = 30
                        },
                        new
                        {
                            Id = 8,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9072),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9074),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 21m,
                            WorkerCountThreshold = 29
                        },
                        new
                        {
                            Id = 9,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9077),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9080),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 20.5m,
                            WorkerCountThreshold = 28
                        },
                        new
                        {
                            Id = 10,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9083),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9085),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 20m,
                            WorkerCountThreshold = 27
                        },
                        new
                        {
                            Id = 11,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9089),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9091),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 19.5m,
                            WorkerCountThreshold = 26
                        },
                        new
                        {
                            Id = 12,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9094),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9097),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 19m,
                            WorkerCountThreshold = 25
                        },
                        new
                        {
                            Id = 13,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9100),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9102),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 18.5m,
                            WorkerCountThreshold = 24
                        },
                        new
                        {
                            Id = 14,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9105),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9108),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 18.25m,
                            WorkerCountThreshold = 23
                        },
                        new
                        {
                            Id = 15,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9111),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9113),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 18m,
                            WorkerCountThreshold = 22
                        },
                        new
                        {
                            Id = 16,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9116),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9118),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 17.75m,
                            WorkerCountThreshold = 21
                        },
                        new
                        {
                            Id = 17,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9121),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9124),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 17.5m,
                            WorkerCountThreshold = 20
                        },
                        new
                        {
                            Id = 18,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9127),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9129),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 17.25m,
                            WorkerCountThreshold = 19
                        },
                        new
                        {
                            Id = 19,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9132),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9134),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 17m,
                            WorkerCountThreshold = 18
                        },
                        new
                        {
                            Id = 20,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9137),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9140),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 16.75m,
                            WorkerCountThreshold = 17
                        },
                        new
                        {
                            Id = 21,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9143),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9145),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 16.5m,
                            WorkerCountThreshold = 16
                        },
                        new
                        {
                            Id = 22,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9149),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9151),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 16.25m,
                            WorkerCountThreshold = 15
                        },
                        new
                        {
                            Id = 23,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9154),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 145, DateTimeKind.Local).AddTicks(9156),
                            EffectiveDate = new DateTime(2019, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 16m,
                            WorkerCountThreshold = 0
                        });
                });

            modelBuilder.Entity("Payroll.Domain.MinimumWage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("Wage")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("MinimumWages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 140, DateTimeKind.Local).AddTicks(1757),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(3530),
                            EffectiveDate = new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 8m
                        },
                        new
                        {
                            Id = 2,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5542),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5564),
                            EffectiveDate = new DateTime(2014, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 9m
                        },
                        new
                        {
                            Id = 3,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5598),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5601),
                            EffectiveDate = new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 10m
                        },
                        new
                        {
                            Id = 4,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5653),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5657),
                            EffectiveDate = new DateTime(2017, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 10.5m
                        },
                        new
                        {
                            Id = 5,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5660),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5662),
                            EffectiveDate = new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 11m
                        },
                        new
                        {
                            Id = 6,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5665),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5668),
                            EffectiveDate = new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 12m
                        },
                        new
                        {
                            Id = 7,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5671),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5673),
                            EffectiveDate = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 13m
                        },
                        new
                        {
                            Id = 8,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5677),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5679),
                            EffectiveDate = new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 14m
                        },
                        new
                        {
                            Id = 9,
                            DateCreated = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5683),
                            DateModified = new DateTime(2020, 2, 18, 14, 44, 3, 143, DateTimeKind.Local).AddTicks(5686),
                            EffectiveDate = new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            IsDeleted = false,
                            Wage = 15m
                        });
                });

            modelBuilder.Entity("Payroll.Domain.PaidSickLeave", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BatchId")
                        .HasColumnType("int");

                    b.Property<string>("Company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Gross")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Hours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("HoursUsed")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal>("NinetyDayGross")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("NinetyDayHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("ShiftDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("PaidSickLeaves");
                });

            modelBuilder.Entity("Payroll.Domain.RanchPayLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AlternativeWorkWeek")
                        .HasColumnType("bit");

                    b.Property<int>("BatchId")
                        .HasColumnType("int");

                    b.Property<int>("BlockId")
                        .HasColumnType("int");

                    b.Property<int>("Crew")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("FiveEight")
                        .HasColumnType("bit");

                    b.Property<decimal>("GrossFromHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("GrossFromPieces")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("HourlyRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("HoursWorked")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("LaborCode")
                        .HasColumnType("int");

                    b.Property<int>("LayoffId")
                        .HasColumnType("int");

                    b.Property<decimal>("OtDtWotHours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OtDtWotRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OtherGross")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PayType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PieceRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Pieces")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("QuickBaseRecordId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ShiftDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("WeekEndDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("RanchPayLines");
                });
#pragma warning restore 612, 618
        }
    }
}
