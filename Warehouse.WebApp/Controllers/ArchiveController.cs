using Microsoft.AspNetCore.Mvc;

namespace Warehouse.WebApp.Controllers;

public class ArchiveController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}