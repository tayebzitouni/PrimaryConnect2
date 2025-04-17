using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class nine9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Class_Classes_ClassId",
                table: "Teacher_Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Class_Teacher_TeacherId",
                table: "Teacher_Class");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teacher_Class",
                table: "Teacher_Class");

            migrationBuilder.RenameTable(
                name: "Teacher_Class",
                newName: "TeacherClasses");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_Class_TeacherId",
                table: "TeacherClasses",
                newName: "IX_TeacherClasses_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_Class_ClassId",
                table: "TeacherClasses",
                newName: "IX_TeacherClasses_ClassId");

            migrationBuilder.AddColumn<string>(
                name: "Assignementdate",
                table: "TeacherClasses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherClasses",
                table: "TeacherClasses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClasses_Classes_ClassId",
                table: "TeacherClasses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClasses_Teacher_TeacherId",
                table: "TeacherClasses",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClasses_Classes_ClassId",
                table: "TeacherClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClasses_Teacher_TeacherId",
                table: "TeacherClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherClasses",
                table: "TeacherClasses");

            migrationBuilder.DropColumn(
                name: "Assignementdate",
                table: "TeacherClasses");

            migrationBuilder.RenameTable(
                name: "TeacherClasses",
                newName: "Teacher_Class");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherClasses_TeacherId",
                table: "Teacher_Class",
                newName: "IX_Teacher_Class_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherClasses_ClassId",
                table: "Teacher_Class",
                newName: "IX_Teacher_Class_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teacher_Class",
                table: "Teacher_Class",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Class_Classes_ClassId",
                table: "Teacher_Class",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Class_Teacher_TeacherId",
                table: "Teacher_Class",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
