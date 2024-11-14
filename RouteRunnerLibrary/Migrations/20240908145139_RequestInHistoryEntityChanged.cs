using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteRunnerLibrary.Migrations
{
    /// <inheritdoc />
    public partial class RequestInHistoryEntityChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestsHistory_Requests_RequestId",
                table: "RequestsHistory");

            migrationBuilder.DropIndex(
                name: "IX_RequestsHistory_RequestId",
                table: "RequestsHistory");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "RequestsHistory",
                newName: "HttpVerb");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "RequestsHistory",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RequestsHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "RequestsHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "RequestsHistory");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RequestsHistory");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "RequestsHistory");

            migrationBuilder.RenameColumn(
                name: "HttpVerb",
                table: "RequestsHistory",
                newName: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestsHistory_RequestId",
                table: "RequestsHistory",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestsHistory_Requests_RequestId",
                table: "RequestsHistory",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
