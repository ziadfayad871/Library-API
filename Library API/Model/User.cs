using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Admin"; // Default role is "Admin"
    }
}
