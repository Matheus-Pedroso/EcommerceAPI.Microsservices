using Mango.Services.Web.Models.DTO;
using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface ICartService
{
    Task<ResponseDTO?> GetCart(string userId);
    Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO);
    Task<ResponseDTO?> RemoveCart(int cartDetailsId);
    Task<ResponseDTO?> ApplyCoupon(CartDTO cartDTO);
}
