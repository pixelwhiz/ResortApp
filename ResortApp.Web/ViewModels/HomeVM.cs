using ResortApp.Domain.Entities;

namespace ResortApp.Web.ViewModels;

public class HomeVM
{
    public IEnumerable<Villa>? VillaList { get; set; }
    public DateOnly CheckInDate { get; set; }
    public int Nights { get; set; }
}