using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController(IProductService productService) : Controller
{
    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDTO> products = new List<ProductDTO>();

        ResponseDTO? response = await productService.GetProductsAsync();

        if (response != null && response.IsSuccess)
        {
            products = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["Error"] = response?.Message;
        }
        return View(products);
    }

    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDTO model)
    {
        if (ModelState.IsValid)
        {
            ResponseDTO? response = await productService.CreateProductAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Product has been created";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
        }
        return View(model);
    }

    public async Task<IActionResult> ProductUpdate(int id)
    {
        ProductDTO product = new ProductDTO();

        ResponseDTO? response = await productService.GetProductAsync(id);

        if (response != null && response.IsSuccess)
        {
            product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["Error"] = response?.Message;
            return RedirectToAction(nameof(ProductIndex));
        }
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> ProductUpdate(ProductDTO model)
    {
        if (ModelState.IsValid)
        {
            ResponseDTO? response = await productService.UpdateProductAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Product has been updated";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
        }
        return View(model);
    }

    public async Task<IActionResult> ProductDelete(int id)
    {
        ProductDTO product = new ProductDTO();

        ResponseDTO? response = await productService.GetProductAsync(id);

        if (response != null && response.IsSuccess)
        {
            product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["Error"] = response?.Message;
            return RedirectToAction(nameof(ProductIndex));
        }
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDTO model)
    {
        ResponseDTO? response = await productService.DeleteProductAsync(model.ProductId);

        if (response != null && response.IsSuccess)
        {
            TempData["Success"] = "Product has been deleted";
            return RedirectToAction(nameof(ProductIndex));
        }
        else
        {
            TempData["Error"] = response?.Message;
        }
        return View();
    }
}
