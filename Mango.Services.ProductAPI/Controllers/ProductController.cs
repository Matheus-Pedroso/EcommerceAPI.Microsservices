using System.Globalization;
using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    //[Authorize(Roles = "ADMINISTRATOR")]
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
    public ResponseDTO Post(ProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            _context.Products.Add(product);
            _context.SaveChanges();

            // Verify image of product
            if (productDTO.Image != null)
            {
                string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + fileName;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    productDTO.Image.CopyTo(fileStream);
                }
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                product.ImageLocalPathUrl = filePath;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }
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

    [HttpPut]
    [Authorize(Roles = "ADMINISTRATOR")]
    public ResponseDTO Put(ProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            _context.Products.Update(product);
            _context.SaveChanges();

            // Verify image of product
            if (productDTO.Image != null)
            {
                // delete old image
                if (!string.IsNullOrEmpty(product.ImageLocalPathUrl))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPathUrl);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                        file.Delete();
                }

                // new image
                string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + fileName;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    productDTO.Image.CopyTo(fileStream);
                }
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                product.ImageLocalPathUrl = filePath;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }

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
            if (!string.IsNullOrEmpty(product.ImageLocalPathUrl))
            {
                var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPathUrl);
                FileInfo file = new FileInfo(oldFilePathDirectory);
                if (file.Exists)
                    file.Delete();
            }
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
