using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

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

            var orderCreated = context.OrderHeaders.Add(mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
            await context.SaveChangesAsync();

            orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;
            _response.Result = orderHeaderDTO;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [Authorize]
    [HttpPost("CreateStripeSession")]
    public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
    {
        try
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = stripeRequestDTO.ApprovedUrl,
                CancelUrl = stripeRequestDTO.CancelUrl,
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach(var item in stripeRequestDTO.OrderHeader.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(item.Price * 100), // R$ 20.99 -> 2099
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            stripeRequestDTO.StripeSessionUrl = session.Url;
            OrderHeader orderHeader = context.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
            orderHeader.StripeSessionId = session.Id;
            session.AmountTotal -= (long)orderHeader.Discount;
            context.SaveChanges();
            _response.Result = stripeRequestDTO;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }
        return _response;
    }
}
