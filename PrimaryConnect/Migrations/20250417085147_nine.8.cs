using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class nine8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Classes_ClassId",
                table: "Teacher");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Teacher",
                newName: "Classid");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_ClassId",
                table: "Teacher",
                newName: "IX_Teacher_Classid");

            migrationBuilder.AlterColumn<int>(
                name: "Classid",
                table: "Teacher",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "requests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "requests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Teacher_Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeacherId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher_Class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_Class_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teacher_Class_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requests_ParentId",
                table: "requests",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_requests_TeacherId",
                table: "requests",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Class_ClassId",
                table: "Teacher_Class",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Class_TeacherId",
                table: "Teacher_Class",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_requests_Parents_ParentId",
                table: "requests",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_requests_Teacher_TeacherId",
                table: "requests",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Classes_Classid",
                table: "Teacher",
                column: "Classid",
                principalTable: "Classes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requests_Parents_ParentId",
                table: "requests");

            migrationBuilder.DropForeignKey(
                name: "FK_requests_Teacher_TeacherId",
                table: "requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Classes_Classid",
                table: "Teacher");

            migrationBuilder.DropTable(
                name: "Teacher_Class");

            migrationBuilder.DropIndex(
                name: "IX_requests_ParentId",
                table: "requests");

            migrationBuilder.DropIndex(
                name: "IX_requests_TeacherId",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "requests");

            migrationBuilder.RenameColumn(
                name: "Classid",
                table: "Teacher",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_Classid",
                table: "Teacher",
                newName: "IX_Teacher_ClassId");

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "Teacher",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Classes_ClassId",
                table: "Teacher",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
