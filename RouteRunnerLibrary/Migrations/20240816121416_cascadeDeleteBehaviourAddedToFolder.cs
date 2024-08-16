using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteRunnerLibrary.Migrations
{
    /// <inheritdoc />
    public partial class cascadeDeleteBehaviourAddedToFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ParentId",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedRequests_Folders_FolderId",
                table: "SavedRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ParentId",
                table: "Folders",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedRequests_Folders_FolderId",
                table: "SavedRequests",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ParentId",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedRequests_Folders_FolderId",
                table: "SavedRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ParentId",
                table: "Folders",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedRequests_Folders_FolderId",
                table: "SavedRequests",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");
        }
    }
}
