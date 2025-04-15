using EFTP1.Consola.Validators;
using EFTP1.Data;
using EFTP1.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EFTP1.Consola
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //CreateDb();
            do
            {
                Console.Clear();
                Console.WriteLine("Main Menu");
                Console.WriteLine("1 - Artists");
                Console.WriteLine("2 - Songs");
                Console.WriteLine("x - Exit");
                Console.Write("Enter an option:");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        ArtistsMenu();
                        break;
                    case "2":
                        SongsMenu();
                        break;
                    case "x":
                        Console.WriteLine("Fin del Programa");
                        return;
                    default:
                        break;
                }

            } while (true);
        }

        private static void SongsMenu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("SONGS");
                Console.WriteLine("1 - List of Songs");
                Console.WriteLine("2 - Add New Songs");
                Console.WriteLine("3 - Delete a Songs");
                Console.WriteLine("4 - Edit a Song");
                Console.WriteLine("r - Return");
                Console.Write("Enter an option:");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        SongsList();
                        break;
                    case "2":
                        AddSongs();
                        break;
                    case "3":
                        DeleteSongs();
                        break;
                    case "4":
                        //EditAuthors(); coming soon
                        break;
                    case "r":
                        return;
                    default:
                        break;
                }

            } while (true);
        }

        private static void DeleteSongs()
        {
            Console.Clear();
            Console.WriteLine("Deleting Songs");
            Console.WriteLine("List of Songs to Delete");
            using (var context = new SongService())
            {
                var songs = context.Songs
                    .OrderBy(s => s.Id)
                    .Select(s => new
                    {
                        s.Id,
                        s.Title
                    }).ToList();
                foreach (var s in songs)
                {
                    Console.WriteLine($"{s.Id} - {s.Title}");
                }
                Console.Write("Select SongID to Delete (0 to Escape):");
                if (!int.TryParse(Console.ReadLine(), out int songId) || songId < 0)
                {
                    Console.WriteLine("Invalid SongID...");
                    Console.ReadLine();
                    return;
                }
                if (songId == 0)
                {
                    Console.WriteLine("Cancelled by user");
                    Console.ReadLine();
                    return;
                }
                var deleteSong = context.Songs.Find(songId);
                if (deleteSong is null)
                {
                    Console.WriteLine("Song does not exist!!!");
                }
                else
                {
                    context.Songs.Remove(deleteSong);
                    context.SaveChanges();
                    Console.WriteLine("Song Successfully Deleted");
                }
                Console.ReadLine();
                return;
            }
        }

        private static void AddSongs()
        {
            Console.Clear();
            Console.WriteLine("Adding New Songs");
            Console.Write("Enter song's title:");
            var title = Console.ReadLine();
            Console.Write("Enter Duration (minutes):");
            if (!int.TryParse(Console.ReadLine(), out var duration))
            {
                Console.WriteLine("Wrong Duration....");
                Console.ReadLine();
                return;
            }
            Console.Write("Enter Gender:");
            var gender = Console.ReadLine();
            Console.WriteLine("List of Artists to Select");
            using (var context = new SongService())
            {
                var artistsList = context.Artists
                    .OrderBy(a => a.Id)
                    .ToList();
                foreach (var artist in artistsList)
                {
                    Console.WriteLine($"{artist.Id} - {artist}");
                }
                Console.Write("Enter ArtistID (0 New Artist):");
                if (!int.TryParse(Console.ReadLine(), out var artistId) || artistId < 0)
                {
                    Console.WriteLine("Invalid ArtistID....");
                    Console.ReadLine();
                    return;
                }
                var selectedArtist = context.Artists.Find(artistId);
                if (selectedArtist is null)
                {
                    Console.WriteLine("Artist not found!!!");
                    Console.ReadLine();
                    return;
                }
                var newSong = new Song
                {
                    Title = title ?? string.Empty,
                    Duration = duration,
                    Gender = gender,
                    ArtistId = artistId
                };

                var songsValidator = new SongsValidator();
                var validationResult = songsValidator.Validate(newSong);

                if (validationResult.IsValid)
                {
                    //bool exist=context.Songs.Any(b=>b.Title.ToLower()== title.ToLower() && 
                    //    b.ArtistId==artistId);
                    var existingSong = context.Songs.FirstOrDefault(s => s.Title.ToLower() == title!.ToLower() &&
                        s.ArtistId == artistId);

                    if (existingSong is null)
                    {
                        context.Songs.Add(newSong);
                        context.SaveChanges();
                        Console.WriteLine("Song Successfully Added!!!");

                    }
                    else
                    {
                        Console.WriteLine("Song duplicated!!!");
                    }

                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine(error);
                    }
                }
                Console.ReadLine();
                return;
            }
        }

        private static void SongsList()
        {
            Console.Clear();
            Console.WriteLine("List of Songs");
            using (var context = new SongService())
            {
                var songs = context.Songs
                    .Include(s => s.Artist)
                    .Select(s => new
                    {
                        s.Id,
                        s.Title,
                        s.Artist
                    })
                    .ToList();
                foreach (var s in songs)
                {
                    Console.WriteLine($"{s.Title} - Artist:{s.Artist}");
                }
            }
            Console.WriteLine("ENTER to continue");
            Console.ReadLine();
        }

        private static void ArtistsMenu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("ARTISTS");
                Console.WriteLine("1 - List of Artists");
                Console.WriteLine("2 - Add New Artist");
                Console.WriteLine("3 - Delete an Artist");
                Console.WriteLine("4 - Edit an Artist");
                Console.WriteLine("r - Return");
                Console.Write("Enter an option:");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        ArtistsList();
                        break;
                    case "2":
                        AddArtists();
                        break;
                    case "3":
                        DeleteArtists();
                        break;
                    case "4":
                        EditArtists();
                        break;
                    case "r":
                        return;
                    default:
                        break;
                }

            } while (true);

        }

        private static void EditArtists()
        {
            Console.Clear();
            Console.WriteLine("Edit An Artist");
            using (var context = new SongService())
            {
                var artists = context.Artists
                    .OrderBy(a => a.Id)
                    .ToList();
                foreach (var artist in artists)
                {
                    Console.WriteLine($"{artist.Id} - {artist}");
                }
                Console.Write("Enter an ArtistID to edit:");
                int artistId;
                if (!int.TryParse(Console.ReadLine(), out artistId) || artistId <= 0)
                {
                    Console.WriteLine("Invalid ArtistId!!!");
                    Console.ReadLine();
                    return;
                }

                var artistInDb = context.Artists.Find(artistId);
                if (artistInDb == null)
                {
                    Console.WriteLine("Artist does not exist");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"Current Artist Name: {artistInDb.Name}");
                Console.Write("Enter New Name (or ENTER to Keep the same): ");
                var newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    artistInDb.Name = newName;
                }
                Console.WriteLine($"Current Artist Country: {artistInDb.Country}");
                Console.Write("Enter New Country (or ENTER to Keep the same): ");
                var newCountry = Console.ReadLine();
                if (!string.IsNullOrEmpty(newCountry))
                {
                    artistInDb.Country = newCountry;
                }
                Console.WriteLine($"Current Artist Birthday: {artistInDb.Birthday}");
                Console.Write("Enter New Birthday (yyyy-MM-dd) (or ENTER to Keep the same): ");
                var newBirthdayInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(newBirthdayInput))
                {
                    if (DateTime.TryParseExact(newBirthdayInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var newBirthday))
                    {
                        artistInDb.Birthday = newBirthday;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                        Console.ReadLine();
                        return;
                    }
                }

                var originalArtist = context.Artists
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == artistInDb.Id);

                Console.Write($"Are you sure to edit \"{originalArtist!.Name} {originalArtist.Country} {originalArtist.Birthday}\"? (y/n): ");
                var confirm = Console.ReadLine();
                if (confirm?.ToLower() == "y")
                {
                    context.SaveChanges();
                    Console.WriteLine("Artist successfully edited");
                }
                else
                {
                    Console.WriteLine("Operation cancelled by user");
                }
                Console.ReadLine();
                return;
            }
        }

        private static void DeleteArtists()
        {
            Console.Clear();
            Console.WriteLine("Delete An Artist");
            using (var context = new SongService())
            {
                var artists = context.Artists
                    .OrderBy(a => a.Id)
                    .ToList();
                foreach (var artist in artists)
                {
                    Console.WriteLine($"{artist.Id} - {artist}");
                }
                Console.Write("Enter an ArtistID to delete:");
                int artistId;
                if (!int.TryParse(Console.ReadLine(), out artistId) || artistId <= 0)
                {
                    Console.WriteLine("Invalid ArtistId!!!");
                    Console.ReadLine();
                    return;
                }

                var artistInDb = context.Artists.Find(artistId);
                if (artistInDb == null)
                {
                    Console.WriteLine("Artist does not exist");
                    Console.ReadLine();
                    return;
                }
                var hasSongs = context.Songs.Any(s => s.ArtistId == artistInDb.Id);
                if (!hasSongs)
                {
                    Console.Write($"Are you sure to delete \"{artistInDb.Name} {artistInDb.Country} {artistInDb.Birthday} \"? (y/n):");
                var confirm = Console.ReadLine();
                if (confirm?.ToLower() == "y")
                {
                    context.Artists.Remove(artistInDb);
                    context.SaveChanges();
                    Console.WriteLine("Artist successfully removed");
                }
                else
                {
                    Console.WriteLine("Operation cancelled by user");
                }

                }
                else
                {
                    Console.WriteLine("Artist with songs!!! Delete deny");
                }

                Console.ReadLine();
                return;
            }
        }

        private static void AddArtists()
        {
            Console.Clear();
            Console.WriteLine("Adding a New Artist");
            Console.Write("Enter Name:");
            var name = Console.ReadLine();
            Console.Write("Enter Country:");
            var country = Console.ReadLine();
            Console.Write("Enter Year (yyyy-MM-dd):");
            var birthdayInput = Console.ReadLine();

            DateTime birthday;
            if (!DateTime.TryParseExact(birthdayInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
            {
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                Console.ReadLine();
                return;
            }

            using (var context = new SongService())
            {
                bool exist = context.Artists.Any(a => a.Name == name &&
                    a.Country == country && a.Birthday == birthday);

                if (!exist)
                {
                    var artist = new Artist
                    {
                        Name = name ?? string.Empty,
                        Country = country ?? string.Empty,
                        Birthday = birthday
                    };

                    var validationContext = new ValidationContext(artist);
                    var errorMessages = new List<ValidationResult>();

                    bool isValid = Validator.TryValidateObject(artist, validationContext, errorMessages, true);

                    if (isValid)
                    {
                        context.Artists.Add(artist);
                        context.SaveChanges();
                        Console.WriteLine("Artist Succesfully added");
                    }
                    else
                    {
                        foreach (var message in errorMessages)
                        {
                            Console.WriteLine(message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Artist already exist");
                }
            }
            Console.ReadLine();
        }

        private static void ArtistsList()
        {
            Console.Clear();
            Console.WriteLine("List of Artists");
            using (var context = new SongService())
            {
                var artists = context.Artists
                    .OrderBy(a => a.Name)
                    .ThenBy(a => a.Country)
                    .AsNoTracking()
                    .ToList();
                foreach (var artist in artists)
                {
                    Console.WriteLine(artist);
                }
                Console.WriteLine("ENTER to continue");
                Console.ReadLine();
            }
        }

        private static void CreateDb()
        {
            using (var context = new SongService())
            {
                context.Database.EnsureCreated();
            }
            Console.WriteLine("Database created!!!");
        }
    }
}
