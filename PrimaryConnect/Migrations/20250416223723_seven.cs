using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class seven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "marks");

            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "marks",
                newName: "mark");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "marks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_marks_SubjectId",
                table: "marks",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_marks_subjects_SubjectId",
                table: "marks",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_marks_subjects_SubjectId",
                table: "marks");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropIndex(
                name: "IX_marks_SubjectId",
                table: "marks");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "marks");

            migrationBuilder.RenameColumn(
                name: "mark",
                table: "marks",
                newName: "Mark");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "marks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
