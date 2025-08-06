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
    public class ShippingDocumentController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ShippingDocumentController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: ShippingDocument
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShippingDocuments.Where(sd => sd.Condition != Condition.Archived).ToListAsync());
        }
        
        public async Task<IActionResult> Archived()
        {
            return View(await _context.ShippingDocuments.Where(sd => sd.Condition == Condition.Archived).ToListAsync());
        }

        // GET: ShippingDocument/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingDocument = await _context.ShippingDocuments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingDocument == null)
            {
                return NotFound();
            }

            return View(shippingDocument);
        }

        // GET: ShippingDocument/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShippingDocument/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Date,ClientId")] ShippingDocument shippingDocument)
        {
            ModelState.Remove("Condition");
            if (ModelState.IsValid)
            {
                shippingDocument.Id = Guid.NewGuid();
                _context.Add(shippingDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shippingDocument);
        }

        // GET: ShippingDocument/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingDocument = await _context.ShippingDocuments.FindAsync(id);
            if (shippingDocument == null)
            {
                return NotFound();
            }
            return View(shippingDocument);
        }

        // POST: ShippingDocument/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Number,Date,ClientId,Condition")] ShippingDocument shippingDocument)
        {
            if (id != shippingDocument.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippingDocument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingDocumentExists(shippingDocument.Id))
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
            return View(shippingDocument);
        }

        // GET: ShippingDocument/Archive/5
        public async Task<IActionResult> Archive(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingDocument = await _context.ShippingDocuments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingDocument == null)
            {
                return NotFound();
            }

            return View(shippingDocument);
        }

        // POST: ShippingDocument/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(Guid id)
        {
            var shippingDocument = await _context.ShippingDocuments.FindAsync(id);
            if (shippingDocument != null)
            {
                shippingDocument.Condition = Condition.Archived;
                _context.ShippingDocuments.Update(shippingDocument);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: ShippingDocument/Activate/5
        public async Task<IActionResult> Activate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingDocument = await _context.ShippingDocuments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingDocument == null)
            {
                return NotFound();
            }

            return View(shippingDocument);
        }

        // POST: ShippingDocument/Activate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(Guid id)
        {
            var shippingDocument = await _context.ShippingDocuments.FindAsync(id);
            if (shippingDocument != null)
            {
                shippingDocument.Condition = Condition.Active;
                _context.ShippingDocuments.Update(shippingDocument);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippingDocumentExists(Guid id)
        {
            return _context.ShippingDocuments.Any(e => e.Id == id);
        }
    }
}
