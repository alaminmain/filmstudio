﻿
using AutoMapper;
using Filmstudion.Helpers;
using Filmstudion.Models;
using Filmstudion.Models.User;
using Filmstudion.Resources.Users;
using Filmstudion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Filmstudion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFilmStudioServices _filmStudioService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;


        public UsersController(
            IUserService userService, IFilmStudioServices filmStudioService,
            IMapper mapper,
            IConfiguration config)
        {
            _userService = userService;
            _filmStudioService = filmStudioService;
            _mapper = mapper;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user.Id,
                Username = user.Email,
                Token = tokenString
            });
        }

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

                var user = _mapper.Map<User>(model);
                if (await _filmStudioService.SaveChangesAsync())
                {
                    user.Email = model.Email;
                    user.FilmStudioId = filmStudio.FilmStudioId;
                    user.IsAdmin = false;
                    // create user
                    _userService.Create(user, model.Password);
                }
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register/admin")]
        public IActionResult RegisterAdmin([FromBody] RegisterAdminUserModel model)
        {
            var user = _mapper.Map<User>(model);

            try
            {
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
