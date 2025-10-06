using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mango.Web.Models;

namespace Mango.Services.Web.Models.DTO;

public class CartDetailsDTO
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public int ProductId { get; set; }
    public ProductDTO Product { get; set; }
    public int Count { get; set; } = 0;
}
