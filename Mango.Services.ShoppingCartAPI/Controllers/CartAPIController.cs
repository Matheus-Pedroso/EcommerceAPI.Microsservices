using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[Route("api/cart")]
[ApiController]
public class CartAPIController(AppDbContext _context, IMapper mapper) : ControllerBase
{
    private ResponseDTO _response = new ResponseDTO();

    [HttpGet("/cart/{userId}")]
    public async Task<ResponseDTO> GetCartFromUser(string userId)
    {
        try
        {

        }
        catch (Exception ex) 
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost("CartUpsert")]
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
}
