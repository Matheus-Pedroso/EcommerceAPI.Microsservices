using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController(AppDbContext _context, IMapper _mapper) : ControllerBase
{
    protected ResponseDTO _response = new ResponseDTO();

    [HttpGet]
    public ResponseDTO Get()
    {
        try
        {
            IEnumerable<ProductDTO> products = _mapper.Map<IEnumerable<ProductDTO>>(_context.Products.ToList());
            _response.Result = products;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public ResponseDTO Get(int id)
    {
        try
        {
            ProductDTO product = _mapper.Map<ProductDTO>(_context.Products.First(p => p.ProductId == id));
            _response.Result = product;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPost]
    [Authorize(Roles = "ADMINISTRATOR")]
    public ResponseDTO Post([FromBody] ProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            _context.Products.Add(product);
            _context.SaveChanges();
            _response.Result = _mapper.Map<ProductDTO>(product);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpPut]
    [Authorize(Roles = "ADMINISTRATOR")]
    public ResponseDTO Put([FromBody] ProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            _context.Products.Update(product);
            _context.SaveChanges();
            _response.Result = _mapper.Map<ProductDTO>(product);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public ResponseDTO Delete(int id)
    {
        try
        {
            Product product = _context.Products.First(p => p.ProductId == id);
            _context.Products.Remove(product);
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
