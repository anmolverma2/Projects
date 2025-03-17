using FM.Web.Utility;
using static FM.Web.Utility.SD;

namespace FM.Web.Models
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }

    }
}
