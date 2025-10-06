using Mango.Services.Web.Models.DTO;

namespace Mango.Services.Web.Models.DTO;

public class CartDTO
{
    public CartHeaderDTO? CartHeader { get; set; }
    public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
}
