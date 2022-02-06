using AutoMapper;
using Filmstudion.Models.Film;
using Filmstudion.Resources.Movies;
using Filmstudion.Resources.Public;
using Filmstudion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filmstudion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {

        private readonly IFilmServices _filmService;
        private readonly IFilmCopyService _filmCopyService;
        //private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public FilmsController(IFilmServices filmService, IFilmCopyService filmCopyService, IMapper mapper)
        {
            _filmService = filmService;
            _filmCopyService = filmCopyService;
            //_userService = userService;
            _mapper = mapper;
        }

        //It should be possible to add new films via the Web API if you are authenticated as an administrator. (requirement 5)
        //Method: PUT
        //Endpoint: /api/films
        [HttpPut]
        public async Task<ActionResult<FilmResource>> PostNewFilmAsync([FromBody] CreateUpdateFilmResource resource)
        {
            try
            {
                //var userName = User.Identity.Name;
                //var userId = int.Parse(userName);
                //var user = _userService.GetById(userId);

                //if (user.IsAdmin == false) { return Unauthorized("Only Admins are allowed this action."); }

                var film = _mapper.Map<CreateUpdateFilmResource, Film>(resource);
                _filmService.Add(film);

                if (await _filmService.SaveChangesAsync())
                {
                    var newCopies = film.NumberOfCopies;
                    var id = film.FilmId;
                    _filmCopyService.CreateCopies(newCopies, id);

                    var result = _mapper.Map<Film, FilmResource>(film);
                    result.AvailableFilmcopies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(id);

                    return Created("Successfully Created", result);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to save Movie.");
            }

            return BadRequest();
        }



        //You should be able to get all films by the Web API. (requirement 8)
        //Method: GET
        //Endpoint: /api/films
        [HttpGet]
        public async Task<ActionResult<FilmResource[]>> GetAllFilmsAsync()
        {
            try
            {
                var films = await _filmService.GetAllFilmsAsync();
                var moviesFilmCopies = _mapper.Map<FilmResource[]>(films);
                if (User.Identity.IsAuthenticated)
                {
                    foreach (var film in moviesFilmCopies)
                    {
                        var filmCopies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(film.FilmId);
                        var available = filmCopies.Where(f => f.RentedOut == false);
                        film.AvailablefCopies = available.Count();
                        film.AvailableFilmcopies = available;
                    }
                }

                return moviesFilmCopies;
            }

            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        //Via the Web API, it must be possible to retrieve the information about an individual film. (requirement 9)
        //Method: GET
        //Endpoint: /api/films/{id}

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PublicFilmResource>> GetFilmByIdAsync(int id)
        {
            try
            {
                var film = await _filmService.GetFilmByIdAsync(id);
                if (film == null)
                {
                    return NotFound($"Could not find movie with id of {id}");
                }
                return _mapper.Map<Film, PublicFilmResource>(film);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
        //It should be possible to change the information about a film via the Web API if you are authenticated as an administrator (requirement 10)
        //Method PATCH
        //Endpoint: /api/films/{id}


        //An authenticated administrator should be able to change the number of available copies that can be borrowed from each film. (requirement 11)
        //Method: UPDATE
        //Endpoint: /api/films/{id}


        //An authenticated filmstudio should be able to borrow a copy of a film via the Web API if copies are available. (requirement 12)
        //Method: POST
        //Endpoint: /api/films/rent?Id={id}&studioid={studioid}


        //An authenticated filmstudio must be able to return a borrowed copy of a film via the Web API.
        //Method: POST
        //Endpoint: /api/films/return?Id={id}&studioid={studioid}




        //Example how to set Authorize on Controller to work with JWT: [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    }
}
