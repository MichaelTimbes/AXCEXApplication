using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEX_ONLINE.Data.Migrations
{
    public partial class EmployeeModel_003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMP_ID",
                table: "AspNetUsers",
                newName: "EMPID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMPID",
                table: "AspNetUsers",
                newName: "EMP_ID");
        }
    }
}
