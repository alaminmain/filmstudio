using AutoMapper;
using Filmstudion.Helpers;
using Filmstudion.Models;
using Filmstudion.Models.User;
using Filmstudion.Resources.FilmCopies;
using Filmstudion.Resources.Public;
using Filmstudion.Resources.Users;
using Filmstudion.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filmstudion.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]

    public class FilmStudiosController : ControllerBase
    {
        //private readonly UserManager<User> userManager;
        //private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IFilmServices _filmService;
        private readonly IFilmCopyService _filmCopyService;

        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IFilmStudioServices _filmStudioService;
        public FilmStudiosController(IConfiguration configuration
            , IFilmServices filmService, IFilmCopyService filmCopyService, IUserService userService, IMapper mapper, IFilmStudioServices filmStudioService)
        {
            //this.userManager = userManager;
            //this.roleManager = roleManager;
            _configuration = configuration;
            _filmService = filmService;
            _userService = userService;
            _mapper = mapper;
            _filmStudioService = filmStudioService;
            _filmCopyService = filmCopyService;


        }
        //A filmstudio must be able to register via the API  (requirement 2)
        //Method: POST
        //See my example implementation below, edit to meet the requirements
        //[HttpPost]
        //[Route("register")]
        //public async Task<IActionResult> Register([FromBody] FilmStudioRegisterModel model)
        //{
        //    var userExists = await userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        //    User user = new User()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        //}

        //You should be able to get all filmstudios by the Web API. (requirement 6)
        //Method: GET
        //Endpoint: /api/filmstudios
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PublicFilmStudioResource[]>> GetAllFilmStudiosAsync()
        {
            try
            {
                var filmStudios = await _filmStudioService.GetAllFilmStudiosAsync();
                var filmStudiosMap = _mapper.Map<PublicFilmStudioResource[]>(filmStudios);
                if (User.Identity.IsAuthenticated)
                {
                    foreach (var rentedfilm in filmStudiosMap)
                    {
                        var filmCopies = await _filmStudioService.GetRentedFilmCopiesAsync(rentedfilm.FilmStudioId);
                        //var available = filmCopies.Where(f => f.RentedOut == false);
                        //film.AvailablefCopies = available.Count();
                        rentedfilm.RentedFilmcopies = filmCopies;
                    }
                }
                return Ok(filmStudiosMap);


            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        //A filmstudio must be able to register via the API  (requirement 2)
        //Method: POST
        //See my example implementation below, edit to meet the requirements
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterFilmStudioModel model)
        {
            try
            {
                var filmStudio = _mapper.Map<RegisterFilmStudioModel, FilmStudio>(model);
                var allFilmStudios = await _filmStudioService.GetAllFilmStudiosAsync();
                int id = allFilmStudios.Count() + 1;
                filmStudio.FilmStudioId = id;

                _filmStudioService.Add(filmStudio);

                var user = new User(); //_mapper.Map<User>(model);
                if (await _filmStudioService.SaveChangesAsync())

                {
                    user.Email = model.Email;
                    user.FilmStudioId = filmStudio.FilmStudioId;
                    user.IsAdmin = false;
                    // create user
                    _userService.Create(user, model.Password);
                }
                return Ok(new
                {
                    FilmStudioId = filmStudio.FilmStudioId,
                    Name = filmStudio.Name,
                    City = filmStudio.City
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        //Via the Web API, it should be possible to retrieve information about an individual filmstudio(requirment 7)
        //Method: GET
        //Endpoint: /api/filmstudio/{id}
        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<ActionResult<PublicFilmStudioResource>> GetFilmStudioByIdAsync(int Id)
        {
            try
            {
                var filmStudios = await _filmStudioService.GetFilmStudioByIdAsync(Id);
                var filmStudiosMap = _mapper.Map<PublicFilmStudioResource>(filmStudios);
                if (User.Identity.IsAuthenticated)
                {
                    var userName = User.Identity.Name;
                    var userId = int.Parse(userName);
                    var user = _userService.GetById(userId);

                    if (user.IsAdmin)
                    {

                        var filmCopies = await _filmStudioService.GetRentedFilmCopiesAsync(filmStudios.FilmStudioId);

                        filmStudios.RentedFilmCopies = filmCopies.ToList();
                    }

                }
                return Ok(filmStudiosMap);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        //An authenticated filmstudio must be able to get which films this studio has currently borrowed via the Web API (requirment 14)
        //Method: GET
        //Endpoint: /api/filmstudio/rentals
        //Comment: Don't know if this should be in this controller, or add this in separate Controller or how to implement (you can decide)
        [HttpGet]
        [Route("/api/FilmStudios/rentals")]
        public async Task<ActionResult<FilmCopyResource[]>> GetAllRentedFilmCopiesAsync()
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = int.Parse(userName);
                var user = _userService.GetById(userId);

                if (user.IsAdmin)
                {
                    var rented = await _filmCopyService.GetAllRentedFilmCopiesAsync();
                    var filmcopies = _mapper.Map<FilmCopyResource[]>(rented);

                    foreach (var copie in filmcopies)
                    {
                        var film = await _filmService.GetFilmByIdAsync(copie.FilmId);
                        copie.FilmName = film.Name;
                    }
                    return filmcopies;
                }

                else if (!user.IsAdmin && user.FilmStudioId != 0)
                {
                    var filmStudioId = user.FilmStudioId;

                    var rented = await _filmStudioService.GetRentedFilmCopiesAsync(filmStudioId);
                    var filmcopies = _mapper.Map<FilmCopyResource[]>(rented);

                    if (rented == null)
                    {
                        return NotFound($"You have no rented movies.");
                    }

                    foreach (var copie in filmcopies)
                    {
                        var movie = await _filmService.GetFilmByIdAsync(copie.FilmId);
                        copie.FilmName = movie.Name;
                    }

                    return filmcopies;
                }

                return Unauthorized("You are not allowed this action.");
            }

            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }





        //Example how to set Authorize on Controller to work with JWT: [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    }
}
