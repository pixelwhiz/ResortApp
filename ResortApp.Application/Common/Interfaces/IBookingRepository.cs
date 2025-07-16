using ResortApp.Domain.Entities;

namespace ResortApp.Application.Common.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    void Update(Booking entity);
    void UpdateStatus(int bookingId, string bookingStatus);
    void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
}