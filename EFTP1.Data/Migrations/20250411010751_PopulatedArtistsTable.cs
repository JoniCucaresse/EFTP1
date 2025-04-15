using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFTP1.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulatedArtistsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Artists",
               columns: new[] { "Id", "Name", "Country", "Birthday" },
               values: new object[,]
                   {
                        {1, "Red Hot Chili Peppers", "USA", "1983"  },
                        {2, "Nirvana","USA", "1987" },
                        {3, "Pink Floyd", "United Kingdom", "1965" },
                        {4, "The Beatles","United Kingdom", "1960" },
                        {5, "Soda Stereo","Argentina", "1982" }
                   }
               );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "Artists",
               keyColumn: "Id",
               keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
