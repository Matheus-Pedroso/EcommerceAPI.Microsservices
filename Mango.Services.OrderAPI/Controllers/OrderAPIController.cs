using AutoMapper;
using Mango.MessageBus;
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
public class OrderAPIController(IProductService productService, AppDbContext context, IMapper mapper, IConfiguration configuration, IMessageBus messageBus) : ControllerBase
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

            var discountObj = new List<SessionDiscountOptions>()
            {
                new SessionDiscountOptions
                {
                    Coupon = stripeRequestDTO.OrderHeader.CouponCode
                }
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

            // If have discount
            if (stripeRequestDTO.OrderHeader.Discount > 0)
                options.Discounts = discountObj;

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
    [Authorize]
    [HttpPost("ValidateStripeSession")]
    public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
    {
        try
        {
            OrderHeader orderHeader = context.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

            var service = new SessionService();
            Session session = service.Get(orderHeader.StripeSessionId);

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

            if (paymentIntent.Status == "succeeded")
            {
                orderHeader.PaymentIntentId = paymentIntent.Id;
                orderHeader.Status = StaticDetails.Status_Approved;
                context.SaveChanges();

                RewardsDTO rewardsDTO = new()
                {
                    OrderId = orderHeader.OrderHeaderId,
                    RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                    UserId = orderHeader.UserId
                };
                string topicName = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                await messageBus.PublishMessage(rewardsDTO, topicName);

                _response.Result = mapper.Map<OrderHeaderDTO>(orderHeader);
            }
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message;
            _response.IsSuccess = false;
        }
        return _response;
    }
}
