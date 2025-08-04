using Microsoft.AspNetCore.Hosting;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Services.Interface;
using ResortApp.Domain.Entities;

namespace ResortApp.Application.Services.Implementation;

public class VillaNumberService : IVillaNumberService
{

    private readonly IUnitOfWork _unitOfWork;

    public VillaNumberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public void CreateVillaNumber(VillaNumber villaNumber)
    {
        _unitOfWork.VillaNumber.Add(villaNumber);
        _unitOfWork.Save();

    }

    public bool DeleteVillaNumber(int id)
    {
        try
        {
            VillaNumber? objFromDb = _unitOfWork.VillaNumber.Get(u => u.VillaNum == id);
            if (objFromDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(objFromDb);
                _unitOfWork.Save();
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public void UpdateVillaNumber(VillaNumber villaNumber)
    {
        _unitOfWork.VillaNumber.Update(villaNumber);
        _unitOfWork.Save();
    }

    public IEnumerable<VillaNumber> GetAllVillaNumbers()
    {
        return _unitOfWork.VillaNumber.GetAll();
    }

    public VillaNumber GetVillaNumberById(int id)
    {
        return _unitOfWork.VillaNumber.Get(u => u.VillaNum == id);
    }
}