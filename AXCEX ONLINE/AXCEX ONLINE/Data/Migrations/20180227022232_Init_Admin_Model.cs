using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEX_ONLINE.Data.Migrations
{
    public partial class Init_Admin_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ADMIN_ID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ADMIN_NAME",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADMIN_ID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ADMIN_NAME",
                table: "AspNetUsers");
        }
    }
}
