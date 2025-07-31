using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Common.Utility;
using ResortApp.Web.ViewModels;

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

        return Json(GetRadialChartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));
    }

    public async Task<IActionResult> GetRegisteredUserChartData()
    {
        var totalUsers = _unitOfWork.User.GetAll();
        var countByCurrentMonth =
            totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.Now);
        var countByPreviousMonth =
            totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= currentMonthStartDate);

        return Json(GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));
    }

    public async Task<IActionResult> GetRevenueChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCancelled);
        var totalReveneue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));
        var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now).Sum(u => u.TotalCost);
        var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

        return Json(GetRadialChartDataModel(totalReveneue, countByCurrentMonth, countByPreviousMonth));
    }

    public async Task<IActionResult> GetBookingPieChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

        var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

        int bookingsByNewCustomer = customerWithOneBooking.Count();
        int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

        PieChartDto pieChartDto = new()
        {
            Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
            Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
        };

        return Json(pieChartDto);
    }

    public async Task<IActionResult> GetMemberAndBookingLineChartData()
    {
        var bookingData = _unitOfWork.Booking
            .GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && u.BookingDate.Date <= DateTime.Now)
            .GroupBy(b => b.BookingDate)
            .Select(u => new
            {
                DateTime = u.Key,
                NewBookingCount = u.Count()
            });

        var customerData = _unitOfWork.User
            .GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) && u.CreatedAt.Date <= DateTime.Now)
            .GroupBy(b => b.CreatedAt)
            .Select(u => new
            {
                DateTime = u.Key,
                NewCustomerCount = u.Count()
            });

        var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
            (booking, customer) => new
            {
                booking.DateTime,
                booking.NewBookingCount,
                NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault(),
            });

        var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
            (customer, booking) => new
            {
                customer.DateTime,
                NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                customer.NewCustomerCount
            });

        var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

        var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
        var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
        var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

        List<ChartData> chartDataList = new()
        {
            new ChartData()
            {
                Name = "New Bookings",
                Data = newBookingData
            },
            new ChartData()
            {
                Name = "New Members",
                Data = newCustomerData
            }
        };

        LineChartDto lineChartDto = new()
        {
            Categories = categories,
            Series = chartDataList
        };

        return Json(lineChartDto);
    }

    private static RadialBarChartDto GetRadialChartDataModel(int totalCount, double currentMonthCount, double prevMonthCount)
    {
        RadialBarChartDto radialBarChartDto = new();

        int increaseDecreaseRatio = 100;

        if (prevMonthCount != 0)
        {
            increaseDecreaseRatio =
                Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
        }

        radialBarChartDto.TotalCount = totalCount;
        radialBarChartDto.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
        radialBarChartDto.HasRatioIncreased = currentMonthCount > prevMonthCount;
        radialBarChartDto.Series = new int[] { increaseDecreaseRatio };
        return radialBarChartDto;
    }

}