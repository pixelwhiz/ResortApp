using System.ComponentModel.DataAnnotations;

namespace ResortApp.Web.ViewModels;

public class LoginViewModel
{
    [Microsoft.Build.Framework.Required]
    public string Email { get; set; }


    [Microsoft.Build.Framework.Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }

    public string? RedirectUrl { get; set; }
}