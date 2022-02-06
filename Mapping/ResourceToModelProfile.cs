using AutoMapper;
using Filmstudion.Authentication;
using Filmstudion.Models;
using Filmstudion.Models.Film;
using Filmstudion.Resources.FilmCopies;
using Filmstudion.Resources.Movies;
using Filmstudion.Resources.Users;

namespace Filmstudion.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<RegisterAdminUserModel, User>();

            CreateMap<RegisterFilmStudioModel, FilmStudio>();
            CreateMap<RegisterFilmStudioModel, User>();

            CreateMap<CreateUpdateFilmResource, Film>();

            CreateMap<FilmCopyResource, FilmCopy>();
        }
    }
}
