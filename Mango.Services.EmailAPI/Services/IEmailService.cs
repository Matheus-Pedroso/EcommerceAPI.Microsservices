using Mango.Services.EmailAPI.Model.DTO;

namespace Mango.Services.EmailAPI.Services;

public interface IEmailService
{
    Task EmailCartAndLog(CartDTO cartDTO);
    Task EmailRegisterUserAndLog(string email);
}
