using Library_API.Dtos;
using Library_API.Dtos.APIResponse;

namespace Library_API.Repository.IRepository
{
    public interface IBookRepository
    {
        public Task<APIResponse> CreateBook(BookCreateDTO bookCreateDTO);
        public Task<APIResponse> UpdateBook(BookUpdateDTO bookUpdateDTO);
        public Task<APIResponse> GetBookById(int bookId);
        public Task<APIResponse> GetAllBooks(string? author = null);
        public Task<APIResponse> DeleteBook(int bookId);
        public Task<bool> Save();
    }
}
