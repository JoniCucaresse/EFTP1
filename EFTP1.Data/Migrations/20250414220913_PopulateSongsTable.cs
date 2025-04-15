using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFTP1.Data.Migrations
{
    /// <inheritdoc />
    public partial class PopulateSongsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Songs (Title, Duration, Gender, ArtistId) VALUES ('Californication', 5, 'Funk Rock', 1)");
            migrationBuilder.Sql("INSERT INTO Songs (Title, Duration, Gender, ArtistId) VALUES ('Come as you are', 3, 'Grunge', 2)");
            migrationBuilder.Sql("INSERT INTO Songs (Title, Duration, Gender, ArtistId) VALUES ('Another brick in the wall', 4, 'Progressive Rock', 3)");
            migrationBuilder.Sql("INSERT INTO Songs (Title, Duration, Gender, ArtistId) VALUES ('Blackbird', 2, 'Rock', 4)");
            migrationBuilder.Sql("INSERT INTO Songs (Title, Duration, Gender, ArtistId) VALUES ('Persiana americana', 5, 'Rock New Wave', 5)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Songs WHERE Id IN (1,2,3,4,5)");
        }
    }
}
