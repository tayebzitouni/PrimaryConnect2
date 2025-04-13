using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class five : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "homeworks",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "homeworks",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "DateAssigned",
                table: "homeworks",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "homeworks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    TeacherRemark = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "homeworks");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "homeworks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "homeworks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "homeworks",
                newName: "DateAssigned");
        }
    }
}
