using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using Mango.Services.Web.Models.DTO;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController(ICartService cartService) : Controller
{
    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDTOBasedOnLoggedInUser());
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDTO? response = await cartService.RemoveCartAsync(cartDetailsId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
    {
        ResponseDTO? response = await cartService.ApplyCouponAsync(cartDTO);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon has been added";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EmailCart(CartDTO cartDTO)
    {
        CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();
        cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

        ResponseDTO? response = await cartService.EmailCart(cart);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Email will be processed and sent shortly";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
    {
        cartDTO.CartHeader.CouponCode = "";
        ResponseDTO? response = await cartService.ApplyCouponAsync(cartDTO);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon has been added";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDTO? response = await cartService.GetCartAsync(userId);
        if (response != null && response.IsSuccess)
        {
            CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
            return cartDTO;
        }
        return new CartDTO();
    }
}
