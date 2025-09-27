using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    { 
        LoginRequestDTO loginRequestDTO = new();
        return View(loginRequestDTO);
    }

    [HttpGet]
    public IActionResult Register()
    { 
        return View();
    }

    public IActionResult Logout()
    { 
        return View();
    }
}
