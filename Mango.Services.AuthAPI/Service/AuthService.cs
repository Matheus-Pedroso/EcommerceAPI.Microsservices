using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service;

public class AuthService(AppDbContext _context, 
    UserManager<ApplicationUser> _userManager, 
    RoleManager<IdentityRole> _roleManager, 
    IJwtTokenGenerator _jwtTokenGenerator
    ) : IAuthService
{
    public async Task<bool> AssignRole(string email, string roleName)
    {
        var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        if (user is not null)
        {
            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                // crate role if does not exist
                _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }
            await _userManager.AddToRoleAsync(user, roleName);
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        if (loginRequestDTO is LoginRequestDTO)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user is null || isValid == false)
                return new LoginResponseDTO() { User = null, Token = "" };

            // Create JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDTO userDTO = new()
            {
                ID = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            LoginResponseDTO loginResponseDTO = new()
            {
                User = userDTO,
                Token = token
            };
            return loginResponseDTO;
        }
        return new LoginResponseDTO() { User = null, Token = "" };
    }

    public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
    {
        if (registrationRequestDTO == null)
            return "Data invalid";

        ApplicationUser user = new()
        {
            UserName = registrationRequestDTO.Email,
            Email = registrationRequestDTO.Email,
            NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
            Name = registrationRequestDTO.Name,
            PhoneNumber = registrationRequestDTO.PhoneNumber
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
            if (result.Succeeded)
            {
                var userToReturn = _context.ApplicationUsers.First(u => u.UserName == registrationRequestDTO.Email);

                UserDTO userDTO = new()
                {
                    ID = userToReturn.Id,
                    Email = userToReturn.Email,
                    Name = userToReturn.Name,
                    PhoneNumber = userToReturn.PhoneNumber
                };

                return "";
            }
            else
            {
                return result.Errors.FirstOrDefault().Description;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
