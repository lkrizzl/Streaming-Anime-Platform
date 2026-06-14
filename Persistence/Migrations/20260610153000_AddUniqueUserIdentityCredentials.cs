using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    [Migration("20260610153000_AddUniqueUserIdentityCredentials")]
    public partial class AddUniqueUserIdentityCredentials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_Email",
                table: "UserIdentities",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_Username",
                table: "UserIdentities",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserIdentities_Email",
                table: "UserIdentities");

            migrationBuilder.DropIndex(
                name: "IX_UserIdentities_Username",
                table: "UserIdentities");
        }
    }
}