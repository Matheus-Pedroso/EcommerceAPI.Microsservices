using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.AuthAPI.Service;

public class JwtTokenGenerator(IOptions<JwtOptions> _jwtOptions) : IJwtTokenGenerator
{
    public string GenerateToken(ApplicationUser applicationUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

        var claimList = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtOptions.Value.Audience,
            Issuer = _jwtOptions.Value.Issuer,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddDays(_jwtOptions.Value.ExpirationInDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
