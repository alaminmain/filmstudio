using System.ComponentModel.DataAnnotations;

namespace Filmstudion.Authentication
{
    public class AdminRegisterModel
    {
        //FilmStudioRegisterModel (change below to meet requirement)

        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public int FilmStudioId = 0;
        public bool IsAdmin = true;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
