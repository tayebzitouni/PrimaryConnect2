using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class sds4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "homeworks");

            migrationBuilder.AlterColumn<string>(
                name: "ClassId",
                table: "homeworks",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "teacherId",
                table: "homeworks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "teacherId",
                table: "homeworks");

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "homeworks",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "homeworks",
                type: "INTEGER",
                nullable: true);
        }
    }
}
