using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Common.Utility;
using ResortApp.Application.Services.Implementation;
using ResortApp.Application.Services.Interface;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

public class DashboardController : Controller
{

    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetTotalBookingRadialChartData()
    {
        return Json(await _dashboardService.GetTotalBookingRadialChartData());
    }

    public async Task<IActionResult> GetRegisteredUserChartData()
    {
        return Json(await _dashboardService.GetRegisteredUserChartData());
    }

    public async Task<IActionResult> GetRevenueChartData()
    {
        return Json(await _dashboardService.GetRevenueChartData());
    }

    public async Task<IActionResult> GetBookingPieChartData()
    {
        return Json(await _dashboardService.GetBookingPieChartData());
    }

    public async Task<IActionResult> GetMemberAndBookingLineChartData()
    {
        return Json(await _dashboardService.GetMemberAndBookingLineChartData());
    }

}