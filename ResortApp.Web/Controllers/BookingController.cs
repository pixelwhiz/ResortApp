using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Common.Utility;
using ResortApp.Domain.Entities;
using Stripe;
using Stripe.Checkout;

namespace ResortApp.Web.Controllers;

public class BookingController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ApplicationUser user = _unitOfWork.User.Get(u => u.Id == userId);

        Booking booking = new()
        {
            VillaId = villaId,
            Villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
            CheckInDate = checkInDate,
            Nights = nights,
            CheckOutDate = checkInDate.AddDays(nights),
            UserId = userId,
            Phone = user.PhoneNumber,
            Email = user.Email,
            Name = user.Name,
        };

        booking.TotalCost = booking.Villa.Price * nights;
        return View(booking);
    }

    [Authorize]
    [HttpPost]
    public IActionResult FinalizeBooking(Booking booking)
    {
        var villa = _unitOfWork.Villa.Get(u => u.Id == booking.VillaId);
        booking.TotalCost = villa.Price * booking.Nights;

        booking.Status = SD.StatusPending;
        booking.BookingDate = DateTime.Now;

        var villaNumbersList = _unitOfWork.VillaNumber.GetAll().ToList();
        var bookedVillas = _unitOfWork.Booking
            .GetAll(u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn).ToList();

        int roomAvailable = SD.VillaRoomsAvailable_Count(villa.Id, villaNumbersList, booking.CheckInDate, booking.Nights, bookedVillas);

        if (roomAvailable == 0)
        {
            TempData["error"] = "Room has been sold out!";
            return RedirectToAction(nameof(FinalizeBooking), new
            {
                villaId = booking.VillaId,
                checkInDate = booking.CheckInDate,
                nights = booking.Nights,
            });
        }

        _unitOfWork.Booking.Add(booking);
        _unitOfWork.Save();

        var domain = $"{Request.Scheme}://{Request.Host.Value}";
        var options = new SessionCreateOptions()
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = $"{domain}/booking/BookingConfirmation?bookingId={booking.Id}",
            CancelUrl = $"{domain}/booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate:yyyy-MM-dd}&nights={booking.Nights}"
        };

        options.LineItems.Add(new SessionLineItemOptions()
        {
            PriceData = new SessionLineItemPriceDataOptions()
            {
                UnitAmount = (long)(booking.TotalCost * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions()
                {
                    Name = villa.Name,
                    // Images = new List<string> { domain + villa.ImageUrl }
                },
            },
            Quantity = 1,
        });

        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.Booking.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    [Authorize]
    public IActionResult BookingConfirmation(int bookingId)
    {
        Booking bookingFromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Villa");

        if (bookingFromDb.Status == SD.StatusPending)
        {
            //this is a pending order, we need to confirm if payment was successful

            var service = new SessionService();
            Session session = service.Get(bookingFromDb.StripeSessionId);

            if (session.PaymentStatus == "paid")
            {
                _unitOfWork.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved, bookingFromDb.VillaNumber);
                _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
            }

        }
        
        return View(bookingId);
    }

    [Authorize]
    public IActionResult BookingDetails(int bookingId)
    {
        Booking bookingFromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId,
            includeProperties: "User,Villa");

        if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
        {
            var availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId);
            bookingFromDb.VillaNumbers = _unitOfWork.VillaNumber.GetAll(u => u.VillaId == bookingFromDb.VillaId
                && availableVillaNumber.Any(x => x == u.VillaNum)).ToList();
        }

        return View(bookingFromDb);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CheckIn(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
        _unitOfWork.Save();
        TempData["Success"] = "Booking Updated Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CheckOut(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);
        _unitOfWork.Save();
        TempData["Success"] = "Booking Completed Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CancelBooking(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCancelled, 0);
        _unitOfWork.Save();
        TempData["Success"] = "Booking Cancelled Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    private List<int> AssignAvailableVillaNumberByVilla(int villaId)
    {
        List<int> availableVillaNumbers = new();
        var villaNumbers = _unitOfWork.VillaNumber.GetAll(u => u.VillaId == villaId);
        var checkedInVilla = _unitOfWork.Booking.GetAll(u => u.VillaId == villaId && u.Status == SD.StatusCheckedIn)
            .Select(u=>u.VillaNumber);
        foreach (var villaNumber in villaNumbers)
        {
            if (!checkedInVilla.Contains(villaNumber.VillaNum))
            {
                availableVillaNumbers.Add(villaNumber.VillaNum);
            }
        }

        return availableVillaNumbers;
    }

    #region API Calls
    [HttpGet]
    [Authorize]
    public IActionResult GetAll(string status)
    {
        IEnumerable<Booking> objBookings;
        if (User.IsInRole(SD.Role_Admin))
        {
            objBookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Villa");
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            objBookings = _unitOfWork.Booking
                .GetAll(u => u.UserId == userId, includeProperties: "User,Villa");
        }

        if (!string.IsNullOrEmpty(status))
        {
            objBookings = objBookings.Where(u => u.Status.ToLower().Equals(status.ToLower()));

        }
        return Json(new { data = objBookings });
    }
    #endregion

}