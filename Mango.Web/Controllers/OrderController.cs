using System.IdentityModel.Tokens.Jwt;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class OrderController(IOrderService orderService) : Controller
{
    public IActionResult OrderIndex()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<OrderHeaderDTO> list;
        string userId = string.Empty;
        if (!User.IsInRole(StaticDetails.RoleAdmin))
        {
            userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
        }
        ResponseDTO responseDTO = orderService.GetAllOrder(userId).GetAwaiter().GetResult();
        if (responseDTO != null)
        {
            list = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(responseDTO.Result));
        }
        else
        {
            list = new List<OrderHeaderDTO>();
        }
        return Json(new { data = list });
    }
}
