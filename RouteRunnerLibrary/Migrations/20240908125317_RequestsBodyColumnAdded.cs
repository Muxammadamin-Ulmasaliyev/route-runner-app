using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteRunnerLibrary.Migrations
{
    /// <inheritdoc />
    public partial class RequestsBodyColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Requests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestsHistory_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestsHistory_RequestId",
                table: "RequestsHistory",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestsHistory");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "Requests");
        }
    }
}
