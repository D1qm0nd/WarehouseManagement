using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Database;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Primitives;
using Models.Entities;
using Models.Enums;
using Warehouse.WebApp.Models;

namespace Warehouse.WebApp.Controllers
{
    public class ResourceBalanceController : Controller
    {
        private readonly WarehouseDbContext _context;
        private ResourceBalancesViewModel _viewModel;

        public ResourceBalanceController(WarehouseDbContext context)
        {
            _context = context;
            _viewModel = new ResourceBalancesViewModel()
            {
                ResourcesForSearch = _context.Resources.Where(r => r.Condition != Condition.Archived)
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList(),
                UnitForSearch = _context.UnitsOfMeasurement.Where(u => u.Condition != Condition.Archived)
                    .Select(u => new SelectListItem(u.Id, u.Id)).ToList(),
                Balances = GetResourceBalances()
            };
        }

        // GET: ResourceBalance
        public async Task<IActionResult> Index()
        {
            // var warehouseDbContext =
            //     _context.ResourceBalances
            //         .Where(rb => rb.Condition != Condition.Archived)
            //         .Include(r => r.Resource)
            //         .Include(r => r.UnitOfMeasurement);
            // _viewModel.Balances = warehouseDbContext.ToList();
            return View(_viewModel);
        }

        public async Task<IActionResult> ClearSearchResult()
        {
            _viewModel.Balances = GetResourceBalances();
            return View(nameof(Index), _viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Search()
        {
            var selectedResources = Request.Form["resourceSelector"];
            var selectUnits = Request.Form["unitSelector"];

            var res = await Search(selectedResources, selectUnits).ToListAsync();
            _viewModel.Balances = res;

            return View(nameof(Index),_viewModel);
        }

        public List<ResourceBalance> GetResourceBalances() => _context.ResourceBalances
            .Include(r => r.Resource)
            .ToList();

        private IQueryable<ResourceBalance> Search(StringValues selectedResources, StringValues selectUnits)
        {
            return _context.ResourceBalances
                .Include(r => r.Resource)
                .Where(rb =>
                    selectUnits.Contains(rb.Resource.UnitOfMeasurementId) ||
                    selectedResources.Contains(rb.Resource.Id.ToString()));
        }

        public IActionResult OnGetPartial() =>
            new PartialViewResult
            {
                ViewName = "ResourceBalance",
                ViewData = ViewData
            };
    }
}