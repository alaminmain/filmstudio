using Microsoft.AspNetCore.Identity;

namespace Filmstudion.Authentication
{
    public class User : IdentityUser
    {
        //example User (change below to meet requirements)
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public int FilmStudioId { get; set; }
        //public FilmStudio FilmStudio { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
