using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_Teachers_TeacherId",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Students_Teachers_TeacherId",
                table: "Teacher_Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_schools_SchoolId",
                table: "Teachers");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "Person");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_SchoolId",
                table: "Person",
                newName: "IX_Person_SchoolId");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "Person",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "Person",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "Person",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Person",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Permitions",
                table: "Person",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Teacher_SchoolId",
                table: "Person",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Person",
                table: "Person",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "chatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chatMessages_Person_UserId",
                        column: x => x.UserId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_Teacher_SchoolId",
                table: "Person",
                column: "Teacher_SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_UserId",
                table: "chatMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_Person_TeacherId",
                table: "courses",
                column: "TeacherId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_schools_SchoolId",
                table: "Person",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_schools_Teacher_SchoolId",
                table: "Person",
                column: "Teacher_SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Person_ParentId",
                table: "Students",
                column: "ParentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Students_Person_TeacherId",
                table: "Teacher_Students",
                column: "TeacherId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_Person_TeacherId",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_schools_SchoolId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_schools_Teacher_SchoolId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Person_ParentId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Students_Person_TeacherId",
                table: "Teacher_Students");

            migrationBuilder.DropTable(
                name: "chatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_Teacher_SchoolId",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Permitions",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Teacher_SchoolId",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "Teachers");

            migrationBuilder.RenameIndex(
                name: "IX_Person_SchoolId",
                table: "Teachers",
                newName: "IX_Teachers_SchoolId");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SchoolId",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Permitions = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administrators_schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_SchoolId",
                table: "Administrators",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_Teachers_TeacherId",
                table: "courses",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Students_Teachers_TeacherId",
                table: "Teacher_Students",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_schools_SchoolId",
                table: "Teachers",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
