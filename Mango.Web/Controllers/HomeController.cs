using System.Diagnostics;
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class HomeController(IProductService productService, ILogger<HomeController> logger) : Controller
    {
        public async Task<IActionResult> Index()
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

        public async Task<IActionResult> Details(int productId)
        {
            ProductDTO product = new ProductDTO();

            ResponseDTO? response = await productService.GetProductAsync(productId);

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response?.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
