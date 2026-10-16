using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_API.Model
{
    public class BookCopy
    {
        // Not auto-increment because it is a copy of the book
        [Key]
        [Required(ErrorMessage = "Copy number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Copy number must be greater than 0")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Copy number must be a positive integer")]
        // not auto-increment because it is a copy of the book
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CopyNo { get; set; }
        [Required(ErrorMessage = "Book ID is required")]
        public int BookId { get; set; }
        // Navigation property for the related Book entity
        public Book? Book { get; set; }
        // ShelfLocation 
        [Required(ErrorMessage = "Shelf location is required")]
        public string ShelfLocation { get; set; } = string.Empty;   
        public bool IsAvailable { get; set; } = true;
        // Condition 
        public string Condition { get; set; } = "Good";
    }
}
