using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

public class AmenityController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public AmenityController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
        return View(amenities);
    }

    public IActionResult Create()
    {
        AmenityVM amenityVm = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            })
        };
        return View(amenityVm);
    }

    [HttpPost]
    public IActionResult Create(AmenityVM obj)
    {
        // ModelState.Remove("Villa");
        if (ModelState.IsValid)
        {
            _unitOfWork.Amenity.Add(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The amenity has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        return View(obj);
    }

    public IActionResult Update(int amenityId)
    {
        AmenityVM amenityVm = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
        };

        if (amenityVm.Amenity == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(amenityVm);
    }

    [HttpPost]
    public IActionResult Update(AmenityVM amenityVm)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Amenity.Update(amenityVm.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "The amenity has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        amenityVm.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        return View(amenityVm);
    }


    public IActionResult Delete(int amenityId)
    {
        AmenityVM amenityVm = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
        };

        if (amenityVm.Amenity == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(amenityVm);
    }

    [HttpPost]
    public IActionResult Delete(AmenityVM amenityVm)
    {
        Amenity? objFromDb = _unitOfWork.Amenity
            .Get(u => u.Id == amenityVm.Amenity.Id);
        if (objFromDb is not null)
        {
            _unitOfWork.Amenity.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "The amenity has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The amenity could not be deleted!";
        return View();
    }

}