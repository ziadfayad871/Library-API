using LibraryWeb.Models.APIResponse;
using LibraryWeb.Models.AuthDTO;
     using LibraryWeb.Services.IServices;
using System.Text.Json;

namespace LibraryWeb.Services.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly string _libraryApiUrl;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {
            _libraryApiUrl = configuration.GetValue<string>("ServiceUrls:LibraryAPI");
        }
     
        public async Task<APIResponse> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.POST,
                Data = loginRequestDTO,
                Url = $"{_libraryApiUrl}/api/v1/UserAPI/login"
            });

            if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<LoginResponseDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }
            return apiResponse;
        }

        public async Task<APIResponse> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.POST,
                Data = registrationRequestDTO,
                Url = $"{_libraryApiUrl}/api/v1/UserAPI/register"
            });

            if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<UserDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }
            return apiResponse;
        }
    }
}
