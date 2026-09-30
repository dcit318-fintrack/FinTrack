using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required."), EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required."), MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required."), MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
}
