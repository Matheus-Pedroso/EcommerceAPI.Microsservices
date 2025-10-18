using Mango.Services.Web.Models.DTO;
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
            Url = StaticDetails.CartAPIBase + "/api/order/CreateOrder"
        });
    }
}
