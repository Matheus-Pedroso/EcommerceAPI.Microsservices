using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[ApiController]
public class CouponAPIController(AppDbContext _context, IMapper _mapper) : ControllerBase
{
    protected ResponseDTO _response = new ResponseDTO();

    [HttpGet]
    [Authorize]
    public ResponseDTO Get()
    {
        try
        {
            IEnumerable<Coupon> coupons = _context.Coupons.ToList();
            _response.Result = _mapper.Map<IEnumerable<CouponDTO>>(coupons);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpGet]
    [Route("{id:int}")]

    public ResponseDTO Get(int id)
    {
        try
        {
            Coupon coupon = _context.Coupons.First(c => c.CouponId == id);
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpGet]
    [Route("GetByCode/{code}")]

    public ResponseDTO Get(string code)
    {
        try
        {
            Coupon coupon = _context.Coupons.First(c => c.CouponCode.ToLower() == code.ToLower());            
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public ResponseDTO Post([FromBody] CouponDTO couponDTO)
    {
        try
        {
            Coupon coupon = _mapper.Map<Coupon>(couponDTO);
            _context.Add(coupon);
            _context.SaveChanges();
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public ResponseDTO Put([FromBody] CouponDTO couponDTO)
    {
        try
        {
            Coupon coupon = _mapper.Map<Coupon>(couponDTO);
            _context.Update(coupon);
            _context.SaveChanges();
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public ResponseDTO Delete(int id)
    {
        try
        {
            Coupon coupon = _context.Coupons.First(c => c.CouponId == id);
            _context.Remove(coupon);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }
}
