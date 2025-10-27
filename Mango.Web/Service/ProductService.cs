using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class ProductService(IBaseService baseService) : IProductService
{
    public async Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.POST,
            Url = StaticDetails.ProductAPIBase + "/api/product",
            Data = productDTO,
            ContentType = StaticDetails.ContentType.MultipartFormData
        });
    }

    public async Task<ResponseDTO?> DeleteProductAsync(int id)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.DELETE,
            Url = StaticDetails.ProductAPIBase + $"/api/product/{id}",
        });
    }

    public async Task<ResponseDTO?> GetProductAsync(int id)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.ProductAPIBase + $"/api/product/{id}",
        });
    }

    public async Task<ResponseDTO?> GetProductsAsync()
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.ProductAPIBase + "/api/product",
        });
    }

    public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.PUT,
            Url = StaticDetails.ProductAPIBase + "/api/product",
            Data = productDTO,
            ContentType = StaticDetails.ContentType.MultipartFormData
        });
    }
}
