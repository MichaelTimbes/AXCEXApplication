using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEX_ONLINE.Data.Migrations
{
    public partial class ChangestoAdmin2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADMINID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ADMIN_NAME",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ADMINID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ADMIN_NAME",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
