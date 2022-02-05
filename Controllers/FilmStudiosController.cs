using Filmstudion.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Filmstudion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmStudiosController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public FilmStudiosController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }
        //A filmstudio must be able to register via the API  (requirement 2)
        //Method: POST
        //See my example implementation below, edit to meet the requirements
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] FilmStudioRegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        //You should be able to get all filmstudios by the Web API. (requirement 6)
        //Method: GET
        //Endpoint: /api/filmstudios


        //Via the Web API, it should be possible to retrieve information about an individual filmstudio
        //Method: GET
        //Endpoint: /api/filmstudio/{id}


        //An authenticated filmstudio must be able to get which films this studio has currently borrowed via the Web API (requirment 14)
        //Method: GET
        //Endpoint: /api/mystudio/rentals
        //Comment: Don't know if this should be in this controller, or add this in separate Controller or how to implement (you can decide)






        //Example how to set Authorize on Controller to work with JWT: [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    }
}
