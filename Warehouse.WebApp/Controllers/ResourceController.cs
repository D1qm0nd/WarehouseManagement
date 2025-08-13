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
    public class ResourceController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ResourceController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: Resource
        public async Task<IActionResult> Index()
        {
            return View(await _context.Resources.Where(r => r.Condition != Condition.Archived).ToListAsync());
        }

        // GET: Resource
        public async Task<IActionResult> Archived()
        {
            return View(await _context.Resources.Where(r => r.Condition == Condition.Archived).ToListAsync());
        }

        // GET: Resource/Create
        public IActionResult Create()
        {
            ViewData["UnitsOfMeasurement"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id");
            return View();
        }

        // POST: Resource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UnitOfMeasurementId")] Resource resource)
        {
            ModelState.Remove("Condition");
            ModelState.Remove(nameof(resource.UnitOfMeasurement));
            var exists = _context.CheckResourceExists(resource);
            if (ModelState.IsValid && exists == false)
            {
                resource.Id = Guid.NewGuid();
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UnitsOfMeasurement"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id");
            return View(resource);
        }

        // GET: Resource/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            ViewData["UnitsOfMeasurement"] = new SelectList(_context.UnitsOfMeasurement, "Id", "Id");
            return View(resource);
        }

        // POST: Resource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,UnitOfMeasurementId")] Resource resource)
        {
            if (id != resource.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Condition");
            if (ModelState.IsValid && !_context.CheckOtherResourceWithNameExists(resource))
            {
                try
                {
                    _context.Update(resource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceExists(resource.Id))
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

            return View(resource);
        }

        // GET: Resource/Delete/5
        public async Task<IActionResult> Archive(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: Resource/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                resource.Condition = Condition.Archived;
                _context.Resources.Update(resource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Resource/Delete/5
        public async Task<IActionResult> Activate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: Resource/Delete/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                resource.Condition = Condition.Active;
                _context.Resources.Update(resource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ResourceExists(Guid id)
        {
            return _context.Resources.Any(e => e.Id == id);
        }
    }
}