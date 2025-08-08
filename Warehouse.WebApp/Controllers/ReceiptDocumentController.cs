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
    public class ReceiptDocumentController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ReceiptDocumentController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: ReceiptDocument
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReceiptDocuments.Where(rd => rd.Condition != Condition.Archived).ToListAsync());
        }
        public async Task<IActionResult> Archived()
        {
            return View(await _context.ReceiptDocuments.Where(rd => rd.Condition == Condition.Archived).ToListAsync());
        }

        // GET: ReceiptDocument/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptDocument = await _context.ReceiptDocuments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receiptDocument == null)
            {
                return NotFound();
            }

            return View(receiptDocument);
        }

        // GET: ReceiptDocument/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReceiptDocument/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Date")] ReceiptDocument receiptDocument)
        {
            ModelState.Remove("Condition");
            ModelState.Remove("Date");
            ModelState.Remove("ReceiptResources");
            if (ModelState.IsValid && !_context.CheckReceiptDocumentWithNumber(receiptDocument))
            {
                receiptDocument.Id = Guid.NewGuid();
                receiptDocument.Date = receiptDocument.Date.ToUniversalTime();
                _context.Add(receiptDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(receiptDocument);
        }

        // GET: ReceiptDocument/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptDocument = await _context.ReceiptDocuments.FindAsync(id);
            if (receiptDocument == null)
            {
                return NotFound();
            }
            return View(receiptDocument);
        }

        // POST: ReceiptDocument/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Number,Date,Condition")] ReceiptDocument receiptDocument)
        {
            if (id != receiptDocument.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receiptDocument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceiptDocumentExists(receiptDocument.Id))
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
            return View(receiptDocument);
        }

        // GET: ReceiptDocument/Archive/5
        public async Task<IActionResult> Archive(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptDocument = await _context.ReceiptDocuments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receiptDocument == null)
            {
                return NotFound();
            }

            return View(receiptDocument);
        }

        // POST: ReceiptDocument/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(Guid id)
        {
            var receiptDocument = await _context.ReceiptDocuments.FindAsync(id);
            if (receiptDocument != null)
            {
                receiptDocument.Condition = Condition.Archived;
                _context.ReceiptDocuments.Update(receiptDocument);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: ReceiptDocument/Activate/5
        public async Task<IActionResult> Activate(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receiptDocument = await _context.ReceiptDocuments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receiptDocument == null)
            {
                return NotFound();
            }

            return View(receiptDocument);
        }

        // POST: ReceiptDocument/Activate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(Guid id)
        {
            var receiptDocument = await _context.ReceiptDocuments.FindAsync(id);
            if (receiptDocument != null)
            {
                receiptDocument.Condition = Condition.Active;
                _context.ReceiptDocuments.Update(receiptDocument);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptDocumentExists(Guid id)
        {
            return _context.ReceiptDocuments.Any(e => e.Id == id);
        }
    }
}
