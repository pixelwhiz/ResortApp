using ResortApp.Domain.Entities;
using ResortApp.Web.ViewModels;

namespace ResortApp.Application.Common.Utility;

public static class SD
{
    public const string Role_Customer = "Customer";
    public const string Role_Admin = "Admin";

    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusCheckedIn = "CheckedIn";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    public static int VillaRoomsAvailable_Count(int villaId,
        List<VillaNumber> villaNumberList,
        DateOnly checkInDate, int nights,
        List<Booking> bookings
    )
    {
        List<int> bookingInDate = new();
        int finalAvailableRoomForAllNights = int.MaxValue;
        var roomsInVilla = villaNumberList.Where(x => x.VillaId == villaId).Count();
        for (int i = 0; i < nights; i++)
        {
            var villasBooked = bookings.Where(u =>
                u.CheckInDate <= checkInDate.AddDays(i) && u.CheckOutDate > checkInDate.AddDays(i) &&
                u.VillaId == villaId);

            foreach (var booking in villasBooked)
            {
                if (!bookingInDate.Contains(booking.Id))
                {
                    bookingInDate.Add(booking.Id);
                }
            }

            var totalAvailableRooms = roomsInVilla - bookingInDate.Count;
            if (totalAvailableRooms == 0)
            {
                return 0;
            }
            else
            {
                if (finalAvailableRoomForAllNights > totalAvailableRooms)
                {
                    finalAvailableRoomForAllNights = totalAvailableRooms;
                }
            }

        }

        return finalAvailableRoomForAllNights;
    }

    public static RadialBarChartDto GetRadialChartDataModel(int totalCount, double currentMonthCount, double prevMonthCount)
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