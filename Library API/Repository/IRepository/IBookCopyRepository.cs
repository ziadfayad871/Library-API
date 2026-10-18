using Library_API.Dtos.APIResponse;
using Library_API.Dtos.BookCopyDTOs;

namespace Library_API.Repository.IRepository
{
    public interface IBookCopyRepository
    {
        public Task<APIResponse> GetBookCopyByIdAsync(int id);
        public Task<APIResponse> GetAllBookCopiesAsync();
        public Task<APIResponse> AddBookCopyAsync(BookCopyCreateDTO bookCopy);
        public Task<APIResponse> UpdateBookCopyAsync(BookCopyUpdateDTO bookCopy);
        public Task<APIResponse> DeleteBookCopyAsync(int id);
        public Task<bool> Save();
    }
}
