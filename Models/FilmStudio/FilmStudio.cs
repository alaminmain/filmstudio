using Filmstudion.Models.Film;
using System.Collections.Generic;

namespace Filmstudion.Models
{
    public class FilmStudio
    {
        public int FilmStudioId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public List<FilmCopy> RentedFilmCopies { get; set; }
    }
}
