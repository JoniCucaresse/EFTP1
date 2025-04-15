using EFTP1.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFTP1.Data
{
    public class SongService:DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=JONI-CUCARESSE\\SQLEXPRESS02; Initial Catalog=SongServiceDb; Trusted_Connection=true; TrustServerCertificate=true;")
                //.EnableSensitiveDataLogging() // Permite ver valores en las consultas
                //.LogTo(Console.WriteLine, LogLevel.Information)
                .UseLazyLoadingProxies(false);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>(entity =>
            {
                entity.ToTable("Songs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(300)
                    .IsRequired();
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(300)
                    .IsRequired();
                entity.HasIndex(e => new { e.Title, e.ArtistId }, "IX_Songs_Title_ArtistId").IsUnique();
                entity.HasOne(e => e.Artist).WithMany(e => e.Songs).HasForeignKey(e => e.ArtistId)
                .OnDelete(DeleteBehavior.ClientNoAction);

                var songList = new List<Song>
                {
                    new Song{Id=6,Title="Otherside", Duration=4, Gender="Funk Rock", ArtistId=1},
                    new Song{Id=7,Title="Scar tissue", Duration=3, Gender="Funk Rock", ArtistId=1}
                };
                entity.HasData(songList);
            });

        }

    }
}
