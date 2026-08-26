using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimeFilterIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Index for ReleaseYear filtering (GetAllAnimesQuery.ReleaseYear)
            migrationBuilder.CreateIndex(
                name: "IX_Anime_ReleaseYear",
                table: "Anime",
                column: "ReleaseYear");

            // Index for Status filtering (GetAllAnimesQuery.Status)
            migrationBuilder.CreateIndex(
                name: "IX_Anime_Status",
                table: "Anime",
                column: "Status");

            // Index for CreatedOnUtc sorting/filtering (GetAllAnimesQuery.FromDate, ToDate, SortBy=created)
            migrationBuilder.CreateIndex(
                name: "IX_Anime_CreatedOnUtc",
                table: "Anime",
                column: "CreatedOnUtc");

            // Composite index for common query pattern: IsActive + CreatedOnUtc (base query)
            migrationBuilder.CreateIndex(
                name: "IX_Anime_IsActive_CreatedOnUtc",
                table: "Anime",
                columns: new[] { "IsActive", "CreatedOnUtc" });

            // Trigram index for ILike searches on Title (requires pg_trgm extension)
            // migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            // migrationBuilder.Sql("CREATE INDEX IX_Anime_Title_GIN ON \"Anime\" USING GIN (\"Title\" gin_trgm_ops);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Anime_ReleaseYear",
                table: "Anime");

            migrationBuilder.DropIndex(
                name: "IX_Anime_Status",
                table: "Anime");

            migrationBuilder.DropIndex(
                name: "IX_Anime_CreatedOnUtc",
                table: "Anime");

            migrationBuilder.DropIndex(
                name: "IX_Anime_IsActive_CreatedOnUtc",
                table: "Anime");
        }
    }
}
