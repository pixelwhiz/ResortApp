using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResortApp.Domain.Entities;
using ResortApp.Infrastructure.Data;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

public class VillaNumberController : Controller
{

    private readonly ApplicationDbContext _db;

    public VillaNumberController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var villaNumbers = _db.VillaNumbers.Include(u => u.Villa).ToList();
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        VillaNumberVM villaNumberVM = new()
        {
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            })
        };
        return View(villaNumberVM);
    }

    [HttpPost]
    public IActionResult Create(VillaNumber obj)
    {
        // ModelState.Remove("Villa");
        if (ModelState.IsValid)
        {
            _db.VillaNumbers.Add(obj);
            _db.SaveChanges();

            TempData["success"] = "The villa number has been created successfully!";

            return RedirectToAction("Index");
        }

        return View(obj);
    }

    // public IActionResult Update(int villaId)
    // {
    //     Villa? obj = _db.VillaNumbers.FirstOrDefault(u=>u.Id==villaId);
    //     // Villa? obj = _db.Villas.Find(villaId);
    //     // var villaList = _db.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
    //     if (obj == null)
    //     {
    //         return RedirectToAction("Error", "Home");
    //     }
    //
    //     return View(obj);
    // }
    //
    // [HttpPost]
    // public IActionResult Update(Villa obj)
    // {
    //     if (ModelState.IsValid && obj.Id > 0)
    //     {
    //         _db.VillaNumbers.Update(obj);
    //         _db.SaveChanges();
    //         TempData["success"] = "The villa has been updated successfully!";
    //
    //         return RedirectToAction("Index");
    //     }
    //     return View(obj);
    // }
    //
    // public IActionResult Delete(int villaId)
    // {
    //     Villa? obj = _db.VillaNumbers.FirstOrDefault(u=>u.Id==villaId);
    //     // Villa? obj = _db.Villas.Find(villaId);
    //     // var villaList = _db.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
    //     if (obj is null)
    //     {
    //         return RedirectToAction("Error", "Home");
    //     }
    //
    //     return View(obj);
    // }
    //
    // [HttpPost]
    // public IActionResult Delete(Villa obj)
    // {
    //     Villa? objFromDb = _db.VillaNumbers.FirstOrDefault(u => u.Id == obj.Id);
    //     if (objFromDb is not null)
    //     {
    //         _db.VillaNumbers.Remove(objFromDb);
    //         _db.SaveChanges();
    //
    //         TempData["success"] = "The villa has been deleted successfully!";
    //
    //         return RedirectToAction("Index");
    //     }
    //
    //     TempData["error"] = "The villa could not be deleted!";
    //     return View();
    // }

}