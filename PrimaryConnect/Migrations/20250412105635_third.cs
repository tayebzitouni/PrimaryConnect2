using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "homeworks");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "homeworks");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "homeworks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "homeworks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "homeworks",
                newName: "DateAssigned");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Parents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AssignedToAll",
                table: "homeworks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "homeworks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "homeworks",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "AssignedToAll",
                table: "homeworks");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "homeworks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "homeworks");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "homeworks",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "homeworks",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "DateAssigned",
                table: "homeworks",
                newName: "FileType");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "homeworks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "homeworks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
