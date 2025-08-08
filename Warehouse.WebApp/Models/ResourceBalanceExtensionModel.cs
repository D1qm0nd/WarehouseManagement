using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

namespace Warehouse.WebApp.Models;

public class ResourceBalanceExtensionModel
{
    public IEnumerable<ResourceBalance> ResourceBalances { get; set; }
    public IEnumerable<SelectListItem> SelectListItemsResource
    {
        get
        {
            foreach (var item in ResourceBalances)
            {
                yield return new SelectListItem(item.ResourceId.ToString(), item.Resource.Name);
            }
        }
    }
}