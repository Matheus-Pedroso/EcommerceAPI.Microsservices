using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class CouponService(IBaseService baseService) : ICouponService
{
    public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO)
    {
        return await baseService.SendAsync(new RequestDTO 
        { 
            ApiType = StaticDetails.ApiType.PUT, 
            Url = StaticDetails.CouponAPIBase + "/api/coupon",
            Data = couponDTO
        });
    }

    public async Task<ResponseDTO?> DeleteCouponAsync(int id)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.DELETE,
            Url = StaticDetails.CouponAPIBase + $"/api/coupon/{id}",
        });
    }

    public async Task<ResponseDTO?> GetAllCouponsAsync()
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CouponAPIBase + "/api/coupon",
        });
    }

    public async Task<ResponseDTO?> GetCoupon(string couponCode)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CouponAPIBase + $"/api/coupon/GetByCode/{couponCode}",
        });
    }

    public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CouponAPIBase + $"/api/coupon/{id}",
        });
    }

    public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.PUT,
            Url = StaticDetails.CouponAPIBase + "/api/coupon",
            Data = couponDTO
        });
    }
}
