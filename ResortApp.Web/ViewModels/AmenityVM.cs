using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResortApp.Domain.Entities;

namespace ResortApp.Web.ViewModels;

public class AmenityVM
{
    public Amenity? Amenity { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem>? VillaList { get; set; }
}