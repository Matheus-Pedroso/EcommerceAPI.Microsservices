using Mango.Web.Models;
using Mango.Web.Service.IService;

namespace Mango.Web.Service;

public class CouponService(IBaseService baseService ) : ICouponService
{

    public Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDTO?> DeleteCouponAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDTO?> GetAllCouponsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDTO?> GetCoupon(string couponCode)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDTO?> GetCouponByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO)
    {
        throw new NotImplementedException();
    }
}
