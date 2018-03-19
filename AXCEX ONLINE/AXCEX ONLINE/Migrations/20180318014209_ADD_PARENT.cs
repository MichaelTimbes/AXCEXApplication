using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEXONLINE.Migrations
{
    public partial class ADD_PARENT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SCOPE_PARENT_ID",
                table: "Scopes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SCOPE_PARENT_ID",
                table: "Scopes");
        }
    }
}
