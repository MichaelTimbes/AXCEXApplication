using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AXCEXONLINE.Migrations
{
    public partial class ChangesCustName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerFKID",
                table: "ProjectModel",
                newName: "CustomerName");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "ProjectModel",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "ProjectModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "ProjectModel");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "ProjectModel",
                newName: "CustomerFKID");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerFKID",
                table: "ProjectModel",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
