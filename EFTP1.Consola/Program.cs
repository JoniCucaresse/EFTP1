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
                        EditSongs();
                        break;
                    case "r":
                        return;
                    default:
                        break;
                }

            } while (true);
        }

        private static void EditSongs()
        {
            Console.Clear();
            Console.WriteLine("Editing Songs");
            Console.WriteLine("list Of Songs to Edit");
            using (var context = new SongService())
            {
                var songs = context.Songs.OrderBy(s => s.Id)
                    .Select(s => new
                    {
                        s.Id,
                        s.Title
                    }).ToList();
                foreach (var item in songs)
                {
                    Console.WriteLine($"{item.Id}-{item.Title}");
                }
                Console.Write("Enter SongID to edit (0 to Escape):");
                if (!int.TryParse(Console.ReadLine(), out int songId) || songId < 0)
                {
                    Console.WriteLine("Invalid SongID... ");
                    Console.ReadLine();
                    return;
                }
                if (songId == 0)
                {
                    Console.WriteLine("Cancelled by user");
                    Console.ReadLine();
                    return;
                }

                var songInDb = context.Songs.Include(s => s.Artist)
                    .FirstOrDefault(s => s.Id == songId);
                if (songInDb == null)
                {
                    Console.WriteLine("Song does not exist...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"Current Song Title: {songInDb.Title}");
                Console.Write("Enter New Title (or ENTER to Keep the same):");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrEmpty(newTitle))
                {
                    songInDb.Title = newTitle;
                }

                Console.WriteLine($"Current Song Duration: {songInDb.Duration}");
                Console.Write("Enter Song Duration in minutes (or ENTER to Keep the same):");
                var newDuration = Console.ReadLine();
                if (!string.IsNullOrEmpty(newDuration))
                {
                    if (!int.TryParse(newDuration, out int songDuration) || songDuration <= 0)
                    {
                        Console.WriteLine("You enter an invalid song duration");
                        Console.ReadLine();
                        return;
                    }
                    songInDb.Duration = songDuration;
                }

                Console.WriteLine($"Current Song Gender: {songInDb.Gender}");
                Console.Write("Enter New Song Gender (or ENTER to Keep the same):");
                var newGender = Console.ReadLine();
                if (!string.IsNullOrEmpty(newGender))
                {
                    songInDb.Gender = newGender;
                }
                Console.WriteLine($"Current Song Artist:{songInDb.Artist}");
                Console.WriteLine("Available Artists");
                var artists = context.Artists
                    .OrderBy(a => a.Id)
                    .ToList();
                foreach (var artist in artists)
                {
                    Console.WriteLine($"{artist.Id}-{artist}");
                }
                Console.Write("Enter AartistID (or ENTER to Keep the same or 0 New Artist):");
                var newArtist = Console.ReadLine();
                if (!string.IsNullOrEmpty(newArtist))
                {
                    if (!int.TryParse(newArtist, out int artistId) || artistId < 0)
                    {
                        Console.WriteLine("You enter an invalid ArtistID");
                        Console.ReadLine();
                        return;
                    }
                    if (artistId > 0)
                    {
                        var existArtist = context.Artists.Any(a => a.Id == artistId);
                        if (!existArtist)
                        {
                            Console.WriteLine("ArtistID not found");
                            Console.ReadLine();
                            return;
                        }
                        songInDb.ArtistId = artistId;

                    }
                    else
                    {
                        //Entering new artist
                        Console.WriteLine("Adding a New Artist");

                        Console.Write("Enter Name:");
                        var name = Console.ReadLine() ?? string.Empty;

                        Console.Write("Enter Country:");
                        var country = Console.ReadLine() ?? string.Empty;

                        Console.Write("Enter Foundation Year:");
                        int foundationYear;
                        if (!int.TryParse(Console.ReadLine(), out foundationYear) || foundationYear <= 0)
                        {
                            Console.WriteLine("Invalid Foundation Year!!!");
                            Console.ReadLine();
                            return;
                        }
                        var existingArtist = context.Artists.FirstOrDefault(
                                a => a.Name.ToLower() == name!.ToLower());

                        if (existingArtist is not null)
                        {
                            Console.WriteLine("You have entered an existing artist!!!");
                            Console.WriteLine("Assigning his ArtistID");

                            songInDb.ArtistId = existingArtist.Id;
                        }
                        else
                        {
                            Artist Artist = new Artist
                            {
                                Name = name,
                                Country = country,
                                FoundationYear = foundationYear
                            };
                            
                            var artistValidator = new ArtistValidator();
                            var validatioResult = artistValidator.Validate(Artist);

                            if (!validatioResult.IsValid)
                            {
                                Console.WriteLine("Artist validation failed:");
                                foreach (var error in validatioResult.Errors)
                                {
                                    Console.WriteLine($"- {error.ErrorMessage}");
                                }
                                Console.ReadLine();
                                return;
                            }

                            context.Artists.Add(Artist);
                            context.SaveChanges();
                            songInDb.ArtistId = Artist.Id;

                        }
                    }

                }
                var songsValidator = new SongsValidator();
                var validationResult = songsValidator.Validate(songInDb);

                if (!validationResult.IsValid)
                {
                    Console.WriteLine("Validation failed:");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                    Console.ReadLine();
                    return;
                }

                var originalSong = context.Songs
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == songInDb.Id);

                Console.Write($"Are you sure to edit \"{originalSong!.Title}\"? (y/n):");
                var confirm = Console.ReadLine();
                try
                {
                    if (confirm?.ToLower() == "y")
                    {
                        context.SaveChanges();
                        Console.WriteLine("Song successfully edited");
                    }
                    else
                    {
                        Console.WriteLine("Operation cancelled by user");
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
                return;


            }
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
            Console.WriteLine("Adding New Song");
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
            Console.WriteLine("List of Artist to Select");
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
                    Console.WriteLine("Invalid AuthorID....");
                    Console.ReadLine();
                    return;
                }
                if (artistId > 0)
                {
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
                        var existingSong = context.Songs.FirstOrDefault(b => b.Title.ToLower() == title!.ToLower() &&
                            b.ArtistId == artistId);

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

                }
                else
                {
                    //Entering new artist
                    Console.WriteLine("Adding a New Artist");

                    Console.Write("Enter Name:");
                    var name = Console.ReadLine() ?? string.Empty;

                    Console.Write("Enter Country:");
                    var country = Console.ReadLine() ?? string.Empty;

                    Console.Write("Enter Foundation Year:");
                    int foundationYear;
                    if (!int.TryParse(Console.ReadLine(), out foundationYear) || foundationYear <= 0)
                    {
                        Console.WriteLine("Invalid Foundation Year!!!");
                        Console.ReadLine();
                        return;
                    }

                    var existingArtist = context.Artists.FirstOrDefault(
                            a => a.Name.ToLower() == name!.ToLower());

                    if (existingArtist is not null)
                    {
                        Console.WriteLine("You have entered an existing artist!!!");
                        Console.WriteLine("Assigning his ArtistID");

                        var newSong = new Song
                        {
                            Title = title ?? string.Empty,
                            Duration = duration,
                            Gender = gender ?? string.Empty,
                            ArtistId = existingArtist.Id
                        };

                        var songsValidator = new SongsValidator();
                        var validationResult = songsValidator.Validate(newSong);

                        if (validationResult.IsValid)
                        {
                            var existingSong = context.Songs.FirstOrDefault(b => b.Title.ToLower() == title!.ToLower() &&
                                b.ArtistId == artistId);

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


                    }
                    else
                    {
                        Artist newArtist = new Artist
                        {
                            Name = name ?? string.Empty,
                            Country = country ?? string.Empty,
                            FoundationYear = foundationYear
                        };

                        var artistValidator = new ArtistValidator();
                        var validationresult = artistValidator.Validate(newArtist);

                        

                        if (validationresult.IsValid)
                        {
                            var newSong = new Song
                            {
                                Title = title ?? string.Empty,
                                Duration = duration,
                                Gender = gender ?? string.Empty,
                                Artist = newArtist
                            };

                            var songsValidator = new SongsValidator();
                            var validationResult = songsValidator.Validate(newSong);

                            if (validationResult.IsValid)
                            {

                                var existingSong = context.Songs.FirstOrDefault(b => b.Title.ToLower() == title!.ToLower() &&
                                    b.ArtistId == artistId);

                                if (existingSong is null)
                                {
                                    context.Add(newSong);
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


                        }
                        else
                        {
                            foreach (var message in validationresult.Errors)
                            {
                                Console.WriteLine(message);
                            }
                        }

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
                Console.WriteLine("5 - List of Artists With Songs");
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
                    case "5":
                        ListOfArtistsWithSongs();
                        break;
                    case "r":
                        return;
                    default:
                        break;
                }

            } while (true);

        }

        private static void ListOfArtistsWithSongs()
        {
            Console.Clear();
            Console.WriteLine("List of Artists With Songs");
            using (var context = new SongService())
            {
                var artistGroups = context.Songs
                    .GroupBy(a => a.ArtistId).ToList();
                foreach (var group in artistGroups)
                {
                    Console.WriteLine($"ArtistID: {group.Key}");
                    var artist = context.Artists.Find(group.Key);
                    Console.WriteLine($"Artist: {artist}");
                    foreach (var song in group)
                    {
                        Console.WriteLine($"    {song.Title}");
                    }
                    Console.WriteLine($"Songs Count: {group.Count()}");
                    //Console.WriteLine($"Average Page Count: {group.Average(b => b.Pages)}");
                }
            }
            Console.ReadLine();
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
                    var existingArtist = context.Artists
                        .FirstOrDefault(a => a.Name.ToLower() == newName.ToLower() && a.Id != artistInDb.Id);

                    if (existingArtist != null)
                    {
                        Console.WriteLine($"Error: An artist with the name \"{newName}\" already exists.");
                        Console.ReadLine();
                        return;
                    }

                    artistInDb.Name = newName;
                }

                Console.WriteLine($"Current Artist Country: {artistInDb.Country}");
                Console.Write("Enter New Country (or ENTER to Keep the same): ");
                var newCountry = Console.ReadLine();
                if (!string.IsNullOrEmpty(newCountry))
                {
                    artistInDb.Country = newCountry;
                }

                Console.WriteLine($"Current Artist Foundation Year: {artistInDb.FoundationYear}");
                Console.Write("Enter New FoundationYear (or ENTER to Keep the same): ");
                var newFoundationYearInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(newFoundationYearInput))
                {
                    if (int.TryParse(newFoundationYearInput, out int newFoundationYear))
                    {
                        artistInDb.FoundationYear = newFoundationYear;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Foundation Year! Only numbers are allowed.");
                        Console.ReadLine();
                        return;
                    }
                }

                var artistValidator = new ArtistValidator();
                var validationResult = artistValidator.Validate(artistInDb);

                if (validationResult.IsValid)
                {
                    Console.Write($"Are you sure to edit \"{artistInDb.Name} {artistInDb.Country} {artistInDb.FoundationYear}\"? (y/n): ");
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
                }
                else
                {
                    Console.WriteLine("Validation failed:");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
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
                    Console.Write($"Are you sure to delete \"{artistInDb.Name} {artistInDb.Country} {artistInDb.FoundationYear} \"? (y/n):");
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
                    context.Entry(artistInDb).Collection(a => a.Songs!).Load();
                    foreach (var song in artistInDb.Songs!)
                    {
                        Console.WriteLine($"{song.Title}");
                    }
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
            var name = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter Country:");
            var country = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter Foundation Year:");
            int foundationYear;
            if (!int.TryParse(Console.ReadLine(), out foundationYear) || foundationYear <= 0)
            {
                Console.WriteLine("Invalid Foundation Year!!!");
                Console.ReadLine();
                return;
            }

            var artist = new Artist
            {
                Name = name,
                Country = country,
                FoundationYear = foundationYear
            };

            var artistValidator = new ArtistValidator();
            var validationResult = artistValidator.Validate(artist);

            if (validationResult.IsValid)
            {
                using (var context = new SongService())
                {
                    bool exist = context.Artists.Any(a => a.Name == name);

                    if (!exist)
                    {
                        context.Artists.Add(artist);
                        context.SaveChanges();
                        Console.WriteLine("Artist Successfully added");
                    }
                    else
                    {
                        Console.WriteLine("Artist already exists");
                    }
                }
            }
            else
            {
                Console.WriteLine("Validation failed:");
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
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
