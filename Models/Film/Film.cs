using System;
using System.Collections.Generic;

namespace Filmstudion.Models.Film
{
    public class Film
    {
        public int FilmId { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Country { get; set; }
        public string Director { get; set; }
        public int NumberOfCopies { get; set; }
        public List<Models.Film.FilmCopy> FilmCopies { get; set; }
    }
}
