using System.ComponentModel.DataAnnotations;

namespace Filmstudion.Authentication
{
    public class FilmStudioRegisterModel
    {
        //Example FilmStudioRegisterModel (change below to meet requirement)
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "ChairpersonName is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool IsAdmin = false;
    }
}
