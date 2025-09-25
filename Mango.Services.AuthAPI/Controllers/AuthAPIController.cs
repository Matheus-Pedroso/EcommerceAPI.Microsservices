using Azure;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthAPIController(IAuthService authService) : ControllerBase
{
    protected ResponseDTO _response = new ResponseDTO();

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
    {
        var errorMessage = await authService.Register(model);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            _response.IsSuccess = false;
            _response.Message = errorMessage;
            return BadRequest(_response);
        }

        return Ok(_response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        var result = await authService.Login(model);
        if (result.User is null)
        {
            _response.IsSuccess = false;
            _response.Message = "Username or password is incorrect";
            return BadRequest(_response);
        }

        _response.Result = result;

        return Ok(_response);
    }

    [HttpPost("AssignRole")]
    public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
    {
        var result = await authService.AssignRole(model.Email, model.RoleName.ToUpper());
        if (!result)
        {
            _response.IsSuccess = false;
            _response.Message = "Role doesn't assigned";
            return BadRequest(_response);
        }
        return Ok(_response);
    }
}
