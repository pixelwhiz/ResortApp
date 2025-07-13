using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Infrastructure.Migrations.Repository;

public class BookingRepository : Repository<Booking>, IBookingRepository
{

    private readonly ApplicationDbContext _db;

    public BookingRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Booking entity)
    {
        _db.Bookings.Update(entity);
    }

}