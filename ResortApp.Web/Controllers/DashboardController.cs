using Microsoft.AspNetCore.Mvc;

namespace ResortApp.Web.Controllers;

public class DashboardController : Controller
{

    public IActionResult Index()
    {
        return View();
    }

}