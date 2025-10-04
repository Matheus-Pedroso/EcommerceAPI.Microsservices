using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Services.IServices;

public interface ICouponService
{
    Task<CouponDTO> GetCouponByCode(string code);
}
