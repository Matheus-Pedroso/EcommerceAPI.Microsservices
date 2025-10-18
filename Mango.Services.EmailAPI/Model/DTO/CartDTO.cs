namespace Mango.Services.EmailAPI.Model.DTO;

public class CartDTO
{
    public CartHeaderDTO? CartHeader { get; set; }
    public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
}
