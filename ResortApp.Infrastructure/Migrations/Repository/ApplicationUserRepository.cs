using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Infrastructure.Migrations.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{

    private readonly ApplicationDbContext _db;

    public ApplicationUserRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}