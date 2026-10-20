namespace LibraryWeb.Models.APIResponse
{
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class APIRequest
    {
        public ApiType Type { get; set; }
        public object Data { get; set; }
        public string Url { get; set; }
    }
}
