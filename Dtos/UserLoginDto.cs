using System.ComponentModel.DataAnnotations;

namespace TZTDate_UserWebApi.Dtos;

public class UserLoginDto
{

    [EmailAddress]
    [Required(ErrorMessage = "Email cannot be empty")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password cannot be empty")]
    public string? Password { get; set; }
    [Required(ErrorMessage = "IpAddress cannot be empty")]
    public string? IpAddress { get; set; }
}
