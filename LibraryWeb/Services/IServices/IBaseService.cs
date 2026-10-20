using LibraryWeb.Models.APIResponse;

namespace LibraryWeb.Services.IServices
{
    public interface IBaseService
    {
        Task<APIResponse> SendAsync(APIRequest request);
    }
}
