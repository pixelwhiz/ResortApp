using Microsoft.AspNetCore.Mvc;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;

namespace ResortApp.Web.Controllers;

public class VillaController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public VillaController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // GET
    public IActionResult Index()
    {
        var villas = _unitOfWork.Villa.GetAll();
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
            _unitOfWork.Villa.Add(obj);
            _unitOfWork.Villa.Save();

            TempData["success"] = "The villa has been created successfully!";

            return RedirectToAction(nameof(Index));
        }

        return View(obj);
    }

    public IActionResult Update(int villaId)
    {
        Villa? obj = _unitOfWork.Villa.Get(u=>u.Id==villaId);
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
            _unitOfWork.Villa.Update(obj);
            _unitOfWork.Villa.Save();
            TempData["success"] = "The villa has been updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        return View(obj);
    }

    public IActionResult Delete(int villaId)
    {
        Villa? obj = _unitOfWork.Villa.Get(u=>u.Id==villaId);
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
        Villa? objFromDb = _unitOfWork.Villa.Get(u => u.Id == obj.Id);
        if (objFromDb is not null)
        {
            _unitOfWork.Villa.Remove(objFromDb);
            _unitOfWork.Villa.Save();

            TempData["success"] = "The villa has been deleted successfully!";

            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa could not be deleted!";
        return View();
    }

}