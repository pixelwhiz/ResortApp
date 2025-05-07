using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Domain.Entities;
using ResortApp.Web.ViewModels;

namespace ResortApp.Web.Controllers;

public class VillaNumberController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public VillaNumberController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
        return View(villaNumbers);
    }

    public IActionResult Create()
    {
        VillaNumberVM villaNumberVm = new()
        {
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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

        bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.VillaNum == obj.VillaNumber.VillaNum);

        if (ModelState.IsValid && !roomNumberExists)
        {
            _unitOfWork.VillaNumber.Add(obj.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "The villa number has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        if (roomNumberExists)
        {
            TempData["error"] = "The villa number is already exists!";
        }

        obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _unitOfWork.VillaNumber.Get(u => u.VillaNum == villaNumberId)
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
            _unitOfWork.VillaNumber.Update(villaNumberVm.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "The villa number has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        villaNumberVm.VillaList = _unitOfWork.Villa.GetAll().ToList().Select(u => new SelectListItem
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
            VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            VillaNumber = _unitOfWork.VillaNumber.Get(u => u.VillaNum == villaNumberId)
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
        VillaNumber? objFromDb = _unitOfWork.VillaNumber
            .Get(u => u.VillaNum == villaNumberVm.VillaNumber.VillaNum);
        if (objFromDb is not null)
        {
            _unitOfWork.VillaNumber.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "The villa number has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "The villa could not be deleted!";
        return View();
    }

}