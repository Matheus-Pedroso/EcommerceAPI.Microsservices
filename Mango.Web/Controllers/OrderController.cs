using System.Collections.Generic;
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

    public async Task<IActionResult> OrderDetail(int orderId)
    {
        OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
        string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;

        var responseDTO = await orderService.GetOrder(orderId);
        if (responseDTO != null)
        {
            orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(responseDTO.Result));
        }
        if (!User.IsInRole(StaticDetails.RoleAdmin) && userId != orderHeaderDTO.UserId)
        {
            return NotFound();
        }
        return View(orderHeaderDTO);
    }
    [HttpPost("OrderReadyForPickup")]
    public async Task<IActionResult> OrderReadyForPickup(int orderId)
    {
        var responseDTO = await orderService.UpdateOrderStatus(orderId, StaticDetails.Status_ReadyForPickup);
        if (responseDTO != null)
        {
            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }
        return View();
    }

    [HttpPost("CompleteOrder")]
    public async Task<IActionResult> CompleteOrder(int orderId)
    {
        var responseDTO = await orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Completed);
        if (responseDTO != null)
        {
            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }
        return View();
    }

    [HttpPost("CancelOrder")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var responseDTO = await orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Cancelled);
        if (responseDTO != null)
        {
            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }
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
