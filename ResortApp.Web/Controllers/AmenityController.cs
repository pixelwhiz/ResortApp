using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Common.Utility;
using ResortApp.Application.Services.Interface;
using ResortApp.Domain.Entities;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

[Authorize(Roles = SD.Role_Admin)]
public class AmenityController : Controller
{

    private readonly IAmenityService _amenityService;
    private readonly IVillaService _villaService;

    public AmenityController(IAmenityService amenityService, IVillaService villaService)
    {
        _amenityService = amenityService;
        _villaService = villaService;
    }

    public IActionResult Index()
    {
        var amenities = _amenityService.GetAllAmenities();
        return View(amenities);
    }

    public IActionResult Create()
    {
        AmenityVM amenityVm = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
            _amenityService.CreateAmenity(obj.Amenity);
            TempData["success"] = "The amenity has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Amenity = _amenityService.GetAmenityById(amenityId)
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
            _amenityService.UpdateAmenity(amenityVm.Amenity);
            TempData["success"] = "The amenity has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        amenityVm.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Amenity = _amenityService.GetAmenityById(amenityId)
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
        Amenity? objFromDb = _amenityService.GetAmenityById(amenityVm.Amenity.Id);
        if (objFromDb is not null)
        {
            _amenityService.DeleteAmenity(objFromDb.Id);
            TempData["success"] = "The amenity has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The amenity could not be deleted!";
        return View();
    }

}