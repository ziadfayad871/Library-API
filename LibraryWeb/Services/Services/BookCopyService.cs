using LibraryWeb.Models;
using LibraryWeb.Models.APIResponse;
using LibraryWeb.Models.CopyDTO;
using LibraryWeb.Services.IServices;
using System.Net;
using System.Text.Json;

namespace LibraryWeb.Services.Services
{
    public class BookCopyService : BaseService, IBookCopyService
    {
        private readonly string _libraryApiUrl;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public BookCopyService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {
            _libraryApiUrl = configuration.GetValue<string>("ServiceUrls:LibraryAPI");
        }

        public async Task<APIResponse> CreateBookCopyAsync(BookCopyCreateDTO bookCopyCreateDTO)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.POST,
                Data = bookCopyCreateDTO,
                Url = $"{_libraryApiUrl}/api/v1/BookCopyAPI"
            });

            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<BookCopyDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }

            return apiResponse;
        }

        public async Task<APIResponse> DeleteBookCopyAsync(int copyNo)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.DELETE,
                Data = copyNo,
                Url = $"{_libraryApiUrl}/api/v1/BookCopyAPI" + $"/{copyNo}"
            });

            
            return apiResponse;
        }

        public async Task<APIResponse> GetAllBookCopiesAsync()
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.GET,
                Url = $"{_libraryApiUrl}/api/v1/BookCopyAPI"
            });
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<List<BookCopyDTO>>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }
            return apiResponse;
        }

        public async Task<APIResponse> GetBookCopyByIdAsync(int copyNo)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.GET,
                Url = $"{_libraryApiUrl}/api/v1/BookCopyAPI" + $"/{copyNo}"
            });
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<BookCopyDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }
            return apiResponse;
        }

        public async Task<APIResponse> UpdateBookCopyAsync(BookCopyUpdateDTO bookCopyUpdateDTO)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.PUT,
                Data = bookCopyUpdateDTO,
                Url = $"{_libraryApiUrl}/api/v1/BookCopyAPI" + $"/{bookCopyUpdateDTO.CopyNo}"
            });
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<BookCopyDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }
            return apiResponse;
        }
    }
}
