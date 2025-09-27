using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class AuthService(IBaseService baseService) : IAuthService
{
    public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = registrationRequestDTO,
            Url = StaticDetails.AuthAPIBase + "/api/auth/AssignRole"
        });
    }

    public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = loginRequestDTO,
            Url = StaticDetails.AuthAPIBase + "/api/auth/login"
        });
    }

    public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
    {
        return await baseService.SendAsync(new RequestDTO
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = registrationRequestDTO,
            Url = StaticDetails.AuthAPIBase + "/api/auth/register"
        });
    }
}
