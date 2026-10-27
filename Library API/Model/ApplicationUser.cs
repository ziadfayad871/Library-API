using Microsoft.AspNetCore.Identity;

namespace Library_API.Model
{
    public class ApplicationUser : IdentityUser
    {
        // Add properties for the ApplicationUser class
        public string Name { get; set; }
    }
}
