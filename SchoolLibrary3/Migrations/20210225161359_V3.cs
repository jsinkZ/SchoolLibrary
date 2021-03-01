using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolLibrary3.Migrations
{
    public partial class V3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statuses_Books_bookId",
                table: "Statuses");

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Books",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Books");

            migrationBuilder.AddForeignKey(
                name: "FK_Statuses_Books_bookId",
                table: "Statuses",
                column: "bookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
