using Mango.Services.Web.Models.DTO;
using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IOrderService
{
    Task<ResponseDTO?> CreateOrder(CartDTO cartDTO);
}
