using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteRunnerLibrary.Migrations
{
    /// <inheritdoc />
    public partial class requestIdAddedToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "RequestsHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "RequestsHistory");
        }
    }
}
