using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class HomeController(IProductService productService, ICartService cartService, ILogger<HomeController> logger) : Controller
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

        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> Details(ProductDTO productDTO)
        {
            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = new CartHeaderDTO
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDTO cartDetailsDTO = new CartDetailsDTO()
            {
                Count = productDTO.Count,
                ProductId = productDTO.ProductId
            };

            List<CartDetailsDTO> cartDetails = new() { cartDetailsDTO };
            cartDTO.CartDetails = cartDetails; 

            ResponseDTO? response = await cartService.UpsertCartAsync(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Item has been added to the Shopping Cart";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }

            return View(productDTO);
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
