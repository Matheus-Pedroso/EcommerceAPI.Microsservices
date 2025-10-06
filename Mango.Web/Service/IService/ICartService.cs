using Mango.Services.Web.Models.DTO;
using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface ICartService
{
    Task<ResponseDTO?> GetCartAsync(string userId);
    Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO);
    Task<ResponseDTO?> RemoveCartAsync(int cartDetailsId);
    Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO);
}
