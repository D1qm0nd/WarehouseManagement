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
            return View(await _context.UnitsOfMeasurement.ToListAsync());
        }

        // GET: UnitOfMeasurement/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasurement = await _context.UnitsOfMeasurement
                .FirstOrDefaultAsync(m => m.Name == id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }

            return View(unitOfMeasurement);
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
        public async Task<IActionResult> Create([Bind("Name,Condition")] UnitOfMeasurement unitOfMeasurement)
        {
            if (ModelState.IsValid)
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
            if (id != unitOfMeasurement.Name)
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
                    if (!UnitOfMeasurementExists(unitOfMeasurement.Name))
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
                .FirstOrDefaultAsync(m => m.Name == id);
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

        private bool UnitOfMeasurementExists(string id)
        {
            return _context.UnitsOfMeasurement.Any(e => e.Name == id);
        }
    }
}
