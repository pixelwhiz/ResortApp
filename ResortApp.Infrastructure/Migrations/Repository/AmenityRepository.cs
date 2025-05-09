using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Infrastructure.Migrations.Repository;

public class AmenityRepository : Repository<Amenity>, IAmenityRepository
{

    private readonly ApplicationDbContext _db;

    public AmenityRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Amenity entity)
    {
        _db.Amenities.Update(entity);
    }

}