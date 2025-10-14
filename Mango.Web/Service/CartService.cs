using Mango.Services.Web.Models.DTO;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class CartService(IBaseService baseService) : ICartService
{
    public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDTO,
            Url = StaticDetails.CartAPIBase + "/api/cart/ApplyCoupon"
        });
    }

    public async Task<ResponseDTO?> EmailCart(CartDTO cartDTO)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDTO,
            Url = StaticDetails.CartAPIBase + "/api/cart/EmailCartRequest"
        });
    }

    public async Task<ResponseDTO?> GetCartAsync(string userId)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CartAPIBase + $"/api/cart/GetCart/{userId}"
        });
    }

    public async Task<ResponseDTO?> RemoveCartAsync(int cartDetailsId)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDetailsId,
            Url = StaticDetails.CartAPIBase + $"/api/cart/RemoveCart"
        });
    }

    public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDTO,
            Url = StaticDetails.CartAPIBase + $"/api/cart/CartUpsert"
        });
    }
}
