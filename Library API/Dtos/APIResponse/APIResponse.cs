using Newtonsoft.Json;

namespace Library_API.Dtos.APIResponse
{
    public class APIResponse
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public object Result { get; set; } = null!;

        [JsonIgnore]
        public Pagination Pagination { get; set; }
    }
}
