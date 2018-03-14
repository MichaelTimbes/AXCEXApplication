using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;

namespace AXCEXONLINE.Controllers
{
    public class WBSController : Controller
    {
        private readonly ProjectDbContext _context;

        public WBSController(ProjectDbContext context)
        {
            _context = context;
        }

        // GET: WBSModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkBreakDowns.ToListAsync());
        }

        // GET: WBSModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBSModel = await _context.WorkBreakDowns
                .SingleOrDefaultAsync(m => m.ID == id);
            if (wBSModel == null)
            {
                return NotFound();
            }

            return View(wBSModel);
        }

        // GET: WBSModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WBSModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProjectId,AssignedBy,WBSSummary,WBSHours,StartDate,EndDate,WBSCost")] WBSModel wBSModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wBSModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wBSModel);
        }

        // GET: WBSModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBSModel = await _context.WorkBreakDowns.SingleOrDefaultAsync(m => m.ID == id);
            if (wBSModel == null)
            {
                return NotFound();
            }
            return View(wBSModel);
        }

        // POST: WBSModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProjectId,AssignedBy,WBSSummary,WBSHours,StartDate,EndDate,WBSCost")] WBSModel wBSModel)
        {
            if (id != wBSModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wBSModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WBSModelExists(wBSModel.ID))
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
            return View(wBSModel);
        }

        // GET: WBSModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBSModel = await _context.WorkBreakDowns
                .SingleOrDefaultAsync(m => m.ID == id);
            if (wBSModel == null)
            {
                return NotFound();
            }

            return View(wBSModel);
        }

        // POST: WBSModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wBSModel = await _context.WorkBreakDowns.SingleOrDefaultAsync(m => m.ID == id);
            _context.WorkBreakDowns.Remove(wBSModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WBSModelExists(int id)
        {
            return _context.WorkBreakDowns.Any(e => e.ID == id);
        }
    }
}
