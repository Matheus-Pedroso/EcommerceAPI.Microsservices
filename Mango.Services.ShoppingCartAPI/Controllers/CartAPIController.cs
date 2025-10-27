using System.Net.Http;
using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[Route("api/cart")]
[ApiController]
public class CartAPIController(AppDbContext _context, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration) : ControllerBase
{
    private ResponseDTO _response = new ResponseDTO();

    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDTO> GetCartFromUserId(string userId)
    {
        try
        {
            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = mapper.Map<CartHeaderDTO>(_context.CartHeaders.First(u => u.UserId == userId))
            };
            cartDTO.CartDetails = mapper.Map<IEnumerable<CartDetailsDTO>>(_context.CartDetails.Where(u => u.CartHeaderId == cartDTO.CartHeader.CartHeaderId));
            
            foreach (var item in cartDTO.CartDetails)
            {
                item.Product = await productService.GetProductById(item.ProductId);
                cartDTO.CartHeader.CartTotal += (item.Count * item.Product.Price);
            }

            // Apply discount
            if (!string.IsNullOrEmpty(cartDTO.CartHeader.CouponCode))
            {
                var coupon = await couponService.GetCouponByCode(cartDTO.CartHeader.CouponCode);
                if (coupon != null && cartDTO.CartHeader.CartTotal > coupon.MinAmount)
                {
                    cartDTO.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cartDTO.CartHeader.Discount = coupon.DiscountAmount;
                }
            }

            _response.Result = cartDTO; 
        }
        catch (Exception ex) 
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<ResponseDTO> ApplyCoupon([FromBody] CartDTO cartDTO)
    {
        try
        {
            var cart = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeader.UserId);
            cart.CouponCode = cartDTO.CartHeader.CouponCode;
            _context.CartHeaders.Update(cart);
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost("EmailCartRequest")]
    public async Task<ResponseDTO> EmailCartRequest([FromBody] CartDTO cartDTO)
    {
        try
        {
            await messageBus.PublishMessage(cartDTO, configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost("RemoveCoupon")]
    public async Task<ResponseDTO> RemoveCoupon([FromBody] CartDTO cartDTO)
    {
        try
        {
            var cart = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeader.UserId);
            cart.CouponCode = "";
            _context.CartHeaders.Update(cart);
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost("CartUpsert")]
    [Authorize]
    public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
    {
        try
        {
            var cartHeaderFromDB = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);
            if (cartHeaderFromDB == null)
            {
                // Create cart Header
                var cartHeader = mapper.Map<CartHeader>(cartDTO.CartHeader);
                _context.CartHeaders.Add(cartHeader);
                await _context.SaveChangesAsync();

                // Create cart Details
                var cartDetails = mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                cartDetails.CartHeaderId = cartHeader.CartHeaderId;
                _context.CartDetails.Add(cartDetails);
                await _context.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDB = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cartDTO.CartDetails.First().ProductId && u.CartHeaderId == cartHeaderFromDB.CartHeaderId);
                if (cartDetailsFromDB == null)
                {
                    //create cartDetails
                    var cartDetails = mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                    cartDetails.CartHeaderId = cartHeaderFromDB.CartHeaderId;
                    _context.CartDetails.Add(cartDetails);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //update count in cartDetails
                    cartDTO.CartDetails.First().Count += cartDetailsFromDB.Count;
                    cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDB.CartHeaderId;
                    cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDB.CartDetailsId;
                    var cartDetails = mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                    _context.CartDetails.Update(cartDetails);
                    await _context.SaveChangesAsync();
                }
            }
            _response.Result = cartDTO;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message.ToString();
        }
        return _response;
    }
    [HttpPost("RemoveCart")]
    public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId)
    {
        try
        {
            var cartDetails = _context.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
            int totalCountofCartitem = _context.CartDetails.Where(u => u.CartHeaderId == cartDetailsId).Count();

            _context.CartDetails.Remove(cartDetails);
            if (totalCountofCartitem == 1)
            {
                // remove cartDetails and cartHeader
                var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                _context.CartHeaders.Remove(cartHeader);
            }
            await _context.SaveChangesAsync();

            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message.ToString();
        }
        return _response;
    }
}
