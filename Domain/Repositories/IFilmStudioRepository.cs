﻿using Filmstudion.Models;
using Filmstudion.Models.Film;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmstudion.Repositories
{
    public interface IFilmStudioRepository
    {

        void Add<T>(T entity) where T : class;

        Task<bool> SaveChangesAsync();
        Task<IEnumerable<FilmStudio>> GetAllFilmStudiosAsync();
        Task<FilmStudio> GetFilmStudioByIdAsync(int FilmStudioid);
        Task<IEnumerable<FilmCopy>> GetRentedFilmCopiesAsync(int FilmStudioid);
    }
}
