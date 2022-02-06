﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Filmstudion.Resources.Movies
{
    public class CreateUpdateFilmResource
    {
        public int FilmId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        //[Range(1900, 2021)]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        [Range(1, 9)]
        public int AmountOfCopies { get; set; }
    }
}
