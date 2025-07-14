using ResortApp.Application.Common.Interfaces;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Infrastructure.Migrations.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public IVillaRepository Villa { get; private set; }
    public IApplicationUserRepository User { get; private set; }
    public IAmenityRepository Amenity { get; private set; }
    public IBookingRepository Booking { get; private set; }
    public IVillaNumberRepository VillaNumber { get; private set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Villa = new VillaRepository(_db);
        User = new ApplicationUserRepository(_db);
        Amenity = new AmenityRepository(_db);
        Booking = new BookingRepository(_db);
        VillaNumber = new VillaNumberRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }


}