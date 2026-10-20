using System.ComponentModel.DataAnnotations;
using LibraryWeb.Validation;

namespace LibraryWeb.Models
{
    public class BookUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Book name is required.")]
        [MinLength(1, ErrorMessage = "Book name cannot be empty.")]
        [MaxLength(200, ErrorMessage = "Book name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Author name is required.")]
        [MinLength(1, ErrorMessage = "Author name cannot be empty.")]
        [MaxLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string Author { get; set; } = string.Empty;
        [MinLength(10, ErrorMessage = "Description cannot be less than 10 characters.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Pages is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pages must be greater than 0.")]
        public int Pages { get; set; }
        [Required(ErrorMessage = "Published year is required.")]
        [NotInFuture(ErrorMessage = "Published year cannot be in the future.")]
        public DateTime PublishedYear { get; set; }

    }
}
