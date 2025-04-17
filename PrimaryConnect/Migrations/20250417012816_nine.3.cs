using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class nine3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_documents_Parents_ParentId",
                table: "documents");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "documents",
                newName: "personid");

            migrationBuilder.RenameIndex(
                name: "IX_documents_ParentId",
                table: "documents",
                newName: "IX_documents_personid");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_Person_personid",
                table: "documents",
                column: "personid",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_documents_Person_personid",
                table: "documents");

            migrationBuilder.RenameColumn(
                name: "personid",
                table: "documents",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_documents_personid",
                table: "documents",
                newName: "IX_documents_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_documents_Parents_ParentId",
                table: "documents",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
