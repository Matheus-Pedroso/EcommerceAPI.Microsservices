using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class AuthController(IAuthService authService, ITokenProvider tokenProvider) : Controller
{
    [HttpGet]
    public IActionResult Login()
    { 
        LoginRequestDTO loginRequestDTO = new();
        return View(loginRequestDTO);
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDTO model)
    { 
        ResponseDTO responseDTO = await authService.LoginAsync(model);

        if (responseDTO != null && responseDTO.IsSuccess)
        {
            LoginResponseDTO loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDTO.Result))!;
            await SignInUser(loginResponse);
            tokenProvider.SetToken(loginResponse.Token);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError("CustomError", responseDTO?.Message??"Error encountered when login");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        var roleList = new List<SelectListItem>()
        {
            new SelectListItem{ Text = StaticDetails.RoleAdmin, Value = StaticDetails.RoleAdmin },
            new SelectListItem{ Text = StaticDetails.RoleCustomer, Value = StaticDetails.RoleCustomer },
        };

        ViewBag.RoleList = roleList;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegistrationRequestDTO model)
    {
        ResponseDTO result = await authService.RegisterAsync(model);
        ResponseDTO assignRole;

        if (result != null && result.IsSuccess)
        {
            if (string.IsNullOrEmpty(model.RoleName))
            {
                model.RoleName = StaticDetails.RoleCustomer;
            }

            assignRole = await authService.AssignRoleAsync(model);
            if (assignRole != null && assignRole.IsSuccess)
            {
                TempData["success"] = "Registration successful.";
                return RedirectToAction(nameof(Login));
            }
        }

        var roleList = new List<SelectListItem>()
        {
            new SelectListItem{ Text = StaticDetails.RoleAdmin, Value = StaticDetails.RoleAdmin },
            new SelectListItem{ Text = StaticDetails.RoleCustomer, Value = StaticDetails.RoleCustomer },
        };

        ViewBag.RoleList = roleList;
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        tokenProvider.ClearToken();
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(LoginResponseDTO loginResponse)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(loginResponse.Token);

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
        
        identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
