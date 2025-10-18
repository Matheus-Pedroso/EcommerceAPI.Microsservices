using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Model.DTO;

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Count { get; set; } = 1;
}
