using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class nine2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprvoved",
                table: "documents",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprvoved",
                table: "documents");
        }
    }
}
