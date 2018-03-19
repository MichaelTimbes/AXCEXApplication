using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEXONLINE.Migrations
{
    public partial class ADD_NUMERICAL_PHASE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SCOPE_MAX_NUMERICAL_PHASE",
                table: "Scopes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SCOPE_NUMERICAL_PHASE",
                table: "Scopes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SCOPE_MAX_NUMERICAL_PHASE",
                table: "Scopes");

            migrationBuilder.DropColumn(
                name: "SCOPE_NUMERICAL_PHASE",
                table: "Scopes");
        }
    }
}
