using System.Text;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Service;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            // token

            message.RequestUri = new Uri(requestDTO.Url);
            if (requestDTO.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
            }
            HttpResponseMessage? apiResponse = null;

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

            apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case System.Net.HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Access Denied" };
                case System.Net.HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case System.Net.HttpStatusCode.InternalServerError:
                    return new() { IsSuccess = false, Message = "InternalServerError" };
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                    return apiResponseDTO;
            }
        }
        catch (Exception ex)
        {
            var dto = new ResponseDTO
            {
                Message = ex.Message.ToString(),
                IsSuccess = false
            };
            return dto;
        }
    }
}
