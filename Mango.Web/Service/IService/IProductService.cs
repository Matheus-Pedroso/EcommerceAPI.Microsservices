using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IProductService
{
    Task<ResponseDTO?> GetProductsAsync();
    Task<ResponseDTO?> GetProductAsync(int id);
    Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO);
    Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO);
    Task<ResponseDTO?> DeleteProductAsync(int id);
}
