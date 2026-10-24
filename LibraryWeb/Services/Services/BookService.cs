using System.Text.Json;
using LibraryWeb.Models;
using LibraryWeb.Models.APIResponse;
using LibraryWeb.Services.IServices;

namespace LibraryWeb.Services.Services
{
    public class BookService : BaseService,IBookService
    {
        private readonly string _libraryApiUrl;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public BookService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {
            _libraryApiUrl = configuration.GetValue<string>("ServiceUrls:LibraryAPI");
        }
        public async Task<APIResponse> CreateBookAsync(BookCreateDTO bookCreateDTO)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.POST,
                Data = bookCreateDTO,
                Url = $"{_libraryApiUrl}/api/BookAPI"
            });

            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<BookDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }

            return apiResponse;
        }

        public async Task<APIResponse> DeleteBookAsync(int bookId)
        {
            
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.DELETE,
                Data = bookId,
                Url = $"{_libraryApiUrl}/api/BookAPI"+ $"/{bookId}"
            });

           
            return apiResponse;
        }

        public async Task<APIResponse> GetAllBooksAsync(string author = null)
        {
           var apiResponse = await SendAsync(new APIRequest
           {
               Type = ApiType.GET,
               Url = $"{_libraryApiUrl}/api/BookAPI" + (string.IsNullOrEmpty(author) ? "" : $"?author={Uri.EscapeDataString(author)}")
           });
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<List<BookDTO>>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }
            return apiResponse;
        }
        public async Task<APIResponse> GetBookByIdAsync(int bookId)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.GET,
                Url = $"{_libraryApiUrl}/api/BookAPI/{bookId}"
            });
            // convert the result to BookDTO if the response is successful and the result is not null : 
            
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<BookDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }

            return apiResponse;
        }

        public async Task<APIResponse> UpdateBookAsync(BookUpdateDTO bookUpdateDTO)
        {
            var apiResponse = await SendAsync(new APIRequest
            {
                Type = ApiType.PUT,
                Data = bookUpdateDTO,
                Url = $"{_libraryApiUrl}/api/BookAPI" + $"/{bookUpdateDTO.Id}"
            });

            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                apiResponse.Result = JsonSerializer.Deserialize<BookDTO>(
                    apiResponse.Result.ToString(), _jsonOptions);
            }

            return apiResponse;
        }
    }
}
