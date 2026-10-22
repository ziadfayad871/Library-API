namespace Library_API.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; } = "Admin"; // Default role is "Admin"
    }
}
