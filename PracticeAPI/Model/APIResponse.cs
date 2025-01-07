using System.Net;

namespace PracticeAPI.Model
{
    public class APIResponse
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public dynamic Data { get; set; }
        public string Error { get; set; }
    }
}
