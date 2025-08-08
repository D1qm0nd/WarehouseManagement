using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Enums;

namespace Warehouse.WebApp.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ReceiptsController(WarehouseDbContext context)
        {
            _context = context;
        }
        
        // GET: ReceiptsController
        public async Task<ActionResult> Index()
        {
            var entities = await _context.ReceiptDocuments.Where(rd => rd.Condition != Condition.Archived)
                .Include(rd => rd.ReceiptResources).ThenInclude(rr => rr.Resource).ToListAsync();
            return View(entities);
        }
    }
}
