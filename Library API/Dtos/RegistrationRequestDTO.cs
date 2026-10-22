using System.ComponentModel.DataAnnotations;

namespace Library_API.Dtos
{
    public class RegistrationRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Role { get; set; } = "Admin"; // Default role is "Admin"
    }
}
