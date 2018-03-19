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
    [Migration("20180318003035_ADD_NUMERICAL_PHASE")]
    partial class ADD_NUMERICAL_PHASE
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AXCEX_ONLINE.Models.CustomerModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CUSTOMER_ID");

                    b.Property<string>("CUSTOMER_ACCOUNT")
                        .HasColumnName("CUSTOMER_ACCOUNT_NUM");

                    b.Property<string>("CUSTOMER_EMAIL")
                        .HasColumnName("CUSTOMER_EMAIL");

                    b.Property<string>("CUSTOMER_NAME")
                        .HasColumnName("CUSTOMER_NAME");

                    b.HasKey("ID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.ProjectAssignment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EmpKey");

                    b.Property<int>("ProjKey");

                    b.Property<string>("authorized_assignment");

                    b.HasKey("ID");

                    b.ToTable("ProjectAssignments");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.ProjectModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjectID");

                    b.Property<string>("Customer")
                        .HasColumnName("CustomerName");

                    b.Property<DateTime>("EndDate")
                        .HasColumnName("EndDate");

                    b.Property<bool>("IsActive")
                        .HasColumnName("ActiveProject");

                    b.Property<decimal>("ProjBudget")
                        .HasColumnType("money");

                    b.Property<decimal>("ProjCurentCost")
                        .HasColumnType("money");

                    b.Property<string>("ProjectName")
                        .HasColumnName("ProjectName");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("StartDate");

                    b.HasKey("ID");

                    b.ToTable("ProjectModel");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.ScopeModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SCOPE_ID");

                    b.Property<int>("ProjectId")
                        .HasColumnName("Project_ID");

                    b.Property<string>("ScopeAuthor")
                        .HasColumnName("SCOPE_VERSION_AUTHOR");

                    b.Property<DateTime>("ScopeEndDate")
                        .HasColumnName("SCOPE_END_DATE");

                    b.Property<string>("ScopeExpectations")
                        .HasColumnName("SCOPE_EXPECTATIONS");

                    b.Property<string>("ScopeGoals")
                        .HasColumnName("SCOPE_GOALS");

                    b.Property<string>("ScopeLimitations")
                        .HasColumnName("SCOPE_LIMITATIONS");

                    b.Property<string>("ScopeManager")
                        .HasColumnName("SCOPE_MANAGER");

                    b.Property<int>("ScopeMaxPhaseNumber")
                        .HasColumnName("SCOPE_MAX_NUMERICAL_PHASE");

                    b.Property<string>("ScopePhase")
                        .HasColumnName("SCOPE_PHASE");

                    b.Property<int>("ScopePhaseNumber")
                        .HasColumnName("SCOPE_NUMERICAL_PHASE");

                    b.Property<DateTime>("ScopeStartDate")
                        .HasColumnName("SCOPE_START_DATE");

                    b.Property<string>("ScopeSummary")
                        .HasColumnName("SCOPE_SUMMARY");

                    b.Property<int>("ScopeVersion")
                        .HasColumnName("SCOPE_VERSION");

                    b.HasKey("ID");

                    b.ToTable("Scopes");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.WBSAssignment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("WBS_ASSIGNMENT_ID");

                    b.Property<string>("AuthorizedBy")
                        .HasColumnName("ASSIGNING_MANAGER");

                    b.Property<int>("EmpKey")
                        .HasColumnName("EMPLOYEE_ID");

                    b.Property<bool>("IsComplete")
                        .HasColumnName("WBS_COMPLETION_STAT");

                    b.Property<bool>("IsCompleteAwaiting")
                        .HasColumnName("WBS_AWAITING_COMPLETION_STAT");

                    b.Property<bool>("IsCompleteVerified")
                        .HasColumnName("WBS_COMPLETION_VERIFIED");

                    b.Property<int>("WBSKey")
                        .HasColumnName("WBS_ID");

                    b.HasKey("ID");

                    b.ToTable("WBSAssignments");
                });

            modelBuilder.Entity("AXCEX_ONLINE.Models.WBSModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("WBS_ID");

                    b.Property<string>("AssignedBy")
                        .HasColumnName("WBS_ASSIGNED_BY");

                    b.Property<DateTime>("EndDate")
                        .HasColumnName("WBS_END_DATE");

                    b.Property<int>("ProjectId")
                        .HasColumnName("Project_ID");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("WBS_START_DATE");

                    b.Property<decimal>("WBSCost")
                        .HasColumnName("WBS_TOTAL_COST")
                        .HasColumnType("money");

                    b.Property<double>("WBSHours")
                        .HasColumnName("MAX_HOURS");

                    b.Property<string>("WBSSummary")
                        .HasColumnName("WBS_SUMMARY");

                    b.HasKey("ID");

                    b.ToTable("WorkBreakDowns");
                });
#pragma warning restore 612, 618
        }
    }
}