namespace LibraryWeb.Models
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Pages { get; set; }
        public DateTime PublishedYear { get; set; }
    }
}
