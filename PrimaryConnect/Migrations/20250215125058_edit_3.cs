using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class edit_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arab",
                table: "marks");

            migrationBuilder.DropColumn(
                name: "Frensh",
                table: "marks");

            migrationBuilder.DropColumn(
                name: "Math",
                table: "marks");

            migrationBuilder.DropColumn(
                name: "english",
                table: "marks");

            migrationBuilder.RenameColumn(
                name: "histoy_gio",
                table: "marks",
                newName: "Mark");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "marks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "marks");

           
        }
    }
}
