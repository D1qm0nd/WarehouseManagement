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
    public class UnitOfMeasurementController : Controller
    {
        private readonly WarehouseDbContext _context;

        public UnitOfMeasurementController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: UnitOfMeasurement
        public async Task<IActionResult> Index()
        {
            return View(await _context.UnitsOfMeasurement.Where(u => u.Condition != Condition.Archived).ToListAsync());
        }
        
        // GET: UnitOfMeasurement
        public async Task<IActionResult> Archived()
        {
            return View(await _context.UnitsOfMeasurement.Where(u => u.Condition == Condition.Archived).ToListAsync());
        }

        // GET: UnitOfMeasurement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnitOfMeasurement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Condition")] UnitOfMeasurement unitOfMeasurement)
        {
            if (ModelState.IsValid && !_context.UnitsOfMeasurement.Any(u => u.Id == unitOfMeasurement.Id))
            {
                _context.Add(unitOfMeasurement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unitOfMeasurement);
        }

        // GET: UnitOfMeasurement/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasurement = await _context.UnitsOfMeasurement.FindAsync(id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }
            return View(unitOfMeasurement);
        }

        // POST: UnitOfMeasurement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Condition")] UnitOfMeasurement unitOfMeasurement)
        {
            if (id != unitOfMeasurement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unitOfMeasurement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitOfMeasurementExists(unitOfMeasurement.Id))
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
            return View(unitOfMeasurement);
        }

        // GET: UnitOfMeasurement/Archive/5
        public async Task<IActionResult> Archive(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasurement = await _context.UnitsOfMeasurement
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }

            return View(unitOfMeasurement);
        }

        // POST: UnitOfMeasurement/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(string id)
        {
            var unitOfMeasurement = await _context.UnitsOfMeasurement.FindAsync(id);
            if (unitOfMeasurement != null)
            {
                unitOfMeasurement.Condition = Condition.Archived;
                _context.UnitsOfMeasurement.Update(unitOfMeasurement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: UnitOfMeasurement/Activate/5
        public async Task<IActionResult> Activate(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasurement = await _context.UnitsOfMeasurement
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }

            return View(unitOfMeasurement);
        }

        // POST: UnitOfMeasurement/Archive/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(string id)
        {
            var unitOfMeasurement = await _context.UnitsOfMeasurement.FindAsync(id);
            if (unitOfMeasurement != null)
            {
                unitOfMeasurement.Condition = Condition.Active;
                _context.UnitsOfMeasurement.Update(unitOfMeasurement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitOfMeasurementExists(string id)
        {
            return _context.UnitsOfMeasurement.Any(e => e.Id == id);
        }
    }
}
