using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class OrderService(IBaseService baseService) : IOrderService
{
    public async Task<ResponseDTO?> CreateOrder(CartDTO cartDTO)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDTO,
            Url = StaticDetails.OrderAPIBase + "/api/order/CreateOrder"
        });
    }

    public async Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = stripeRequestDTO,
            Url = StaticDetails.OrderAPIBase + "/api/order/CreateStripeSession"
        });
    }

    public async Task<ResponseDTO?> GetAllOrder(string? userId)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Data = userId,
            Url = StaticDetails.OrderAPIBase + "/api/order/GetOrders"
        });
    }

    public async Task<ResponseDTO?> GetOrder(int? orderId)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.OrderAPIBase + $"/api/order/GetOrder/{orderId}"
        });
    }

    public async Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = newStatus,
            Url = StaticDetails.OrderAPIBase + $"/api/order/UpdateOrderStatus/{orderId}"
        });
    }

    public async Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId)
    {
        return await baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = orderHeaderId,
            Url = StaticDetails.OrderAPIBase + "/api/order/ValidateStripeSession"
        });
    }
}
