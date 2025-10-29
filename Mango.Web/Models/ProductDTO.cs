using System.ComponentModel.DataAnnotations;
using Mango.Web.Utility;

namespace Mango.Web.Models;

public class ProductDTO
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public string? ImageLocalPathUrl { get; set; } = string.Empty;

    [Range(1,100)]
    public int Count { get; set; } = 1;

    [AllowedExtensions(new string[] {".jpg", ".png"})]
    public IFormFile? Image { get; set; }
}
