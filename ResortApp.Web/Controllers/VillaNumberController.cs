using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Services.Interface;
using ResortApp.Domain.Entities;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

public class VillaNumberController : Controller
{

    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;

    public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
    {
        _villaNumberService = villaNumberService;
        _villaService = villaService;
    }

    public IActionResult Index()
    {
        var villaNumbers = _villaNumberService.GetAllVillaNumbers();
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        VillaNumberVM villaNumberVm = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            })
        };
        return View(villaNumberVm);
    }

    [HttpPost]
    public IActionResult Create(VillaNumberVM obj)
    {
        // ModelState.Remove("Villa");

        bool roomNumberExists = _villaNumberService.CheckVillaNumberExists(obj.VillaNumber.VillaNum);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _villaNumberService.CreateVillaNumber(obj.VillaNumber);
            TempData["success"] = "The villa number has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The villa number is already exists!";
        }

        obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        return View(obj);
    }

    public IActionResult Update(int villaNumberId)
    {
        VillaNumberVM villaNumberVm = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
        };

        if (villaNumberVm.VillaNumber == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(villaNumberVm);
    }

    [HttpPost]
    public IActionResult Update(VillaNumberVM villaNumberVm)
    {
        if (ModelState.IsValid)
        {
            _villaNumberService.UpdateVillaNumber(villaNumberVm.VillaNumber);
            TempData["success"] = "The villa number has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        villaNumberVm.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        return View(villaNumberVm);
    }


    public IActionResult Delete(int villaNumberId)
    {
        VillaNumberVM villaNumberVm = new()
        {
            VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
        };

        if (villaNumberVm.VillaNumber == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(villaNumberVm);
    }

    [HttpPost]
    public IActionResult Delete(VillaNumberVM villaNumberVm)
    {
        VillaNumber? objFromDb = _villaNumberService.GetVillaNumberById(villaNumberVm.VillaNumber.VillaNum);
        if (objFromDb is not null)
        {
            _villaNumberService.DeleteVillaNumber(objFromDb.VillaNum);
            TempData["success"] = "The villa number has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa could not be deleted!";
        return View();
    }

}