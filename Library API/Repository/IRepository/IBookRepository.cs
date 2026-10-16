using Library_API.Dtos;

namespace Library_API.Repository.IRepository
{
    public interface IBookRepository
    {
        public Task<BookDTO> CreateBook(BookCreateDTO bookCreateDTO);
        public Task<BookDTO> UpdateBook(BookUpdateDTO bookUpdateDTO);
        public Task<BookDTO> GetBookById(int bookId);
        public Task<IEnumerable<BookDTO>> GetAllBooks();
        public Task<bool> DeleteBook(int bookId);
        public Task<bool> Save();
    }
}
