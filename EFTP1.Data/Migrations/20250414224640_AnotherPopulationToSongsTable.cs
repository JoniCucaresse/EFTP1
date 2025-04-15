using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EFTP1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnotherPopulationToSongsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "ArtistId", "Duration", "Gender", "Title" },
                values: new object[,]
                {
                    { 6, 1, 4, "Funk Rock", "Otherside" },
                    { 7, 1, 3, "Funk Rock", "Scar tissue" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
