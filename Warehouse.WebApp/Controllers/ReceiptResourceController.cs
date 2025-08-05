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
    public class ReceiptResourceController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ReceiptResourceController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: ReceiptResource
        public async Task<IActionResult> Index()
        {
            var warehouseDbContext = _context.ResourcesOfReceipt.Include(r => r.DocumentOfReceipt).Include(r => r.Resource);
            return View(await warehouseDbContext.ToListAsync());
        }

        // GET: ReceiptResource/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptResource = await _context.ResourcesOfReceipt
                .Include(r => r.DocumentOfReceipt)
                .Include(r => r.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receiptResource == null)
            {
                return NotFound();
            }

            return View(receiptResource);
        }

        // GET: ReceiptResource/Create
        public IActionResult Create()
        {
            ViewData["ReceiptDocumentId"] = new SelectList(_context.ReceiptDocuments, "Id", "Id");
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
            return View();
        }

        // POST: ReceiptResource/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceiptDocumentId,ResourceId,UnitOfMeasurementId,Count")] ReceiptResource receiptResource)
        {
            ModelState.Remove("Condition");
            if (ModelState.IsValid)
            {
                receiptResource.Id = Guid.NewGuid();
                _context.Add(receiptResource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiptDocumentId"] = new SelectList(_context.ReceiptDocuments, "Id", "Id", receiptResource.ReceiptDocumentId);
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", receiptResource.ResourceId);
            return View(receiptResource);
        }

        // GET: ReceiptResource/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptResource = await _context.ResourcesOfReceipt.FindAsync(id);
            if (receiptResource == null)
            {
                return NotFound();
            }
            ViewData["ReceiptDocumentId"] = new SelectList(_context.ReceiptDocuments, "Id", "Id", receiptResource.ReceiptDocumentId);
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", receiptResource.ResourceId);
            return View(receiptResource);
        }

        // POST: ReceiptResource/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ReceiptDocumentId,ResourceId,UnitOfMeasurementId,Count,Condition")] ReceiptResource receiptResource)
        {
            if (id != receiptResource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receiptResource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceiptResourceExists(receiptResource.Id))
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
            ViewData["ReceiptDocumentId"] = new SelectList(_context.ReceiptDocuments, "Id", "Id", receiptResource.ReceiptDocumentId);
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", receiptResource.ResourceId);
            return View(receiptResource);
        }

        // GET: ReceiptResource/Delete/5
        public async Task<IActionResult> Archive(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptResource = await _context.ResourcesOfReceipt
                .Include(r => r.DocumentOfReceipt)
                .Include(r => r.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receiptResource == null)
            {
                return NotFound();
            }

            return View(receiptResource);
        }

        // POST: ReceiptResource/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(Guid id)
        {
            var receiptResource = await _context.ResourcesOfReceipt.FindAsync(id);
            if (receiptResource != null)
            {
                receiptResource.Condition = Condition.Archived;
                _context.ResourcesOfReceipt.Update(receiptResource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptResourceExists(Guid id)
        {
            return _context.ResourcesOfReceipt.Any(e => e.Id == id);
        }
    }
}
