using Library_API.Dtos.BookCopyDTOs;
using Library_API.Model;

namespace Library_API.Repository.IRepository
{
    public interface IBookCopyRepository
    {
        public Task<BookCopyDTO> GetBookCopyByIdAsync(int id);
        public Task<IEnumerable<BookCopyDTO>> GetAllBookCopiesAsync();
        public Task<BookCopyDTO> AddBookCopyAsync(BookCopyCreateDTO bookCopy);
        public Task<BookCopyDTO> UpdateBookCopyAsync(BookCopyUpdateDTO bookCopy);
        public Task<bool> DeleteBookCopyAsync(int id);
        public Task<bool> Save();
    }
}
