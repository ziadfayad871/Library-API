using LibraryWeb.Models;
using LibraryWeb.Models.APIResponse;

namespace LibraryWeb.Services.IServices
{
    public interface IBookService
    {
        Task<APIResponse> GetAllBooksAsync(string author = null);
        Task<APIResponse> GetBookByIdAsync(int bookId);
        Task<APIResponse> CreateBookAsync(BookCreateDTO bookCreateDTO);
        Task<APIResponse> UpdateBookAsync( BookUpdateDTO bookUpdateDTO);
        Task<APIResponse> DeleteBookAsync(int bookId);
    }
}
