
using AutoMapper;
using Filmstudion.Helpers;
using Filmstudion.Models.User;
using Filmstudion.Resources.Users;
using Filmstudion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        /// <summary>
        /// 4: Both filmstudios and administrators must be able to authenticate
        ///        themselves via the API.
        ///       - This call should use the following method: POST
        ///        Authentication should take place on a call to the following endpoint:
        ///api/users/authenticate
        ///        - The BODY in this call should be able to contain a json-stringed object that
        ///        satisfies the IUserAuthenticate interface.
        ///This object contains the necessary data to correctly authenticate a user.
        ///- If this authentication is approved, a JSON object corresponding to the
        ///interface IUser, all except ‘password’ property, must be returned.
        ///It is important to hide sensitive information.When returning prohibited data, a
        ///point deduction is made
        ///- If the username and password used in the authentication belong to a
        ///registered admin, the Role property must be a string with the word "admin".
        ///uppercase or lowercase letters on this string do not matter.
        ///- If the username and password used in the authentication belong to a
        ///registered filmstudio, the Role property in the object must be a string with the
        ///word “filmstudio”. The filmStudioId property must also be present and the
        ///filmStudio property must contain an object that corresponds to the IFilmStudio
        ///interface and contains at least data in the FilmStudioId, Name and City
        ///properties.
        ///- Further authentication in the application must take place via the header
        ///"Authentication" when calling the API.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                RoleName = user.RoleName,
                Token = tokenString
            });
        }


        //  An  administrator  should  be  able  to  register  via  the  API,  an  administrator  is not  a  filmstudio. (requirment 3)
        [AllowAnonymous]
        [HttpPost("register")]
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
