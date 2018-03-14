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
    public class ScopeController : Controller
    {
        private readonly ProjectDbContext _context;

        public ScopeController(ProjectDbContext context)
        {
            _context = context;
        }

        // GET: ScopeModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Scopes.ToListAsync());
        }

        // GET: ScopeModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopeModel = await _context.Scopes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (scopeModel == null)
            {
                return NotFound();
            }

            return View(scopeModel);
        }

        // GET: ScopeModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScopeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProjectId,ScopeVersion,ScopeAuthor,ScopeManager,ScopeExpectations,ScopeLimitations,ScopeSummary,ScopeGoals,ScopePhase,ScopeStartDate,ScopeEndDate")] ScopeModel scopeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scopeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scopeModel);
        }

        // GET: ScopeModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopeModel = await _context.Scopes.SingleOrDefaultAsync(m => m.ID == id);
            if (scopeModel == null)
            {
                return NotFound();
            }
            return View(scopeModel);
        }

        // POST: ScopeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProjectId,ScopeVersion,ScopeAuthor,ScopeManager,ScopeExpectations,ScopeLimitations,ScopeSummary,ScopeGoals,ScopePhase,ScopeStartDate,ScopeEndDate")] ScopeModel scopeModel)
        {
            if (id != scopeModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scopeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScopeModelExists(scopeModel.ID))
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
            return View(scopeModel);
        }

        // GET: ScopeModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopeModel = await _context.Scopes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (scopeModel == null)
            {
                return NotFound();
            }

            return View(scopeModel);
        }

        // POST: ScopeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scopeModel = await _context.Scopes.SingleOrDefaultAsync(m => m.ID == id);
            _context.Scopes.Remove(scopeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScopeModelExists(int id)
        {
            return _context.Scopes.Any(e => e.ID == id);
        }
    }
}
