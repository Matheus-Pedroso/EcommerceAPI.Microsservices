namespace Mango.Services.AuthAPI.Models.DTO;

public class LoginRequestDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
