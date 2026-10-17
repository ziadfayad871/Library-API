using System.ComponentModel.DataAnnotations;

namespace Library_API.Dtos.BookCopyDTOs
{
    public class BookCopyUpdateDTO
    {
        [Required(ErrorMessage = "Copy number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Copy number must be a positive integer.")]
        public int CopyNo { get; set; }
        [Required(ErrorMessage = "Book ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Book ID must be a positive integer.")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "Shelf location is required.")]
        [StringLength(50, ErrorMessage = "Shelf location cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\-]+$", ErrorMessage = "Shelf location can only contain letters, numbers, spaces, and hyphens.")]
        public string ShelfLocation { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
        [Required(ErrorMessage = "Condition is required.")]
        [StringLength(20, ErrorMessage = "Condition cannot exceed 20 characters.")]
        public string Condition { get; set; } = "Good";
    }
}
