using ResortApp.Domain.Entities;

namespace ResortApp.Application.Common.Interfaces;

public interface IVillaNumberRepository : IRepository<VillaNumber>
{
    void Update(VillaNumber entity);
}