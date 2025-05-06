using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Infrastructure.Migrations.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{

    private readonly ApplicationDbContext _db;

    public VillaRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Villa entity)
    {
        _db.Update(entity);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}