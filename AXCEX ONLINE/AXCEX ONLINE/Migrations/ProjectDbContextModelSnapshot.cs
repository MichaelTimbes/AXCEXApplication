﻿// <auto-generated />
using AXCEX_ONLINE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace AXCEXONLINE.Migrations
{
    [DbContext(typeof(ProjectDbContext))]
    partial class ProjectDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AXCEX_ONLINE.Models.EmployeeModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<int>("EMPID");

                    b.Property<string>("EMP_FNAME");

                    b.Property<string>("EMP_LNAME");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int?>("ProjectModelID");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("ProjectModelID");

                    b.ToTable("EmployeeModel");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.ProjectModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjectID");

                    b.Property<string>("Customer")
                        .HasColumnName("CustomerName");

                    b.Property<decimal>("ProjBudget")
                        .HasColumnType("money");

                    b.Property<decimal>("ProjCurentCost")
                        .HasColumnType("money");

                    b.Property<string>("ProjectName")
                        .HasColumnName("ProjectName");

                    b.HasKey("ID");

                    b.ToTable("ProjectModel");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.EmployeeModel", b =>
                {
                    b.HasOne("AXCEX_ONLINE.Models.ProjectModel")
                        .WithMany("AssignedEmployees")
                        .HasForeignKey("ProjectModelID");
                });
#pragma warning restore 612, 618
        }
    }
}
