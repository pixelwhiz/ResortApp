using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Services.Interface;
using ResortApp.Domain.Entities;

namespace ResortApp.Application.Services.Implementation;

public class AmenityService : IAmenityService
{
    private readonly IUnitOfWork _unitOfWork;

    public AmenityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void CreateAmenity(Amenity amenity)
    {
        ArgumentNullException.ThrowIfNull(amenity);
        _unitOfWork.Amenity.Add(amenity);
        _unitOfWork.Save();
    }

    public bool DeleteAmenity(int id)
    {
        try
        {
            var amenity = _unitOfWork.Amenity.Get(u => u.Id == id);
            if (amenity != null)
            {
                _unitOfWork.Amenity.Remove(amenity);
                _unitOfWork.Save();
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Amenity with ID {id} not found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return false;
    }

    public IEnumerable<Amenity> GetAllAmenities()
    {
        return _unitOfWork.Amenity.GetAll(includeProperties: "villa");
    }

    public Amenity GetAmenityById(int id)
    {
        return _unitOfWork.Amenity.Get(u => u.Id == id, includeProperties: "villa");
    }

    public void UpdateAmenity(Amenity amenity)
    {
        ArgumentNullException.ThrowIfNull(amenity);
        _unitOfWork.Amenity.Update(amenity);
        _unitOfWork.Save();
    }
}