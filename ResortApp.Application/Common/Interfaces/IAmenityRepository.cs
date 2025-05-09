using ResortApp.Domain.Entities;

namespace ResortApp.Application.Common.Interfaces;

public interface IAmenityRepository : IRepository<Amenity>
{
    void Update(Amenity entity);
}