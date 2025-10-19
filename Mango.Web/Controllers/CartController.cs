using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController(ICartService cartService, IOrderService orderService) : Controller
{
    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDTOBasedOnLoggedInUser());
    }

    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        return View(await LoadCartDTOBasedOnLoggedInUser());
    }
    
    [HttpPost]
    [ActionName("Checkout")]
    public async Task<IActionResult> Checkout(CartDTO cartDTO)
    {
        CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();
        cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
        cart.CartHeader.Email = cartDTO.CartHeader.Email;
        cart.CartHeader.Name = cartDTO.CartHeader.Name;

        ResponseDTO? response = await orderService.CreateOrder(cart);
        OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

        if (response != null && response.IsSuccess)
        {
            // get stripe session and redirect to stripe to place order
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            StripeRequestDTO stripeRequestDTO = new StripeRequestDTO()
            {
                ApprovedUrl = domain + "cart/confirmation?orderId=" + orderHeaderDTO.OrderHeaderId,
                CancelUrl = domain + "cart/checkout",
                OrderHeader = orderHeaderDTO
            };

            var stripeSessionResponse = await orderService.CreateStripeSession(stripeRequestDTO);

            StripeRequestDTO stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeSessionResponse.Result));
            Response.Headers.Add("location", stripeResponseResult.StripeSessionUrl);

            return new StatusCodeResult(303);
        }
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Confirmation(int orderId)
    {
        return View(orderId);
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
