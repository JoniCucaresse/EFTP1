using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace EFTP1.Entities
{
    [Table("Artists")]
    [Index(nameof(Artist.Name), Name = "IX_Authors_Name", IsUnique = true)]
    public class Artist
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} must be between {2} and {1} characteres", MinimumLength = 3)]
        public string Name { get; set; } = null!;
        public string Country { get; set; }
        public DateTime Birthday { get; set; }

        public ICollection<Song>? Songs { get; set; }

        public override string ToString()
        {
            return $"{Name.ToUpper()}, {Country}, {Birthday.Year}";
        }
    }
}
