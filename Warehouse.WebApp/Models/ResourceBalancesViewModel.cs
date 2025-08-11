using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Models.Enums;

namespace Warehouse.WebApp.Models;

public class ResourceBalancesViewModel
{
    public IEnumerable<ResourceBalance> Balances { get; set; }
    public List<SelectListItem> ResourcesForSearch { get; set; }
    public List<SelectListItem> UnitForSearch { get; set; }
}

