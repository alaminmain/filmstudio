using Filmstudion.Models.Film;
using System;
using System.Collections.Generic;


namespace Filmstudion.Resources.Movies
{
    public class FilmResource
    {
        public int FilmId { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseYear { get; set; }
        public string Country { get; set; }
        public string Director { get; set; }
        public int AvailablefCopies { get; set; }
        public IEnumerable<FilmCopy> AvailableFilmcopies { get; set; }
    }
}
