using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEX_ONLINE.Data.Migrations
{
    public partial class EmployeeModel_002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMP_ID_NUM",
                table: "AspNetUsers",
                newName: "EMP_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMP_ID",
                table: "AspNetUsers",
                newName: "EMP_ID_NUM");
        }
    }
}
