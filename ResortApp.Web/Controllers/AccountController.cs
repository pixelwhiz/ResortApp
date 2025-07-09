using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

public class AccountController : Controller
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Login(string returnUrl=null)
    {
        returnUrl??= Url.Content("~/");
        LoginVM loginVM = new()
        {
            RedirectUrl = returnUrl
        };

        return View(loginVM);
    }

    public IActionResult Register()
    {
        return View();
    }

}