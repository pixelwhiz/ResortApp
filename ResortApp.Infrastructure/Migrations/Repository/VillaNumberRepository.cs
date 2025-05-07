using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Infrastructure.Migrations.Repository;

public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
{

    private readonly ApplicationDbContext _db;

    public VillaNumberRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(VillaNumber entity)
    {
        _db.VillaNumbers.Update(entity);
    }

}