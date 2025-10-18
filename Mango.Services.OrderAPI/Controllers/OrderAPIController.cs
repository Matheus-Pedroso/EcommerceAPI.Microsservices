using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderAPI.Controllers;

[Route("api/order")]
[ApiController]
public class OrderAPIController(IProductService productService, AppDbContext context, IMapper mapper) : ControllerBase
{
    protected ResponseDTO _response = new ResponseDTO();

    [Authorize]
    [HttpPost("CreateOrder")]
    public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
    {
        try
        {
            OrderHeaderDTO orderHeaderDTO = mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
            orderHeaderDTO.OrderTime = DateTime.Now;
            orderHeaderDTO.Status = StaticDetails.Status_Pending;
            orderHeaderDTO.OrderDetails = mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

            var orderCreated = await context.OrderHeaders.AddAsync(mapper.Map<OrderHeader>(orderHeaderDTO));
            await context.SaveChangesAsync();

            orderHeaderDTO.OrderHeaderId = orderHeaderDTO.OrderHeaderId;
            _response.Result = orderHeaderDTO;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }
}
