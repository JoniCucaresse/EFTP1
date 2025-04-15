using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTP1.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int Duration { get; set; } 
        public string Gender { get; set; }

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public override string ToString()
        {
            return $"Title:{Title} - Duration(minutes):{Duration}";
        }
    }
}
