using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimaryConnect.Migrations
{
    /// <inheritdoc />
    public partial class sdc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatMessages_Person_UserId",
                table: "chatMessages");

            migrationBuilder.DropIndex(
                name: "IX_chatMessages_UserId",
                table: "chatMessages");

            migrationBuilder.RenameColumn(
                name: "datetime",
                table: "chatMessages",
                newName: "UserName");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "chatMessages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "chatMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_PersonId",
                table: "chatMessages",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_chatMessages_Person_PersonId",
                table: "chatMessages",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatMessages_Person_PersonId",
                table: "chatMessages");

            migrationBuilder.DropIndex(
                name: "IX_chatMessages_PersonId",
                table: "chatMessages");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "chatMessages");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "chatMessages");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "chatMessages",
                newName: "datetime");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_UserId",
                table: "chatMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_chatMessages_Person_UserId",
                table: "chatMessages",
                column: "UserId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
