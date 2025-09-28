using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models;

public class RegistrationRequestDTO
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public string? RoleName { get; set; } = string.Empty;
}
