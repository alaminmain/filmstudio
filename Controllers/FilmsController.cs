using AutoMapper;
using Filmstudion.Models.Film;
using Filmstudion.Resources.FilmCopies;
using Filmstudion.Resources.Movies;
using Filmstudion.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filmstudion.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]

    public class FilmsController : ControllerBase
    {

        private readonly IFilmServices _filmService;
        private readonly IFilmCopyService _filmCopyService;

        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IFilmStudioServices _filmStudioService;

        public FilmsController(IFilmServices filmService, IFilmCopyService filmCopyService, IUserService userService, IMapper mapper, IFilmStudioServices filmStudioService)
        {
            _filmService = filmService;
            _filmCopyService = filmCopyService;
            _userService = userService;
            _mapper = mapper;
            _filmStudioService = filmStudioService;
        }

        /// <summary>
        /// This Method will add new films via the Web API if you are authenticated as an administrator. (requirement 5)
        /// </summary>
        /// 
        /// <param name="resource">Film Resource</param>
        /// <returns>Film Data with Message</returns>

        //It should be possible to add new films via the Web API if you are authenticated as an administrator. (requirement 5)
        //Method: PUT
        //Endpoint: /api/films
        [HttpPut]
        public async Task<ActionResult<FilmResource>> PostNewFilmAsync([FromBody] CreateUpdateFilmResource resource)
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = int.Parse(userName);
                var user = _userService.GetById(userId);

                if (user.IsAdmin == false) { return Unauthorized("Only Admins are allowed this action."); }

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
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<FilmResource[]>> GetAllFilmsAsync()
        {
            try
            {
                var films = await _filmService.GetAllFilmsAsync();
                var Films = _mapper.Map<FilmResource[]>(films);
                if (User.Identity.IsAuthenticated)
                {
                    foreach (var film in Films)
                    {
                        var filmCopies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(film.FilmId);
                        var available = filmCopies.Where(f => f.RentedOut == false);
                        film.AvailablefCopies = available.Count();
                        film.AvailableFilmcopies = available;
                    }
                }

                return Films;
            }

            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        //Via the Web API, it must be possible to retrieve the information about an individual film. (requirement 9)
        //Method: GET
        //Endpoint: /api/films/{id}

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmResource>> GetFilmByIdAsync(int id)
        {
            try
            {
                var film = await _filmService.GetFilmByIdAsync(id);
                var films = _mapper.Map<FilmResource>(film);
                if (User.Identity.IsAuthenticated)
                {


                    var filmCopies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(film.FilmId);
                    var available = filmCopies.Where(f => f.RentedOut == false);
                    films.AvailablefCopies = available.Count();
                    films.AvailableFilmcopies = available;

                }

                return films;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
        //It should be possible to change the information about a film via the Web API if you are authenticated as an administrator (requirement 10)
        //Method PATCH
        //Endpoint: /api/films/{id}
        [HttpPatch("{filmId}")]
        public async Task<ActionResult<FilmResource>> UpdateFilmInformationAsync(int filmId, [FromBody] CreateUpdateFilmResource resource)
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = int.Parse(userName);
                var user = _userService.GetById(userId);

                if (!user.IsAdmin) { return Unauthorized("Only Admins are allowed this action."); }

                var oldFilm = await _filmService.GetFilmByIdAsync(filmId);

                if (oldFilm == null) { return NotFound($"Could not find film with id of {filmId}"); }

                if (oldFilm.NumberOfCopies != resource.NumberOfCopies)
                {
                    if (oldFilm.NumberOfCopies < resource.NumberOfCopies)
                    {
                        int oldCopies = oldFilm.NumberOfCopies;
                        int newCopies = resource.NumberOfCopies;
                        _filmCopyService.CreateCopies(oldCopies, newCopies, filmId);
                    }

                    else if (oldFilm.NumberOfCopies > resource.NumberOfCopies)
                    {
                        var amount = resource.NumberOfCopies;
                        var copies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(filmId);
                        _filmCopyService.DeleteCopies(amount, copies);
                    }
                }
                var newFilm = _mapper.Map(resource, oldFilm);
                var result = _mapper.Map<Film, FilmResource>(newFilm);
                result.AvailableFilmcopies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(filmId);

                if (await _filmService.SaveChangesAsync()) { return Ok(result); }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }


        //An authenticated administrator should be able to change the number of available copies that can be borrowed from each film. (requirement 11)
        //Method: UPDATE
        //Endpoint: /api/films/{id}
        [HttpPut("{filmId}")]
        public async Task<ActionResult<FilmResource>> UpdateFilmInfoCopiesAsync(int filmId, [FromBody] CreateUpdateFilmResource resource)
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = int.Parse(userName);
                var user = _userService.GetById(userId);

                if (!user.IsAdmin) { return Unauthorized("Only Admins are allowed this action."); }

                var oldFilm = await _filmService.GetFilmByIdAsync(filmId);

                if (oldFilm == null) { return NotFound($"Could not find film with id of {filmId}"); }

                if (oldFilm.NumberOfCopies != resource.NumberOfCopies)
                {
                    if (oldFilm.NumberOfCopies < resource.NumberOfCopies)
                    {
                        int oldCopies = oldFilm.NumberOfCopies;
                        int newCopies = resource.NumberOfCopies;
                        _filmCopyService.CreateCopies(oldCopies, newCopies, filmId);
                    }

                    else if (oldFilm.NumberOfCopies > resource.NumberOfCopies)
                    {
                        var amount = resource.NumberOfCopies;
                        var copies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(filmId);
                        _filmCopyService.DeleteCopies(amount, copies);
                    }
                }
                var newFilm = _mapper.Map(resource, oldFilm);
                var result = _mapper.Map<Film, FilmResource>(newFilm);
                result.AvailableFilmcopies = await _filmCopyService.GetAllFilmCopiesByFilmIdAsync(filmId);

                if (await _filmService.SaveChangesAsync()) { return Ok(result); }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        //An authenticated filmstudio should be able to borrow a copy of a film via the Web API if copies are available. (requirement 12)
        //Method: POST
        //Endpoint: /api/films/rent?Id={id}&studioid={studioid}
        [HttpPost]
        [Route("/api/Films/rent")]
        public async Task<ActionResult<FilmCopyResource>> RentFilmCopyAsync(int id, int studioid) //id mean filmId
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = int.Parse(userName);
                var user = _userService.GetById(userId);

                if (user.IsAdmin) { return Unauthorized("Only FilmStudios are allowed this action."); }

                var filmCopies = await _filmStudioService.GetRentedFilmCopiesAsync(studioid);
                var rentedFilmCopy = filmCopies.FirstOrDefault(f => f.FilmId == id);

                if (rentedFilmCopy != null) { return StatusCode(StatusCodes.Status403Forbidden, $"You already rented a copy of the film with FilmId of {id}. Please return before continue."); }

                var filmCopy = await _filmCopyService.GetAvaibleFilmCopyByFilmIdAsync(id);

                if (filmCopy == null) { return StatusCode(StatusCodes.Status409Conflict, $"Could not find avaible filmcopy with FilmId of {id}"); }

                filmCopy.RentedOut = true;
                filmCopy.FilmStudioId = studioid;

                var resource = _mapper.Map<FilmCopyResource>(filmCopy);
                var film = await _filmService.GetFilmByIdAsync(resource.FilmId);
                resource.FilmName = film.Name;

                if (await _filmCopyService.SaveChangesAsync()) { return Created("", resource); }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        //An authenticated filmstudio must be able to return a borrowed copy of a film via the Web API. (requirement 13)
        //Method: POST
        //Endpoint: /api/films/return?Id={id}&studioid={studioid}

        [HttpPut]
        [Route("/api/Films/return")]
        public async Task<ActionResult<FilmCopyResource>> ReturnFilmCopyAsync(int id, int studioid)
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = int.Parse(userName);
                var user = _userService.GetById(userId);

                if (user.IsAdmin) { return Unauthorized("Only FilmStudios are allowed this action."); }

                var filmCopies = await _filmStudioService.GetRentedFilmCopiesAsync(studioid);
                var filmCopy = filmCopies.FirstOrDefault(f => f.FilmId == id);

                if (filmCopy == null) { return StatusCode(StatusCodes.Status409Conflict, $"Could not find rented filmcopy with filmId of {id}"); }

                filmCopy.RentedOut = false;
                filmCopy.FilmStudioId = 0;

                var resource = _mapper.Map<FilmCopyResource>(filmCopy);
                var film = await _filmService.GetFilmByIdAsync(resource.FilmId);
                resource.FilmName = film.Name;

                if (await _filmCopyService.SaveChangesAsync()) { return Created("", resource); }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }



        //Example how to set Authorize on Controller to work with JWT: [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]

    }
}
