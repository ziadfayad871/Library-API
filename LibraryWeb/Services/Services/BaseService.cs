using System.Text;
using System.Text.Json;
using LibraryWeb.Models.APIResponse;
using LibraryWeb.Services.IServices;

namespace LibraryWeb.Services.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public BaseService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse> SendAsync(APIRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var message = new HttpRequestMessage
                {
                    RequestUri = new Uri(request.Url),
                    Method = request.Type switch
                    {
                        ApiType.POST => HttpMethod.Post,
                        ApiType.PUT => HttpMethod.Put,
                        ApiType.DELETE => HttpMethod.Delete,
                        _ => HttpMethod.Get
                    }
                };
                message.Headers.Add("Accept", "application/json");

                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWTToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                if (request.Data != null && (request.Type == ApiType.POST || request.Type == ApiType.PUT))
                {
                    message.Content = new StringContent(
                        JsonSerializer.Serialize(request.Data),
                        Encoding.UTF8,
                        "application/json");
                }

                var apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(apiContent))
                {
                    var response = JsonSerializer.Deserialize<APIResponse>(apiContent, _jsonOptions);
                    if (response != null)
                    {
                        return response;
                    }
                }

                return new APIResponse
                {
                    StatusCode = (int)apiResponse.StatusCode,
                    IsSuccess = apiResponse.IsSuccessStatusCode
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }
    }
}
