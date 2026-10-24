using LibraryWeb.Models.APIResponse;
using LibraryWeb.Models.AuthDTO;

namespace LibraryWeb.Services.IServices
{
    public interface IAuthService
    {
        Task<APIResponse> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<APIResponse> LoginAsync(LoginRequestDTO loginRequestDTO);
    }
}
