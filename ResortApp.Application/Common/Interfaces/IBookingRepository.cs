using ResortApp.Domain.Entities;

namespace ResortApp.Application.Common.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    void Update(Booking entity);
}