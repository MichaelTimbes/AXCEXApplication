using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEX_ONLINE.Data.Migrations
{
    public partial class ChangestoAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ADMIN_ID",
                table: "AspNetUsers",
                newName: "ADMINID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ADMINID",
                table: "AspNetUsers",
                newName: "ADMIN_ID");
        }
    }
}
