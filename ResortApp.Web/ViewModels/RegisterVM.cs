using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResortApp.Web.ViewModels;

public class RegisterVM
{
    [Required] public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required] public string Name { get; set; } = string.Empty;

    [Display(Name = "Phone Number")] public string? PhoneNumber { get; set; } = string.Empty;

    public string? RedirectUrl { get; set; }
    public string? Role { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem>? RoleList { get; set; }
}