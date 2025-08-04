using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Services.Interface;
using ResortApp.Domain.Entities;

namespace ResortApp.Web.Controllers;

[Authorize]
public class VillaController : Controller
{

    private readonly IVillaService _villaService;

    public VillaController(IVillaService villaService)
    {
        _villaService = villaService;
    }

    // GET
    public IActionResult Index()
    {
        var villas = _villaService.GetAllVillas();
        return View(villas);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Villa obj)
    {
        if (obj.Name == obj.Description)
        {
            ModelState.AddModelError("name", "The description cannot exactly match the Name.");
        }

        if (ModelState.IsValid)
        {

            _villaService.CreateVilla(obj);
            TempData["success"] = "The villa has been created successfully!";

            return RedirectToAction(nameof(Index));
        }

        return View(obj);
    }

    public IActionResult Update(int villaId)
    {
        Villa? obj = _villaService.GetVillaById(villaId);
        // Villa? obj = _unitOfWork.Villas.Find(villaId);
        // var villaList = _unitOfWork.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
        if (obj == null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }

    [HttpPost]
    public IActionResult Update(Villa obj)
    {
        if (ModelState.IsValid && obj.Id > 0)
        {
            _villaService.UpdateVilla(obj);
            TempData["success"] = "The villa has been updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        return View(obj);
    }

    public IActionResult Delete(int villaId)
    {
        Villa? obj = _villaService.GetVillaById(villaId);
        // Villa? obj = _unitOfWork.Villas.Find(villaId);
        // var villaList = _unitOfWork.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }

        return View(obj);
    }

    [HttpPost]
    public IActionResult Delete(Villa obj)
    {
        bool deleted = _villaService.DeleteVilla(obj.Id);
        if (deleted)
        {
            TempData["success"] = "The villa has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = "Failed to delete the villa.";
        }

        return View();
    }

}