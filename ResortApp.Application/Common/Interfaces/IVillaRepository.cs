using System.Linq.Expressions;
using ResortApp.Domain.Entities;

namespace ResortApp.Application.Common.Interfaces;

public interface IVillaRepository : IRepository<Villa>
{
    void Update(Villa entity);
}