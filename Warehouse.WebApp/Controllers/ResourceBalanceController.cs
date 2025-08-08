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
    public class ResourceBalanceController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ResourceBalanceController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: ResourceBalance
        public async Task<IActionResult> Index()
        {
            var warehouseDbContext = _context.ResourceBalances.Where(rb => rb.Condition != Condition.Archived).Include(r => r.Resource).Include(r => r.UnitOfMeasurement);
            ViewData["ResourcesList"] = _context.Resources.Where(r => r.Condition != Condition.Archived);
            ViewData["UnitOfMeasurementList"] = _context.UnitsOfMeasurement.Where(r => r.Condition != Condition.Archived);
            return View(await warehouseDbContext.ToListAsync()); 
        }
        
        // GET: ResourceBalance
        public async Task<IActionResult> Archived()
        {
            var warehouseDbContext = _context.ResourceBalances.Where(rb => rb.Condition == Condition.Archived).Include(r => r.Resource).Include(r => r.UnitOfMeasurement);
            ViewData["ResourcesList"] = _context.Resources.Where(r => r.Condition == Condition.Archived).ToList();
            ViewData["UnitOfMeasurementList"] = _context.UnitsOfMeasurement.Where(r => r.Condition == Condition.Archived);
            return View(await warehouseDbContext.ToListAsync());
        }

        // GET: ResourceBalance2/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceBalance = await _context.ResourceBalances
                .Include(r => r.Resource)
                .Include(r => r.UnitOfMeasurement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceBalance == null)
            {
                return NotFound();
            }

            return View(resourceBalance);
        }

        // GET: ResourceBalance2/Create
        public IActionResult Create()
        {
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id");
            return View();
        }

        // POST: ResourceBalance2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ResourceId,UnitOfMeasurementId,Count")] ResourceBalance resourceBalance)
        {
            ModelState.Remove("Condition");
            ModelState.Remove("Resource");
            ModelState.Remove("UnitOfMeasurement");
            if (ModelState.IsValid)
            {
                resourceBalance.Id = Guid.NewGuid();
                _context.Add(resourceBalance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", resourceBalance.ResourceId);
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id", resourceBalance.UnitOfMeasurementId);
            return View(resourceBalance);
        }

        // GET: ResourceBalance2/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceBalance = await _context.ResourceBalances.FindAsync(id);
            if (resourceBalance == null)
            {
                return NotFound();
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", resourceBalance.ResourceId);
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id", resourceBalance.UnitOfMeasurementId);
            return View(resourceBalance);
        }

        // POST: ResourceBalance2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ResourceId,UnitOfMeasurementId,Count,Condition")] ResourceBalance resourceBalance)
        {
            if (id != resourceBalance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resourceBalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceBalanceExists(resourceBalance.Id))
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
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", resourceBalance.ResourceId);
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id", resourceBalance.UnitOfMeasurementId);
            return View(resourceBalance);
        }

        // GET: ResourceBalance/Archive/5
        public async Task<IActionResult> Archive(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceBalance = await _context.ResourceBalances
                .Include(r => r.Resource)
                .Include(r => r.UnitOfMeasurement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceBalance == null)
            {
                return NotFound();
            }

            return View(resourceBalance);
        }

        // POST: ResourceBalance/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(Guid id)
        {
            var resourceBalance = await _context.ResourceBalances.FindAsync(id);
            if (resourceBalance != null)
            {
                resourceBalance.Condition = Condition.Archived;
                _context.ResourceBalances.Update(resourceBalance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ResourceBalance/Archive/5
        public async Task<IActionResult> Activate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceBalance = await _context.ResourceBalances
                .Include(r => r.Resource)
                .Include(r => r.UnitOfMeasurement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceBalance == null)
            {
                return NotFound();
            }

            return View(resourceBalance);
        }

        // POST: ResourceBalance/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(Guid id)
        {
            var resourceBalance = await _context.ResourceBalances.FindAsync(id);
            if (resourceBalance != null)
            {
                resourceBalance.Condition = Condition.Active;
                _context.ResourceBalances.Update(resourceBalance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceBalanceExists(Guid id)
        {
            return _context.ResourceBalances.Any(e => e.Id == id);
        }

        public IActionResult Search()
        {
            throw new NotImplementedException();
        }
        
        public IActionResult OnGetPartial() =>
            new PartialViewResult
            {
                ViewName = "ResourceBalancePartial",
                ViewData = ViewData
            };
    }
}
