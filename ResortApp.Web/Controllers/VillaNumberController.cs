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
        VillaNumberVM villaNumberVm = new()
        {
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
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

        bool roomNumberExists = _db.VillaNumbers.Any(u => u.VillaNum == obj.VillaNumber.VillaNum);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _db.VillaNumbers.Add(obj.VillaNumber);
            _db.SaveChanges();
            TempData["success"] = "The villa number has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The villa number is already exists!";
        }

        obj.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
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
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.VillaNum == villaNumberId)
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
            _db.VillaNumbers.Update(villaNumberVm.VillaNumber);
            _db.SaveChanges();
            TempData["success"] = "The villa number has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        villaNumberVm.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
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
            VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.VillaNum == villaNumberId)
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
        VillaNumber? objFromDb = _db.VillaNumbers
            .FirstOrDefault(u => u.VillaNum == villaNumberVm.VillaNumber.VillaNum);
        if (objFromDb is not null)
        {
            _db.VillaNumbers.Remove(objFromDb);
            _db.SaveChanges();
            TempData["success"] = "The villa number has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa could not be deleted!";
        return View();
    }

}