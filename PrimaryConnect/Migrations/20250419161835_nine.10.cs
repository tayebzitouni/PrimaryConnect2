using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class nine10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "events",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "events",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "events",
                newName: "StartTime");

            migrationBuilder.AddColumn<bool>(
                name: "All",
                table: "events",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "All",
                table: "events");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "events");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "events");

            migrationBuilder.DropColumn(
                name: "level",
                table: "events");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "events",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "events",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "events",
                newName: "Location");
        }
    }
}
