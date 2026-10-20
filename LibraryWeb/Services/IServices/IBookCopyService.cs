using LibraryWeb.Models.APIResponse;
using LibraryWeb.Models.CopyDTO;

namespace LibraryWeb.Services.IServices
{
    public interface IBookCopyService
    {
        Task<APIResponse> GetAllBookCopiesAsync();
        Task<APIResponse> GetBookCopyByIdAsync(int copyNo);
        Task<APIResponse> CreateBookCopyAsync(BookCopyCreateDTO bookCopyCreateDTO);
        Task<APIResponse> UpdateBookCopyAsync(BookCopyUpdateDTO bookCopyUpdateDTO);
        Task<APIResponse> DeleteBookCopyAsync(int copyNo);
    }
}
