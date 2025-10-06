using System.IdentityModel.Tokens.Jwt;
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

    private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDTO? response = await cartService.GetCart(userId);
        if (response != null && response.IsSuccess)
        {
            CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
            return cartDTO;
        }
        return new CartDTO();
    }
}
