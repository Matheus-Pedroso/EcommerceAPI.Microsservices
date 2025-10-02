using System.Collections.Generic;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CouponController(ICouponService couponService) : Controller
{
    public async Task<IActionResult> CouponIndex()
    {
        List<CouponDTO> list = new();

        ResponseDTO? response = await couponService.GetAllCouponsAsync();

        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["Error"] = response?.Message;
        }

        return View(list);
    }

    public async Task<IActionResult> CouponCreate()
    {
        return View();
    }
    

    [HttpPost]
    public async Task<IActionResult> CouponCreate(CouponDTO model)
    {
        if (ModelState.IsValid)
        {
            ResponseDTO? response = await couponService.CreateCouponAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupon created successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
        }
        return View();
    }

    public async Task<IActionResult> CouponDelete(int couponId)
    {
        CouponDTO? coupon = new CouponDTO();

        ResponseDTO? response = await couponService.GetCouponByIdAsync(couponId);

        if (response != null && response.IsSuccess)
        {
            coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["Error"] = response?.Message;
            return RedirectToAction(nameof(CouponIndex));
        }
        return View(coupon);
    }

    [HttpPost]
    public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
    {
        ResponseDTO? response = await couponService.DeleteCouponAsync(couponDTO.CouponId);

        if (response != null && response.IsSuccess)
        {
            TempData["Success"] = "Coupon deleted successfully";
            return RedirectToAction(nameof(CouponIndex));
        }
        else
        {
            TempData["Error"] = response?.Message;
        }
        return View(couponDTO);
    }
}
