using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services;

public class CouponService(IHttpClientFactory httpClientFactory) : ICouponService
{
    public async Task<CouponDTO> GetCouponByCode(string code)
    {
        var cliente = httpClientFactory.CreateClient("Coupon");
        var response = await cliente.GetAsync($"/api/coupon/GetByCode/{code}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (resp != null && resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(resp.Result));
        }

        return new CouponDTO();
    }
}
