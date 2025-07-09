using System.ComponentModel.DataAnnotations;

namespace ResortApp.Web.ViewModels;

public class RegisterViewModel
{
    [Microsoft.Build.Framework.Required]
    public string Email { get; set; }

    [Microsoft.Build.Framework.Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Microsoft.Build.Framework.Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    [Required]
    public string Name { get; set; }

    [Display(Name="Phone Number")]
    public string? PhoneNumber { get; set; }

    public string? RedirectUrl { get; set; }
}