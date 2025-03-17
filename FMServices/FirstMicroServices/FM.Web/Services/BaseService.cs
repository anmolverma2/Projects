using FM.Services.CoupenAPI.Models;
using FM.Web.Models;
using FM.Web.Services.IService;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;
using static FM.Web.Utility.SD;

namespace FM.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO,bool isWithBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("FM.WebClient");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                var token = _tokenProvider.GetToken();

                if (isWithBearer)
                {
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDTO.Url);

                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), System.Text.Encoding.UTF8, "application/json");
                }
                HttpResponseMessage? response = null;
                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                response = await client.SendAsync(message);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await response.Content.ReadAsStringAsync();
                        var apiContentDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiContentDTO;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO()
                {
                    Message = ex.Message,
                    IsSuccess = false,
                    Data = ex.Data
                };
                return dto;
            }
        }


    }
}
