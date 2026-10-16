using System.ComponentModel.DataAnnotations;

namespace Library_API.Dtos
{
    public class BookCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [MinLength(10)]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Pages { get; set; }
        [Required]
        public DateTime PublishedYear { get; set; }
           }
}
