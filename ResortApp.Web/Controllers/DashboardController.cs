using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Common.Utility;

namespace ResortApp.Web.Controllers;

public class DashboardController : Controller
{

    private readonly IUnitOfWork _unitOfWork;
    static int previousMonth = DateTime.Now.Month == 1? 12 : DateTime.Now.Month -1;

    private readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
    private readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetTotalBookingRadialChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
        || u.Status == SD.StatusCancelled);

        var countByCurrentMonth =
            totalBookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);

        var countByPreviousMonth =
            totalBookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= currentMonthStartDate);

    }

}