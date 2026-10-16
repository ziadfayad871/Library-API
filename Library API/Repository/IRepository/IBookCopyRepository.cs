using Library_API.Model;

namespace Library_API.Repository.IRepository
{
    public interface IBookCopyRepository
    {
        public Task<BookCopy> GetBookCopyByIdAsync(int id);
        public Task<IEnumerable<BookCopy>> GetAllBookCopiesAsync();
        public Task<BookCopy> AddBookCopyAsync(BookCopy bookCopy);
        public Task<BookCopy> UpdateBookCopyAsync(BookCopy bookCopy);
        public Task<bool> DeleteBookCopyAsync(int id);
        public Task<bool> BookCopySavedAsync(int id);
    }
}
