using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEXONLINE.Migrations
{
    public partial class SCOPE_WBS_WBSASSIGNMENTS_ADDED : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    SCOPE_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Project_ID = table.Column<int>(nullable: false),
                    SCOPE_VERSION_AUTHOR = table.Column<string>(nullable: true),
                    SCOPE_END_DATE = table.Column<DateTime>(nullable: false),
                    SCOPE_EXPECTATIONS = table.Column<string>(nullable: true),
                    SCOPE_GOALS = table.Column<string>(nullable: true),
                    SCOPE_LIMITATIONS = table.Column<string>(nullable: true),
                    SCOPE_MANAGER = table.Column<string>(nullable: true),
                    SCOPE_PHASE = table.Column<string>(nullable: true),
                    SCOPE_START_DATE = table.Column<DateTime>(nullable: false),
                    SCOPE_SUMMARY = table.Column<string>(nullable: true),
                    SCOPE_VERSION = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.SCOPE_ID);
                });

            migrationBuilder.CreateTable(
                name: "WBSAssignments",
                columns: table => new
                {
                    WBS_ASSIGNMENT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ASSIGNING_MANAGER = table.Column<string>(nullable: true),
                    EMPLOYEE_ID = table.Column<int>(nullable: false),
                    WBS_COMPLETION_STAT = table.Column<bool>(nullable: false),
                    WBS_AWAITING_COMPLETION_STAT = table.Column<bool>(nullable: false),
                    WBS_COMPLETION_VERIFIED = table.Column<bool>(nullable: false),
                    WBS_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBSAssignments", x => x.WBS_ASSIGNMENT_ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkBreakDowns",
                columns: table => new
                {
                    WBS_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WBS_ASSIGNED_BY = table.Column<string>(nullable: true),
                    WBS_END_DATE = table.Column<DateTime>(nullable: false),
                    Project_ID = table.Column<int>(nullable: false),
                    WBS_START_DATE = table.Column<DateTime>(nullable: false),
                    WBS_TOTAL_COST = table.Column<decimal>(type: "money", nullable: false),
                    MAX_HOURS = table.Column<double>(nullable: false),
                    WBS_SUMMARY = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkBreakDowns", x => x.WBS_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scopes");

            migrationBuilder.DropTable(
                name: "WBSAssignments");

            migrationBuilder.DropTable(
                name: "WorkBreakDowns");
        }
    }
}
