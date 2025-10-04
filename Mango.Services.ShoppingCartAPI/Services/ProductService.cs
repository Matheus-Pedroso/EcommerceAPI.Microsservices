using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services;

public class ProductService(IHttpClientFactory _httpClientFactory) : IProductService
{
    public async Task<IEnumerable<ProductDTO>> GetProducts()
    {
        var client = _httpClientFactory.CreateClient("Product");
        var response = await client.GetAsync($"/api/product");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(resp.Result));
        }
        return new List<ProductDTO>();
    }
    public async Task<ProductDTO> GetProductById(int id)
    {
        var client = _httpClientFactory.CreateClient("Product");
        var response = await client.GetAsync($"/api/product/{id}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (resp.IsSuccess)
        {
            return JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(resp.Result));
        }
        return new ProductDTO();
    }

}
