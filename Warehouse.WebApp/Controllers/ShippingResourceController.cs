using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Database;
using Models.Entities;
using Models.Enums;

namespace Warehouse.WebApp.Controllers
{
    public class ShippingResourceController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ShippingResourceController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: ShippingResource
        public async Task<IActionResult> Index()
        {
            var warehouseDbContext = _context.ShippingResources.Where(sr => sr.Condition == Condition.Active).Include(s => s.Resource);
            return View(await warehouseDbContext.ToListAsync());
        }

        // GET: ShippingResource
        public async Task<IActionResult> Archived()
        {
            var warehouseDbContext = _context.ShippingResources.Where(sr => sr.Condition == Condition.Archived).Include(s => s.Resource);
            return View(await warehouseDbContext.ToListAsync());
        }
        
        // GET: ShippingResource/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingResource = await _context.ShippingResources
                .Include(s => s.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingResource == null)
            {
                return NotFound();
            }

            return View(shippingResource);
        }

        // GET: ShippingResource/Create
        public IActionResult Create()
        {
            ViewData["ResourceId"] = new SelectList(_context.Resources.Where(r => r.Condition == Condition.Active), "Id", "Name");
            ViewData["UnitsOfMeasurement"] = new SelectList(_context.UnitsOfMeasurement.Where(u => u.Condition == Condition.Active), "Id", "Id");
            return View();
        }

        // POST: ShippingResource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResourceId,UnitOfMeasurementId,Count")] ShippingResource shippingResource)
        {
            ModelState.Remove("Resource");
            ModelState.Remove("UnitOfMeasurement");
            ModelState.Remove("Condition");
            if (ModelState.IsValid)
            {
                shippingResource.Id = Guid.NewGuid();
                _context.Add(shippingResource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources.Where(r => r.Condition == Condition.Active), "Id", "Name");
            ViewData["UnitsOfMeasurement"] = new SelectList(_context.UnitsOfMeasurement.Where(u => u.Condition == Condition.Active), "Id", "Id");
            return View(shippingResource);
        }

        // GET: ShippingResource/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingResource = await _context.ShippingResources.FindAsync(id);
            if (shippingResource == null)
            {
                return NotFound();
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", shippingResource.ResourceId);
            return View(shippingResource);
        }

        // POST: ShippingResource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ResourceId,UnitOfMeasurementId,Count,Condition")] ShippingResource shippingResource)
        {
            if (id != shippingResource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippingResource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingResourceExists(shippingResource.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", shippingResource.ResourceId);
            return View(shippingResource);
        }

        // GET: ShippingResource/Archive/5
        public async Task<IActionResult> Archive(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingResource = await _context.ShippingResources
                .Include(s => s.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingResource == null)
            {
                return NotFound();
            }

            return View(shippingResource);
        }

        // POST: ShippingResource/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(Guid id)
        {
            var shippingResource = await _context.ShippingResources.FindAsync(id);
            if (shippingResource != null)
            {
                shippingResource.Condition = Condition.Archived;
                _context.ShippingResources.Update(shippingResource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: ShippingResource/Activate/5
        public async Task<IActionResult> Activate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingResource = await _context.ShippingResources
                .Include(s => s.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingResource == null)
            {
                return NotFound();
            }

            return View(shippingResource);
        }

        // POST: ShippingResource/Activate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(Guid id)
        {
            var shippingResource = await _context.ShippingResources.FindAsync(id);
            if (shippingResource != null)
            {
                shippingResource.Condition = Condition.Active;
                _context.ShippingResources.Update(shippingResource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippingResourceExists(Guid id)
        {
            return _context.ShippingResources.Any(e => e.Id == id);
        }
    }
}
