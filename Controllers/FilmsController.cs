using Microsoft.AspNetCore.Mvc;

namespace Filmstudion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {

        //It should be possible to add new films via the Web API if you are authenticated as an administrator. (requirement 5)
        //Method: PUT
        //Endpoint: /api/films


        //You should be able to get all films by the Web API. (requirement 8)
        //Method: GET
        //Endpoint: /api/films


        //Via the Web API, it must be possible to retrieve the information about an individual film. (requirement 9)
        //Method: GET
        //Endpoint: /api/films/{id}


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
