using System.ComponentModel.DataAnnotations;

namespace LibraryWeb.Models.AuthDTO
{
    public class RegistrationRequestDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
